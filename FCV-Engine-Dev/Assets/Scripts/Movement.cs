using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Component that controls the movement of an object with a RigidBody2D
/// </summary>
public class Movement : MonoBehaviour
{
	public static Transform PlayerTransform { get; private set; } 
	
	#region Run
	[Header("Run Settings:")]
	[Space(10)]
	
	[Tooltip("The players maximum movement speed.")]
	[SerializeField] public float maxSpeed;
	[Tooltip("Dictates the rate of which the players forward momentum increases until reaching maximum run speed. Increase/decrease it to make the player reach the maximum speed faster/slower.")]
	[SerializeField] public float runAccelRate;
	[Tooltip("Dictates the rate of which the players forward momentum decreses until a standstill. Increase/decrease it to make the player reach the maximum speed faster/slower.")]
	[SerializeField] public float runDecelRate;
	[Tooltip("Dictates the rate of which the players forward momentum increases in the air.")]
	[SerializeField] public float runAccelRateInAir;
	[Tooltip("Dictates the rate of which the players forward momentum decreses in the air.")]
	[SerializeField] public float runDecelRateInAir;
	#endregion
	[Space(20)]

	#region Jump
	[Header("Jump Settings:")]
	[Space(10)]
	
	[Tooltip("The amount of force the player jumps with.")]
	[SerializeField] private float jumpForce;
	[Tooltip("Extra time that allows players to jump briefly after leaving a platform, improving the feeling of responsiveness.")]
	[SerializeField] private float jumpCoyoteTime;
	[Tooltip("Limits the players ability to jump only once inside of the time frame. Ensures that the player can't jump multiple times in an instance for unintended increased jumphight.")]
	[SerializeField] private float jumpBufferTime;
	[Tooltip("The amount of extra jumps the player can use before landing, for example: set to 1 for a dubblejump. Set to 0 to not use.")]
	[SerializeField] private int extraJumps; // set to 0 if no extra jumps are to be used
	#endregion
	[Space(20)]
	
	#region Wall Jump/Slide
	[Header("Wall Jump/Slide Settings:")]
	[Space(10)]

	[Tooltip("Enable/Disable the players ability to wall slide and jump.")]
	[SerializeField] private bool enableWallSlideAndJump;
	[Tooltip("The distance/radius from the player that the wall checks in both diractions. Note that the distance must exceed the players collider to function properly.")]
	[SerializeField] private float wallCheckDistance;
	[Tooltip("Dictates the deceleration rate when the player slides on the wall.")]
	[SerializeField] private float wallSlideDecelRate;
	[Tooltip("The max duration for the wall slide before the player starts to fall like normal and can no longer wall-jump.")]
	[SerializeField] private float wallSlideDurationSec;
	[Tooltip("The amount of force the player wall jumps with. X for horizontal force. Y for vertical force.")]
	[SerializeField] private Vector2 wallJumpForce;
	[Tooltip("Set maximum amount of wall jumps the player can use after each other Set to 0 to allow infinite wall jumps")]
	[SerializeField] private int finiteWallJumps; // set to 0 if unlimited.
	#endregion
	[Space(20)]

	#region Gravity scale
	[Header("Gravity Settings:")]
	[Space(10)]

	[Tooltip("The amount of gravity that is applied to the player.")]
	[SerializeField] public float gravityScale;
	[Tooltip("The amount of gravity that is applied when the player is falling. Used to enhance the sense of gravity for the player while not affecting the players ability to jump.")]
	[SerializeField] public float fastFallingGravityMult;
	#endregion
	[Space(20)]
	
	#region Inputs
	[Header("Input Settings:")]
	[Space(10)]
	[SerializeField] private KeyCode jumpButton;
	#endregion
	[Space(20)]
	
	#region Other Settings
	[Header("Other Settings:")]
	[Space(10)]
	[Tooltip("The layers that is counted as ground and walls. Used for the normal jump and the wall-slid/jump.")]
	[FormerlySerializedAs("ground")] [SerializeField] LayerMask groundLayer;
	[Tooltip("The dimentions that is used to checked for ground. By default, checks around the players feet. Note that the distance must exceed the players collider to function properly.")]
	[FormerlySerializedAs("overlapBox")] [SerializeField] private Vector2 overlapBoxSize;
	[Tooltip("The offset from the middle of the player character. Used to get the center point for the above check.")]
	[SerializeField] private Vector2 overlapBoxOffset;
	[Tooltip("The offset from the middle of the player character. Used to get the center point for the above check.")]
	#endregion
	
	private string horizontalAxisName = "Horizontal";
	private Vector2 moveInputs = new Vector2();
	private float lastJumpTimestamp;
	private bool isGrounded;
	private bool isJumping;
	private bool jumpPressed;
	private int jumpsAmount;
	private int walljumpsAmount;
	private int lastWallJumpDirection;
	private bool isTouchingWall;
	private bool isWallSliding;
	private float timeElapsedSinceWallSlide;


	private float lastGroundedTime;
	private Rigidbody2D rb;

	private Animator animator;
	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		PlayerTransform = transform;
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		rb.gravityScale = gravityScale;
	}

	private void Update()
	{
		lastGroundedTime -= Time.deltaTime;

		CheckInputs();

		isGrounded = Physics2D.OverlapBox((Vector2)transform.position + overlapBoxOffset, overlapBoxSize, 0, groundLayer);
		if (isGrounded) 
		{
			// resets jump values
			lastGroundedTime = jumpCoyoteTime;
			isJumping = false;
			jumpsAmount = 0;
			
			// resets wall slide values:
			timeElapsedSinceWallSlide = 0;
			lastWallJumpDirection = 0;
			walljumpsAmount = 0;
		}
		
		if (enableWallSlideAndJump)
		{
			isTouchingWall = Physics2D.OverlapBox((Vector2)transform.position, new Vector2(wallCheckDistance * 2, 0.1f), 0, groundLayer);
			
			if (CanWallJump() && Input.GetKeyDown(jumpButton))
				WallJump();
		}

		SetAnimation();
		Flip();
		GravityScaling();
	}
	/// <summary>
	/// Checks for all inputs and assigns values based on the pressed key
	/// </summary>
	private void CheckInputs()
	{
		moveInputs.x = 0;
		moveInputs.x = Input.GetAxis(horizontalAxisName);

		if (Input.GetKey(KeyCode.LeftArrow))
			moveInputs.x = -1;
		if (Input.GetKey(KeyCode.RightArrow))
			moveInputs.x = 1;
	}

	private void FixedUpdate()
	{
		Run();
		
		if (enableWallSlideAndJump)
			WallSliding();
		
		if (CanJump() && Input.GetKey(jumpButton))
			Jump();
	}
	
	/// <summary>
	/// Calculates the targeted speed and applies it over time using rigidbody2D
	/// </summary>
	private void Run()
	{
		float targetSpeed = moveInputs.x * maxSpeed;

		float speedDiff = targetSpeed - rb.velocity.x;

		float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelRate : runDecelRate;

		accelRate = (Mathf.Abs(targetSpeed) > 0.01f && isJumping) ? runAccelRateInAir : runDecelRateInAir;

		float movement = speedDiff * accelRate;

		rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}
	
	/// <summary>
	/// Applies a force upward to simulate a jump
	/// </summary>
	private void Jump()
	{
		float force = jumpForce;
		rb.velocity = new Vector2(rb.velocity.x, 0);

		rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

		lastGroundedTime = 0;
		isJumping = true;
		animator.SetTrigger("StartJump");
		lastJumpTimestamp = Time.time;
		jumpsAmount++;
	}
	
	/// <summary>
	/// Applies a slowdown the the players downwards velocity for a period of time.
	/// </summary>
	private void WallSliding()
	{
		if (isTouchingWall && !isGrounded && timeElapsedSinceWallSlide <= wallSlideDurationSec)
		{
			isWallSliding = true;
			timeElapsedSinceWallSlide += Time.fixedDeltaTime;
			isJumping = false;
			
			if (rb.velocity.y <= -1f)
			{
				float verticalVelocity = rb.velocity.y;
				verticalVelocity += wallSlideDecelRate * wallSlideDurationSec-timeElapsedSinceWallSlide;
				rb.velocity = new Vector2(rb.velocity.x, verticalVelocity);
			}
		}
		else
		{
			isWallSliding = false;
		}
	}
	
	/// <summary>
	/// Applies a force upward and in the opposite direction from the nearest wall to simulate a wall jump. Also checks and prevents the player from jumping upp the same wall.
	/// </summary>
	private void WallJump()
	{
		if (isTouchingWall && !isGrounded)
		{
			// Raycast to find out in which diraction the wall is in:
			int direction = Physics2D.Raycast((Vector2)transform.position, Vector2.right, wallCheckDistance, groundLayer) ? -1 : 1;
			
			// If the player have already jumped from that direction -> then don't jump. else: remember the current direction to check for during next jump:
			// This is to prevent the player from jumping upp the same wall. Also prevents them from "stacking" jumps by spam-klicking the jump-button.
			if (direction == lastWallJumpDirection) return;
			else lastWallJumpDirection = direction;
			
			// Flip the players sprite depending on the direction:
			if (direction > 0.01f) spriteRenderer.flipX = true;
			else spriteRenderer.flipX = false;
			
			Vector2 force = wallJumpForce;
			
			rb.velocity = new Vector2(rb.velocity.x, 0);
			rb.AddForce(new Vector2(force.x * direction, force.y), ForceMode2D.Impulse);

			lastGroundedTime = 0;
			timeElapsedSinceWallSlide = 0;
			isJumping = true;
			animator.SetTrigger("StartJump");
			walljumpsAmount++; 
		}
	}
	

	/// <summary>
	/// Scales gravity so that the player has higher gravity when falling compared to jumping
	/// </summary>
	private void GravityScaling()
	{
		if (rb.velocity.y < -.3f && !isWallSliding)
		{
			rb.gravityScale = fastFallingGravityMult * gravityScale;
		}
		else
		{
			rb.gravityScale = gravityScale;
		}
	}
	
	/// <summary>
	/// Sets the animations of the player 
	/// </summary>
	private void SetAnimation()
	{
		animator.SetFloat("speed", Mathf.Abs(moveInputs.x));
		animator.SetBool("IsJumping", isJumping);
	}
	
	/// <summary>
	/// Checks if all conditions for player jump is met
	/// </summary>
	/// <returns>returns true if all requirements are met</returns>
	private bool CanJump()
	{
		bool bufferTime = Time.time - lastJumpTimestamp >= jumpBufferTime;
		
		if (extraJumps > 0)
		{
			return bufferTime && jumpsAmount < extraJumps + 1;
		}

		return bufferTime && lastGroundedTime > 0 && !isJumping;
	}
	
	/// <summary>
	/// Checks if all conditions for the player to wall jump is met.
	/// </summary>
	/// <returns>returns true if all requirements are met.</returns>
	private bool CanWallJump()
	{
		if (finiteWallJumps > 0)
			return isWallSliding && walljumpsAmount < finiteWallJumps;
		
		return isWallSliding;
	}
	
	/// <summary>
	/// Flips sprite depending on direction of movement inputs
	/// </summary>
	private void Flip()
	{
		if (moveInputs.x > 0.01f)
		{
			spriteRenderer.flipX = true;
		}

		if (moveInputs.x < -0.01f)
		{
			spriteRenderer.flipX = false;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube((Vector2)transform.position + overlapBoxOffset, overlapBoxSize); // Ground-check
		Gizmos.DrawWireCube((Vector2)transform.position, new Vector2(wallCheckDistance*2, 0.1f)); // Wall-check
	}
}