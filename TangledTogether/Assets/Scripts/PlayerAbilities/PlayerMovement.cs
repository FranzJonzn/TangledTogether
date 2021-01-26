using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
	public Rigidbody rb;
    public Transform cam;
    public Transform groundCheck;
    public LayerMask groundMask;

    public float groundDistance = 0.4f;
	[Tooltip("Should not be lower than maxMovementSpeed. Higher value -> reach max speed faster")]
    public float movementSpeed;
	public float maxMovementSpeed;
    public float jumpHeight = 3f;
    public float gravity = -9.82f;
    public float turnSmoothTime;
	public float maxSlopeAngle;
	public float stopSpeed;

	private PlayerInput playerInput;
	private RaycastHit hit;
	private Vector3 direction;
	private Ray castpoint;
	private float turnSmoothVelocity;
	private float slopeAngle;
    private bool isGrounded;

	private void Awake()
	{
		playerInput = gameObject.GetComponent<PlayerInputHolder>().playerInput;
	}

	private void Update()
	{
		MoveInput();
		GroundCheck();
	}

	private void FixedUpdate()
	{
		Move();
		ReverseMoveForce();
		Jump();
		Swing();
	}

	void MoveInput()
	{
		direction = Vector3.zero;
		if (Input.GetKey(playerInput.left))
			direction += Vector3.left;
		if (Input.GetKey(playerInput.right))
			direction += Vector3.right;
		if (Input.GetKey(playerInput.up))
			direction += Vector3.forward;
		if (Input.GetKey(playerInput.down))
			direction += Vector3.back;
		direction.Normalize();
	}

	void Move()
	{
		if (direction.magnitude > 0.1f && isGrounded)
		{
			if(!playerInput.disableRotation)
				Rotate(direction);
			if(!playerInput.disableMovement)
			rb.AddForce(direction * movementSpeed * Time.deltaTime, ForceMode.VelocityChange);
		}
		Debug.DrawLine(transform.position, transform.position + direction, Color.yellow);
	}

	void ReverseMoveForce()
	{
		//Stop movement when no key is pressed
		if (direction.magnitude <= 0.1f && isGrounded)
		{
			if (rb.velocity.x > 0.1f || rb.velocity.x < -0.1f)
				rb.AddForce(Vector3.left * stopSpeed * rb.velocity.x * Time.deltaTime, ForceMode.VelocityChange);
			if (rb.velocity.z > 0.1f || rb.velocity.z < -0.1f)
				rb.AddForce(Vector3.back * stopSpeed * rb.velocity.z * Time.deltaTime, ForceMode.VelocityChange);
		}

		//Avoid overshooting 
		if (rb.velocity.x > maxMovementSpeed)
		{
			rb.AddForce(Vector3.left * (rb.velocity.x + movementSpeed) * Time.deltaTime, ForceMode.VelocityChange);
		}
		if (rb.velocity.x < -maxMovementSpeed)
		{
			rb.AddForce(Vector3.left * (rb.velocity.x - movementSpeed) * Time.deltaTime, ForceMode.VelocityChange);
		}
		if (rb.velocity.z > maxMovementSpeed)
		{
			rb.AddForce(Vector3.back * (rb.velocity.z + movementSpeed) * Time.deltaTime, ForceMode.VelocityChange);
		}
		if (rb.velocity.z < -maxMovementSpeed)
		{
			rb.AddForce(Vector3.back * (rb.velocity.z - movementSpeed) * Time.deltaTime, ForceMode.VelocityChange);
		}
		if(rb.velocity.y > 0)
		{
			rb.AddForce(Vector3.down * (rb.velocity.y + jumpHeight) * Time.deltaTime, ForceMode.VelocityChange);
		}
	}

	void Jump()
	{
		if(isGrounded && Input.GetKeyDown(playerInput.jump) && !playerInput.disableMovement)
		{
			rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
		}
		else if(!isGrounded)
		{
			rb.AddForce(gravity * Vector3.up);
		}
	}

	void GroundCheck()
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		castpoint = new Ray(transform.position, Vector3.down);
		if (Physics.Raycast(castpoint, out hit, 1.5f, groundMask))
		{
			direction = Vector3.ProjectOnPlane(direction, hit.normal);
		}
		direction.Normalize();
		if (!isGrounded)
		{
			slopeAngle = 90;
		}
		else if(hit.normal != Vector3.up)
		{
			slopeAngle = Vector3.Angle(Vector3.back, direction);
		}
	}

	void Rotate(Vector3 direction)
	{
		float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
		transform.rotation = Quaternion.Euler(0f, angle, 0f);
		//rb.AddTorque(new Vector3(0f, angle, 0f), ForceMode.VelocityChange);
	}

	// TODO: avgöra när man hänger i repet till skillnad från att vara i luften för att man hoppar tex
	void Swing()
    {
		if (direction.magnitude > 0.1f && !isGrounded)
		{
			if (!playerInput.disableRotation)
				Rotate(direction);
			if (!playerInput.disableMovement)
				rb.AddForce(direction * movementSpeed * Time.deltaTime, ForceMode.Impulse);
		}
    }
}
