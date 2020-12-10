using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverCollision : Lever
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			Debug.Log("pull down");
			if (collision.gameObject.GetComponent<PlayerInputHolder>().playerInput.isGrabbing)
			{
				base.move = true;
			}
		}
	}
}
