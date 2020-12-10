using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPointToPoint : MonoBehaviour
{
    public List<Vector3> checkPoints;
    public float speed;
    private Vector3 startPos;
    private Vector3 nextPos;
    private int posNumber = 0;
    private bool maxPos;
    private Activator activator;
    private bool activated;

    void Awake()
    {
        activator = gameObject.GetComponent<Activator>();
        startPos = transform.position;
        nextPos = startPos + checkPoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (activator.activate)
            activated = true;

		if (activated)
		{
            gameObject.transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
            if(checkPoints.Count > 1)
            {
                if (transform.position == nextPos)
                {
                    activated = false;
                    if (!maxPos)
                    {
                        posNumber++;
                        nextPos = startPos + checkPoints[posNumber];
                        if (posNumber == checkPoints.Count - 1)
                            maxPos = true;
                    }
                    else if (maxPos)
                    {
                        if (transform.position == startPos + checkPoints[checkPoints.Count - 1])
                            nextPos = startPos;
                        else if (transform.position == startPos)
                            nextPos = startPos + checkPoints[checkPoints.Count - 1];
                    }
                }
            }
            else
            {
                if (transform.position == startPos)
                {
                    activated = false;
                    nextPos = startPos + checkPoints[0];
                }
                else if (transform.position == startPos + checkPoints[0])
                {
                    activated = false;
                    nextPos = startPos;
                }
            }
        }
    }
}
