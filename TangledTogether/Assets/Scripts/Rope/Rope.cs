using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public struct RopeSegment
	{
        public Vector3 posNow;
        public Vector3 posOld;

        public RopeSegment(Vector3 pos)
		{
            this.posNow = pos;
            this.posOld = pos;
		}
	}

    public GameObject playerOne;
    public Vector3 playerOffsetPos;
    public float ropeSegmentLength;
    public int segmentLength;
    public float lineWidth;

    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = playerOne.transform.position + playerOffsetPos;

        for(int i = 0; i < segmentLength; i++)
		{
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegmentLength;
		}
    }

    void Update()
    {
        DrawRope();
    }

	private void FixedUpdate()
	{
        Simulate();
	}

	void Simulate()
	{
        Vector3 forceGravity = new Vector3(0f, -1f, 0f);

        for(int i = 0; i < segmentLength; i++)
		{
            RopeSegment firstSegment = ropeSegments[i];
            Vector3 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.deltaTime;
            ropeSegments[i] = firstSegment;
		}

        for(int i = 0; i < 50; i++)
        {
            ApplyConstaint();
        }
	}

    void ApplyConstaint()
	{
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = playerOne.transform.position + playerOffsetPos;
        ropeSegments[0] = firstSegment;

        for (int i = 0; i < segmentLength - 1; i++)
		{
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = Mathf.Abs(dist - ropeSegmentLength);
            Vector3 changeDir = Vector3.zero;

            if (dist > ropeSegmentLength)
                changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            else if (dist < ropeSegmentLength)
                changeDir = (secondSeg.posNow - firstSeg.posNow).normalized;

            Vector3 changeAmount = changeDir * error;
            if(i != 0)
			{
                firstSeg.posNow -= changeAmount * 0.5f;
                ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.5f;
			}
			else
                secondSeg.posNow += changeAmount;
            ropeSegments[i + 1] = secondSeg;
        }
    }

    void DrawRope()
	{
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segmentLength];
        for(int i = 0; i < segmentLength; i++)
		{
            ropePositions[i] = ropeSegments[i].posNow;
		}

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
	}
}
