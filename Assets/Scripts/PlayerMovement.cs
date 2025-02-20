using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	private static readonly int IsJumping = Animator.StringToHash("IsJumping");
	private static readonly int IsCrouching = Animator.StringToHash("IsCrouching");
	private static readonly int Speed = Animator.StringToHash("Speed");

	private CharacterController2D controller;
	private Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;

	private void Awake()
	{
		controller = GetComponent<CharacterController2D>();
		animator = GetComponent<Animator>();

		controller.OnCrouch += OnCrouching;
		controller.OnLand += OnLanding;
	}
	
	private void Update () {

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		animator.SetFloat(Speed, Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
			animator.SetBool(IsJumping, true);
		}

		if (Input.GetButtonDown("Crouch"))
		{
			crouch = true;
		} else if (Input.GetButtonUp("Crouch"))
		{
			crouch = false;
		}

	}
	

	private void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
		jump = false;
	}

	private void OnLanding (object sender, EventArgs eventArgs)
	{
		animator.SetBool(IsJumping, false);
	}

	private void OnCrouching (object sender, bool isCrouching)
	{
		animator.SetBool(IsCrouching, isCrouching);
	}
}
