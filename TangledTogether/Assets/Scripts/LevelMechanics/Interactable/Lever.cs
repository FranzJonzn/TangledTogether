using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public float speed = 1;
    public bool move;
    public bool active;
    public float startAngle = 315;
    public float rotation = 90;
    public List<GameObject> activationObejcts;

    // Update is called once per frame
    void Update()
    {
        MoveLever();
    }

    void MoveLever()
	{
        if (move)
        {
            if (transform.eulerAngles.x >= startAngle || transform.eulerAngles.x <= (startAngle + rotation) % 360 + 1)
            {
                Debug.Log(transform.eulerAngles.x);
                transform.Rotate(Vector3.left, speed);
            }
            if (Mathf.Ceil(transform.eulerAngles.x) == (startAngle + rotation) % 360)
            {
                active = true;
                if(activationObejcts != null)
                {
                    for(int i = 0; i < activationObejcts.Count; i++)
                    {
                        activationObejcts[i].GetComponent<Activator>().activate = true;
                    }
                }
                move = false;
            }
        }
    }
}
