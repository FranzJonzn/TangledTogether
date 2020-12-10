using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pullable : MonoBehaviour
{
    [Tooltip ("Only use one direction, the rest should be 0")]
    public Vector3 offsetPos;
    private Vector3 startPos;
    private Vector3 nextPos;
    private Rigidbody rb;
    private GameObject player;

    private float gameObjectPosFloat;
    private float startPosFloat;
    private float nextPosFloat;
    private float playerDirVel;

    void Awake()
    {
        startPos = transform.position;
        nextPos = startPos + offsetPos;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void Update()
    {
        MoveBetweeenPoints();
    }

    void MoveBetweeenPoints()
    {
        if (gameObject.GetComponent<FixedJoint>() != null)
        {
            player = gameObject.GetComponent<FixedJoint>().connectedBody.gameObject;
            Debug.Log("pulling");
            player.GetComponent<PlayerInputHolder>().playerInput.disableRotation = true;
            Debug.Log(Vector3.Angle(transform.position, player.transform.position));

            UpdateValues();

            if (gameObjectPosFloat >= nextPosFloat && gameObjectPosFloat <= startPosFloat ||
                gameObjectPosFloat <= startPosFloat && playerDirVel > 0 ||
                gameObjectPosFloat >= nextPosFloat && playerDirVel < 0)
            {
                if (offsetPos.x != 0)
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                else if (offsetPos.y != 0)
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                else if (offsetPos.z != 0)
                    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
            }
            else
                rb.constraints = RigidbodyConstraints.FreezeAll;

            Debug.Log("transform.pos.z: " + transform.position.z);
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            if(player != null)
                player.GetComponent<PlayerInputHolder>().playerInput.disableRotation = false;
        }
    }

    void UpdateValues()
    {
        if (offsetPos.x != 0)
        {
            gameObjectPosFloat = transform.position.x;
            startPosFloat = startPos.x;
            nextPosFloat = nextPos.x;
            playerDirVel = player.GetComponent<Rigidbody>().velocity.x;
        }
        else if (offsetPos.y != 0)
        {
            gameObjectPosFloat = transform.position.y;
            startPosFloat = startPos.y;
            nextPosFloat = nextPos.y;
            playerDirVel = player.GetComponent<Rigidbody>().velocity.y;
        }
        else if (offsetPos.z != 0)
        {
            gameObjectPosFloat = transform.position.z;
            startPosFloat = startPos.z;
            nextPosFloat = nextPos.z;
            playerDirVel = player.GetComponent<Rigidbody>().velocity.z;
        }
    }
}
