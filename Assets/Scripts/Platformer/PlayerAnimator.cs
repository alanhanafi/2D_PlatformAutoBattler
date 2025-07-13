using System;
using UnityEngine;

namespace Platformer
{
    public class PlayerAnimator : MonoBehaviour
    {
        // TODO : Update naming for Serialize Fields
        [Header("References")] [SerializeField]
        private Animator _anim;

        [SerializeField] private SpriteRenderer _sprite;

        [Header("Settings")] [SerializeField, Range(1f, 3f)]
        private float _maxIdleSpeed = 2;

        [SerializeField] private float _maxTilt = 5;
        [SerializeField] private float _tiltSpeed = 20;

        [Header("Particles")] [SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private ParticleSystem _launchParticles;
        [SerializeField] private ParticleSystem _moveParticles;
        [SerializeField] private ParticleSystem _landParticles;
        [SerializeField] private ParticleSystem _doubleJumpParticles;

        [Header("Audio Clips")] [SerializeField]
        private AudioClip[] _footsteps;

        private AudioSource audioSource;
        private IPlayerController player;
        private Rigidbody2D rb2D;
        private bool grounded;
        private ParticleSystem.MinMaxGradient currentGradient;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            player = GetComponentInParent<IPlayerController>();
            rb2D  = GetComponentInParent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            player.Jumped += OnJumped;
            player.DoubleJumped += OnDoubleJumped;
            player.GroundedChanged += OnGroundedChanged;
            player.WallSlidingChanged += OnWallSlidingChanged;
            player.WallJumped += OnWallJump;
            _moveParticles.Play();
        }

        private void OnDisable()
        {
            player.Jumped -= OnJumped;
            player.DoubleJumped -= OnDoubleJumped;
            player.GroundedChanged -= OnGroundedChanged;
            player.WallSlidingChanged -= OnWallSlidingChanged;
            player.WallJumped -= OnWallJump;

            _moveParticles.Stop();
        }

        private void Update()
        {
            if (player == null) return;

            DetectGroundColor();

            HandleSpriteFlip();

            HandleIdleSpeed();

            HandleVelocity();

            //HandleCharacterTilt();
        }

        private void HandleVelocity()
        {
            _anim.SetFloat(XVelocityKey, Math.Abs(rb2D.linearVelocityX));
            _anim.SetFloat(YVelocityKey, grounded?0:rb2D.linearVelocityY);
        }

        private void HandleSpriteFlip()
        {
            if (player.FrameInput.x != 0) _sprite.flipX = player.FrameInput.x < 0;
            if(player.IsWallSliding)
                _sprite.flipX = player.IsOnLeftWall;
        }

        private void OnWallJump()
        {
            FlipSprite();
        }

        public void FlipSprite()
        {
            _sprite.flipX = !_sprite.flipX;
        }

        private void HandleIdleSpeed()
        {
            var inputStrength = Mathf.Abs(player.FrameInput.x);
            _anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, _maxIdleSpeed, inputStrength));
            _moveParticles.transform.localScale = Vector3.MoveTowards(_moveParticles.transform.localScale, Vector3.one * inputStrength, 2 * Time.deltaTime);
        }

        private void HandleCharacterTilt()
        {
            var runningTilt = grounded ? Quaternion.Euler(0, 0, _maxTilt * player.FrameInput.x) : Quaternion.identity;
            _anim.transform.up = Vector3.RotateTowards(_anim.transform.up, runningTilt * Vector2.up, _tiltSpeed * Time.deltaTime, 0f);
        }

        private void OnJumped()
        {
            _anim.SetTrigger(JumpKey);
            _anim.ResetTrigger(GroundedKey);
            _anim.SetBool(WallSlidingKey,false);


            if (grounded) // Avoid coyote
                _jumpParticles.Play();
        }
        

        private void OnDoubleJumped()
        {
            Debug.Log($"OnDoubleJumped");
            _anim.SetTrigger(JumpKey);
            _anim.ResetTrigger(GroundedKey);
            _anim.SetBool(WallSlidingKey,false);
            
            _doubleJumpParticles.Play();
        }


        private void OnGroundedChanged(bool grounded, float impact)
        {
            this.grounded = grounded;
            
            if (grounded)
            {
                DetectGroundColor();
                //SetColor(_landParticles);

                _anim.SetTrigger(GroundedKey);
                _anim.SetBool(WallSlidingKey,false);
                //audioSource.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
                _moveParticles.Play();

                _landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
                _landParticles.Play();
            }
            else
            {
                _moveParticles.Stop();
            }
        }
        
        private bool isWallSliding = false;

        private void OnWallSlidingChanged(bool isWallSliding)
        {
            this.isWallSliding = isWallSliding;
            // Add particles on wall
            _anim.SetBool(WallSlidingKey, isWallSliding);
        }

        private void DetectGroundColor()
        {
            var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

            if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
            var color = r.color;
            currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
            SetColor(_moveParticles);
        }

        private void SetColor(ParticleSystem ps)
        {
            var main = ps.main;
            main.startColor = currentGradient;
        }

        private static readonly int GroundedKey = Animator.StringToHash("Grounded");
        private static readonly int WallSlidingKey = Animator.StringToHash("WallSliding");
        private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
        private static readonly int JumpKey = Animator.StringToHash("Jump");
        private static readonly int XVelocityKey = Animator.StringToHash("XVelocity");
        private static readonly int YVelocityKey = Animator.StringToHash("YVelocity");
    }
}