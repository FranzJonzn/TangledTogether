using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    private PlayerInput playerInput;
    public float grabRange;
	[Range(0, 360)]
	public float viewAngle = 150;

	private Rigidbody rb;
    private FixedJoint fixedJoint;
	private RaycastHit hit;
	private Ray castpoint;
	private bool grabbedObject;
	private LayerMask grabable;
	private GameObject closestObject;
	private bool specialCase;

	private void Awake()
	{
		playerInput = gameObject.GetComponent<PlayerInputHolder>().playerInput;
		rb = gameObject.GetComponent<Rigidbody>();
		grabable = ~LayerMask.GetMask("Ground");
	}

	// Update is called once per frame
	void Update()
    {
        GrabObject();
		if (specialCase)
		{
			if(closestObject.GetComponent<Lever>().active == true)
			{
				specialCase = false;
				if (!Input.GetKey(playerInput.grab))
				{
					Release();
				}
			}
		}
    }

    void GrabObject()
	{
		if (Input.GetKeyDown(playerInput.grab))
		{
			if (!grabbedObject)
			{
				Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, grabRange, grabable);
				for (int i = 0; i < targetsInViewRadius.Length; i++)
				{
					if(targetsInViewRadius[i].gameObject.layer != gameObject.layer)
					{
						GameObject target = targetsInViewRadius[i].gameObject;
						Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
						if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
						{
							if (closestObject == null)
							{
								closestObject = target;
							}
							else
							{
								float distToTarget = Vector3.Distance(transform.position, target.transform.position);
								float distToTemp = Vector3.Distance(transform.position, closestObject.transform.position);
								if (distToTemp < distToTarget)
								{
									closestObject = target;
								}
							}
						}
					}
				}
				if(closestObject != null)
				{
					fixedJoint = closestObject.AddComponent(typeof(FixedJoint)) as FixedJoint;
					fixedJoint.connectedBody = rb;
					if (closestObject.GetComponent<Rigidbody>().isKinematic)
					{
						//playerInput.disableMovement = true;
						//playerInput.disableRotation = true;
					}
					Debug.Log(closestObject.name);
					grabbedObject = true;
					playerInput.isGrabbing = true;
					SpecialCases();
				}
			}
		}
		else if (Input.GetKeyUp(playerInput.grab) && !specialCase)
		{
			Release();
		}
	}

	void SpecialCases()
	{
		if(closestObject.tag == "Lever")
		{
			closestObject.GetComponent<Lever>().move = true;
			specialCase = true;
		}
	}

	void Release()
	{
		if (closestObject != null && closestObject.GetComponent<Rigidbody>().isKinematic)
		{
			//playerInput.disableMovement = false;
			//playerInput.disableRotation = false;
		}
		Destroy(fixedJoint);
		grabbedObject = false;
		playerInput.isGrabbing = false;
		closestObject = null;
	}
}
