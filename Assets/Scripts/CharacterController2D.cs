using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterController2D : MonoBehaviour
{
	private const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	
	[SerializeField] private float groundJumpForce = 400f;
	[SerializeField] private float wallJumpForceY = 400f;
	[SerializeField] private float wallJumpForceX = 400f;
	[SerializeField, Range(0, 1), Tooltip("Amount of maxSpeed applied to crouching movement. 1 = 100%")]
	private float crouchSpeed;
	[SerializeField, Range(0, .3f), Tooltip("How much to smooth out the movement")]
	private float movementSmoothing = .05f;
	[SerializeField, Tooltip("Whether a player can steer while jumping")] 
	private bool hasAirControl;
	[SerializeField, Tooltip("A mask determining what is ground to the character, must NOT contain the character layer")] 
	private LayerMask groundOrWallLayers;
	[SerializeField, Tooltip("A position marking where to check if the player is grounded")] 
	private Transform groundCheck;
	[SerializeField, Tooltip("A position marking where to check for ceilings")] 
	private Transform ceilingCheck;
	[SerializeField, Tooltip("A position marking where to check for a facing wall, should be on the right of the collider")] 
	private Transform facingWallCheck;
	[SerializeField, Tooltip("A position marking where to check for a wall behind, should be on the left of the collider")] 
	private Transform behindWallCheck;
	
	[SerializeField, Tooltip("A collider that will be disabled when crouching")] 
	private Collider2D crouchDisableCollider;
	
	internal EventHandler<bool> OnCrouch;
	internal EventHandler OnTouchWall;
	internal EventHandler OnLeaveWall;
	internal EventHandler OnLand;
	internal EventHandler OnFall;

	private Rigidbody2D rigidBody;
	private bool isFacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;
	private bool isGrounded;            // Whether the player is grounded.
	private bool isOnWall;            // Whether the player is on a wall.

	private bool isFalling;
	
	private bool wasCrouching;

	private void Awake()
	{
		rigidBody = GetComponent<Rigidbody2D>();
		
		if(Functions.IsInLayerMask(gameObject.layer, groundOrWallLayers))
			Debug.LogError($"{name}: Character Controller2D is in ground layers");
	}

	private void FixedUpdate()
	{
		bool wasFalling = isFalling;
		isFalling = rigidBody.linearVelocityY <= -Values.Epsilon;
		if(isFalling && !wasFalling)
			OnFall?.Invoke(this,EventArgs.Empty);
			
		bool wasGrounded = isGrounded;
		isGrounded = false;
		
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		// This assumes we can only be on one ground at the same time.
		Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, k_GroundedRadius, groundOrWallLayers);
		if (groundCollider is not null)
		{
			isGrounded = true;
			if (!wasGrounded)
				OnLand?.Invoke(this,EventArgs.Empty);
			// Do not check for walls if on the floor
			return;
		}
		
		bool wasOnWall = isOnWall;
		isOnWall = false;
		
		Collider2D facingWallCollider = Physics2D.OverlapCircle(facingWallCheck.position, k_GroundedRadius, groundOrWallLayers);
		if (facingWallCollider is not null)
		{
			isOnWall = true;
			if (!wasOnWall)
				OnTouchWall?.Invoke(this,EventArgs.Empty);
			return;
		}
		
		Collider2D behindWallCollider = Physics2D.OverlapCircle(behindWallCheck.position, k_GroundedRadius, groundOrWallLayers);
		if (behindWallCollider is not null)
		{
			isOnWall = true;
			// Flip before touching the wall to face the wall
			Flip();
			if (!wasOnWall)
				OnTouchWall?.Invoke(this,EventArgs.Empty);
		}
		
		// Leave wall if not in contact with a wall anymore
		if(wasOnWall)
			OnLeaveWall?.Invoke(this,EventArgs.Empty);
			
		
	}


	internal void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(ceilingCheck.position, k_CeilingRadius, groundOrWallLayers))
				crouch = true;
		}

		//only control the player if grounded or airControl is turned on
		if (isGrounded || hasAirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!wasCrouching)
				{
					wasCrouching = true;
					OnCrouch.Invoke(this,true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= crouchSpeed;

				// Disable one of the colliders when crouching
				if (crouchDisableCollider != null)
					crouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (crouchDisableCollider !=null)
					crouchDisableCollider.enabled = true;

				if (wasCrouching)
				{
					wasCrouching = false;
					OnCrouch.Invoke(this,false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, rigidBody.linearVelocity.y);
			// And then smoothing it out and applying it to the character
			rigidBody.linearVelocity = Vector3.SmoothDamp(rigidBody.linearVelocity, targetVelocity, ref velocity, movementSmoothing);
			if ((move > 0 && !isFacingRight) || (move < 0 && isFacingRight))
				Flip();
		}
		
		// Ground jump
		if (isGrounded && jump)
		{
			isGrounded = false;
			rigidBody.AddForce(new Vector2(0f, groundJumpForce));
		}
		
		// Wall jump
		if (isOnWall && jump)
		{
			isOnWall = false;
			if (rigidBody.linearVelocityY < 0)
				rigidBody.linearVelocityY =0;
			rigidBody.AddForce(new Vector2(isFacingRight?-wallJumpForceX:wallJumpForceX, wallJumpForceY));
		}
	}


	/// <summary>
	/// Flips the character.
	/// </summary>
	private void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 tempLocalScale = transform.localScale;
		tempLocalScale.x *= -1;
		transform.localScale = tempLocalScale;
	}
}
