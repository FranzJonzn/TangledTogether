using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yeet : MonoBehaviour
{
	private PlayerInput playerInput;
	private GameObject otherPlayer;

	public float viewRadius = 1.1f;
	[Range(0, 360)]
	public float viewAngle = 150;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[Header("Throw")]
	public float h = 25;
	public float gravity = -18;
	public float angle = 1;
	private Vector3 launchDirection;
	public float throwForce;

	private void Awake()
	{
		playerInput = gameObject.GetComponent<PlayerInputHolder>().playerInput;
	}

	void Update()
	{
		YeetOtherPlayer();
	}

	void YeetOtherPlayer()
	{
		//TODO: TP PLAYER TO A GIVEN POINT IN FRONT OF THE THROWING PLAYER TO MAKE THE THROW MORE CONSITENT
		if (Input.GetKey(playerInput.yeet) && !playerInput.disableThrow)
		{
			Debug.Log("Prepare to yeet");
			playerInput.disableMovement = true;
			CheckOtherPlayerPos();

			Debug.DrawRay(gameObject.transform.position, GetDirection(), Color.cyan);
		}
		else if (Input.GetKeyUp(playerInput.yeet))
		{
			Debug.Log("Yeet denied");
			playerInput.disableMovement = false;
		}
	}

	void CheckOtherPlayerPos()
	{
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
		if(targetsInViewRadius.Length != 0)
		{
			otherPlayer = targetsInViewRadius[0].gameObject;
			Transform target = otherPlayer.transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);

				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
				{
					Debug.Log("Y E E T");
					Launch();
					playerInput.disableMovement = false;
				}
			}
		}
	}

	void Launch()
	{
		launchDirection = GetDirection();
		otherPlayer.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
		otherPlayer.GetComponent<Rigidbody>().AddForce(launchDirection * throwForce, ForceMode.VelocityChange);
		playerInput.disableThrow = true;
		otherPlayer.GetComponent<PlayerInputHolder>().playerInput.disableThrow = true;
		StartCoroutine(WaitForThrow());
	}

	Vector3 GetDirection()
	{
		Vector3 direction = -gameObject.transform.forward + Vector3.up * angle;
		direction.Normalize();

		return direction;
	}

	IEnumerator WaitForThrow()
	{
		yield return new WaitForSeconds(1);
		playerInput.disableThrow = false;
		otherPlayer.GetComponent<PlayerInputHolder>().playerInput.disableThrow = false;
	}
}
