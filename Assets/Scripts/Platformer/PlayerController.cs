using System;
using UnityEngine;

namespace Platformer
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        [SerializeField] private PlatformerInput platformerInput;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput= new()
        {
            JumpDown = false,
            JumpHeld = false,
            Move = new Vector2(0, 0)
        };
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action<bool> WallSlidingChanged;
        
        public event Action WallJumped;
        public event Action Jumped;

        #endregion

        private float _time;
        
        private bool areInputsLocked = true;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
            // Prevent player from jumping with inputs locked
            _timeJumpWasPressed = - _stats.JumpBuffer;
        }
        
        private void Update()
        {
            if (areInputsLocked)
                return;
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = platformerInput.GetJumpPressed(),
                JumpHeld = platformerInput.GetJumpDown(),
                Move = platformerInput.GetMovementVector()
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
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
            int collidableLayers = ~_stats.PlayerLayer;
            collidableLayers &= ~(1 << LayerMask.NameToLayer($"IsTrigger"));
            
            int ceilingCollidableLayers = collidableLayers & ~(1 << LayerMask.NameToLayer($"Platform"));
            // Ground and Ceiling
            groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, collidableLayers);
            RaycastHit2D ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ceilingCollidableLayers);
            bool rightWallHit =Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.GrounderDistance, collidableLayers);
            bool leftWallHit =Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.GrounderDistance,collidableLayers);

            
            
            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!isGrounded && groundHit)
            {
                isGrounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
                IsWallSliding = false;
            }
            // Left the Ground
            else if (isGrounded && !groundHit)
            {
                isGrounded = false;
                frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            if (!isGrounded)
            {
                if (!IsWallSliding && (rightWallHit || leftWallHit))
                {
                    IsWallSliding = true;
                    IsOnLeftWall = leftWallHit;
                    _bufferedJumpUsable = true;
                    _endedJumpEarly = false;
                    WallSlidingChanged?.Invoke(true);
                }else if (IsWallSliding && !rightWallHit && !leftWallHit)
                {
                    IsWallSliding = false;
                    WallSlidingChanged?.Invoke(false);
                }
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion


        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;
        private float _timeWallJumpingStarted;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !isGrounded && _time < frameLeftGrounded + _stats.CoyoteTime;
        private bool IsWallJumping => !isGrounded && !IsWallSliding && _time < _timeWallJumpingStarted + _stats.WallJumpLockTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !isGrounded && !IsWallSliding && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (isGrounded || IsWallSliding || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            hasUsedBumper = false;
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = IsWallSliding ? _stats.WallJumpYPower:_stats.JumpPower;
            if (IsWallSliding)
            {
                _frameVelocity.x = IsOnLeftWall ? _stats.WallJumpXPower : -_stats.WallJumpXPower;
                _timeWallJumpingStarted = _time;
                WallJumped?.Invoke();
            }
            Jumped?.Invoke();
        }

        #endregion

        #region Bumper

        // Determines if the upward velocity is caused by the bumper or the jump
        private bool hasUsedBumper = false;
        
        public void ExecuteBumper()
        {
            hasUsedBumper = true;
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.BumperPower;
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
            
            if (_frameInput.Move.x == 0)
            {
                var deceleration = isGrounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                if (groundHit && groundHit.collider.name == "Ice")
                    deceleration /= 10;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                float acceleration = _stats.Acceleration;
                if (groundHit && groundHit.collider.name == "Ice")
                    acceleration /= 10;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (isGrounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else if (IsWallSliding && _frameVelocity.y < 0f)
            {
                float wallSlidingYTargetVelocity = -_stats.MaxSlideSpeed;
                if(_frameInput.Move.y < 0f)
                    wallSlidingYTargetVelocity = -_stats.MaxFallSpeed;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, wallSlidingYTargetVelocity, _stats.SlideAcceleration * Time.fixedDeltaTime);
                if (_frameVelocity.y < wallSlidingYTargetVelocity)
                    _frameVelocity.y = wallSlidingYTargetVelocity;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0 && !hasUsedBumper) 
                    inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
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
        public event Action<bool> WallSlidingChanged;
        public event Action WallJumped;
        public Vector2 FrameInput { get; }
        public bool IsWallSliding { get;  }
        public bool IsOnLeftWall{ get;  }
    }
}