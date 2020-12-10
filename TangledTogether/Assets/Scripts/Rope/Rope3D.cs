using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope3D : MonoBehaviour
{
    [SerializeField]
    GameObject ropePart, startPosition;
    public GameObject playerOne;
    public GameObject playerTwo;

    [SerializeField]
    [Range(1, 1000)]
    int lenght = 1;

    [SerializeField]
    float partDistance = 0.21f;

    [SerializeField]
    bool reset, spawn, snapFirst, snapLast;

    // Update is called once per frame
    void Update()
    {
        if (reset)
        {
            foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("Rope"))
            {
                Destroy(tmp);
            }

            reset = false;
        }

        if (spawn)
        {
            Spawn();

            spawn = false;
        }
    }


    public void Spawn()
    {
        int count = (int)(lenght / partDistance);

        GameObject temp = null;

        for (int i = 0; i < count; i++)
        {

            temp = Instantiate(ropePart, new Vector3(startPosition.transform.position.x, startPosition.transform.position.y, startPosition.transform.position.z - partDistance * (i + 1)), 
                Quaternion.identity, startPosition.transform);
            temp.transform.eulerAngles = new Vector3(90, 0, 0);

            temp.name = startPosition.transform.childCount.ToString();

            if (i == 0)
            {
                //Destroy(temp.GetComponent<CharacterJoint>());
                //if (snapFirst)
                //{
                //    temp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                //}
                temp.GetComponent<CharacterJoint>().connectedBody = playerOne.GetComponent<Rigidbody>();
                //playerOne.GetComponent<CharacterJoint>().connectedBody = temp.GetComponent<Rigidbody>();
            }
            else
            {
                temp.GetComponent<CharacterJoint>().connectedBody = startPosition.transform.Find((startPosition.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }

            if (snapLast)
            {
                startPosition.transform.Find((startPosition.transform.childCount).ToString()).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        playerTwo.GetComponent<CharacterJoint>().connectedBody = temp.GetComponent<Rigidbody>();
    }
}
