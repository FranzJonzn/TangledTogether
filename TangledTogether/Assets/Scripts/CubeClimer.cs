using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class CubeClimer : MonoBehaviour
{
    public ObiActor rope;

    public int index = 0;
    public int lenght = 0;
    public int index2 = 0;

    public void Update()
    {


        if (Input.GetKeyDown(KeyCode.Space))
            index = (index + 1 < rope.solverIndices.Length) ? index + 1 : 0;


        int i = rope.solverIndices[index];
       transform.position = rope.solver.positions[i];
    }


	void OnDrawGizmos()
	{

		if (rope == null || !rope.isLoaded)
			return;

		Gizmos.color = Color.red;
		Gizmos.matrix = rope.solver.transform.localToWorldMatrix;
		lenght = rope.solverIndices.Length;
		for (int i = 0; i < rope.solverIndices.Length; ++i)
		{
			int solverIndex = rope.solverIndices[i];
			Gizmos.DrawRay(rope.solver.positions[solverIndex],Vector3.right*5f);
			Gizmos.DrawRay(rope.solver.positions[solverIndex],-Vector3.right*5f);
		}

		Gizmos.color = Color.green;
		if (index2 < rope.solverIndices.Length)
			index2++;
		else
			index2 = 0;
		int solverIndex2 = rope.solverIndices[index2];
		Gizmos.DrawRay(rope.solver.positions[solverIndex2], Vector3.right * 5f);
		Gizmos.DrawRay(rope.solver.positions[solverIndex2], -Vector3.right * 5f);


	}

}
