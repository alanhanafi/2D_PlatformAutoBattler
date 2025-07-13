using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Platformer
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [FormerlySerializedAs("_stats")] [SerializeField] private ScriptableStats stats;
        [SerializeField] private PlatformerInput platformerInput;
        [SerializeField] private bool areInitialInputsLocked = true;
        [SerializeField] private float fastMaxSpeedValue = 15;
        
        internal bool IsDoubleJumpActive { get; set; }
        internal bool IsFast { get; set; }
        private Rigidbody2D rb;
        private CapsuleCollider2D col;
        private FrameInput frameInput= new()
        {
            JumpDown = false,
            JumpHeld = false,
            Move = new Vector2(0, 0)
        };
        private Vector2 frameVelocity;
        private bool cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action<bool> WallSlidingChanged;
        
        public event Action WallJumped;
        public event Action Jumped;
        public event Action DoubleJumped;

        #endregion

        private float time;
        
        private bool areInputsLocked = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CapsuleCollider2D>();

            cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
            // Prevent player from jumping with inputs locked
            timeJumpWasPressed = - stats.JumpBuffer;
            areInputsLocked = areInitialInputsLocked;
        }
        
        private void Update()
        {
            if (areInputsLocked)
                return;
            time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            frameInput = new FrameInput
            {
                JumpDown = platformerInput.GetJumpPressed(),
                JumpHeld = platformerInput.GetJumpDown(),
                Move = platformerInput.GetMovementVector()
            };

            if (stats.SnapInput)
            {
                frameInput.Move.x = Mathf.Abs(frameInput.Move.x) < stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(frameInput.Move.x);
                frameInput.Move.y = Mathf.Abs(frameInput.Move.y) < stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(frameInput.Move.y);
            }

            if (frameInput.JumpDown)
            {
                jumpToConsume = true;
                timeJumpWasPressed = time;
            }
        }
        

        public void UnlockInputs()
        {
            areInputsLocked = false;
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            HandleGravity();
            
            ApplyMovement();
        }

        #region Collisions
        
        private float frameLeftGrounded = float.MinValue;
        private bool isGrounded;
        public bool IsWallSliding { get; private set; }
        public bool IsOnLeftWall{ get; private set; }
        private RaycastHit2D groundHit;

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;
            int collidableLayers = ~stats.PlayerLayer;
            collidableLayers &= ~(1 << LayerMask.NameToLayer($"IsTrigger"));
            
            int ceilingCollidableLayers = collidableLayers & ~(1 << LayerMask.NameToLayer($"Platform"));
            // Ground and Ceiling
            groundHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down, stats.GrounderDistance, collidableLayers);
            RaycastHit2D ceilingHit = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.up, stats.GrounderDistance, ceilingCollidableLayers);
            bool rightWallHit =Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.right, stats.GrounderDistance, collidableLayers);
            bool leftWallHit =Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.left, stats.GrounderDistance,collidableLayers);

            
            
            // Hit a Ceiling
            if (ceilingHit) frameVelocity.y = Mathf.Min(0, frameVelocity.y);

            // Landed on the Ground
            if (!isGrounded && groundHit)
            {
                isGrounded = true;
                canDoubleJump = false;
                coyoteUsable = true;
                bufferedJumpUsable = true;
                endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(frameVelocity.y));
                IsWallSliding = false;
            }
            // Left the Ground
            else if (isGrounded && !groundHit)
            {
                isGrounded = false;
                frameLeftGrounded = time;
                GroundedChanged?.Invoke(false, 0);
            }

            if (!isGrounded)
            {
                if (!IsWallSliding && (rightWallHit || leftWallHit))
                {
                    IsWallSliding = true;
                    canDoubleJump = false;
                    IsOnLeftWall = leftWallHit;
                    bufferedJumpUsable = true;
                    endedJumpEarly = false;
                    WallSlidingChanged?.Invoke(true);
                }else if (IsWallSliding && !rightWallHit && !leftWallHit)
                {
                    IsWallSliding = false;
                    WallSlidingChanged?.Invoke(false);
                }
            }

            Physics2D.queriesStartInColliders = cachedQueryStartInColliders;
        }

        #endregion


        #region Jumping

        private bool jumpToConsume;
        private bool bufferedJumpUsable;
        private bool endedJumpEarly;
        private bool coyoteUsable;
        private float timeJumpWasPressed;
        private float timeWallJumpingStarted;
        private bool canDoubleJump;

        private bool HasBufferedJump => bufferedJumpUsable && time < timeJumpWasPressed + stats.JumpBuffer;
        private bool CanUseCoyote => coyoteUsable && !isGrounded && time < frameLeftGrounded + stats.CoyoteTime;
        private bool IsWallJumping => !isGrounded && !IsWallSliding && time < timeWallJumpingStarted + stats.WallJumpLockTime;

        private bool CanDoubleJump => canDoubleJump //&& IsDoubleJumpActive 
                                      || coyoteUsable; // Allows to double jump while falling

        private void HandleJump()
        {
            if (!endedJumpEarly && !isGrounded && !IsWallSliding && !frameInput.JumpHeld && rb.linearVelocity.y > 0) 
                endedJumpEarly = true;

            if (!jumpToConsume && !HasBufferedJump) 
                return;

            if (isGrounded || IsWallSliding || CanUseCoyote) 
                ExecuteJump();
            else if (CanDoubleJump)
                ExecuteDoubleJump();

            jumpToConsume = false;
        }

        private void ExecuteDoubleJump()
        {
            ExecuteJump();
            DoubleJumped?.Invoke();
            canDoubleJump = false;
        }

        private void ExecuteJump()
        {
            hasUsedBumper = false;
            SetJumpVariablesAfterJump();
            frameVelocity.y = IsWallSliding ? stats.WallJumpYPower:stats.JumpPower;
            if (IsWallSliding)
            {
                frameVelocity.x = IsOnLeftWall ? stats.WallJumpXPower : -stats.WallJumpXPower;
                timeWallJumpingStarted = time;
                WallJumped?.Invoke();
            }
            Jumped?.Invoke();
        }

        private void SetJumpVariablesAfterJump()
        {
            canDoubleJump = true;
            endedJumpEarly = false;
            timeJumpWasPressed = 0;
            bufferedJumpUsable = false;
            coyoteUsable = false;
        }

        #endregion

        #region Bumper

        // Determines if the upward velocity is caused by the bumper or the jump
        private bool hasUsedBumper = false;

        public void ExecuteBumper()
        {
            hasUsedBumper = true;
            SetJumpVariablesAfterJump();
            frameVelocity.y = stats.BumperPower;
            // TODO : Check if we need a jumped event or something else
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            //lock direction inputs while wall jumping
            if (IsWallJumping)
                return;
            
            if (frameInput.Move.x == 0)
            {
                var deceleration = isGrounded ? stats.GroundDeceleration : stats.AirDeceleration;
                if (groundHit && groundHit.collider.name == "Ice")
                    deceleration /= 10;
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                float acceleration = stats.Acceleration;
                if (groundHit && groundHit.collider.name == "Ice")
                    acceleration /= 10;
                float maxSpeed = IsFast? fastMaxSpeedValue:stats.MaxSpeed;
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, frameInput.Move.x * maxSpeed, acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (isGrounded && frameVelocity.y <= 0f)
            {
                frameVelocity.y = stats.GroundingForce;
            }
            else if (IsWallSliding && frameVelocity.y < 0f)
            {
                float wallSlidingYTargetVelocity = -stats.MaxSlideSpeed;
                if(frameInput.Move.y < 0f)
                    wallSlidingYTargetVelocity = -stats.MaxFallSpeed;
                frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, wallSlidingYTargetVelocity, stats.SlideAcceleration * Time.fixedDeltaTime);
                if (frameVelocity.y < wallSlidingYTargetVelocity)
                    frameVelocity.y = wallSlidingYTargetVelocity;
            }
            else
            {
                var inAirGravity = stats.FallAcceleration;
                if (endedJumpEarly && frameVelocity.y > 0 && !hasUsedBumper) 
                    inAirGravity *= stats.JumpEndEarlyGravityModifier;
                frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement() => rb.linearVelocity = frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;
        public event Action DoubleJumped;
        public event Action<bool> WallSlidingChanged;
        public event Action WallJumped;
        public Vector2 FrameInput { get; }
        public bool IsWallSliding { get;  }
        public bool IsOnLeftWall{ get;  }
    }
}