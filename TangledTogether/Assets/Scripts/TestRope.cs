using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiActor))]
public class TestRope : MonoBehaviour
{



    public Transform character_1;
    public Transform character_2;




    public ObiSolver solver;
    public Material material;
    public ObiRopeSection section;


    public Transform rope;

	ObiActor actor;

	void Awake()
	{
		actor = GetComponent<ObiActor>();
	}

	void OnDrawGizmos()
	{

		if (actor == null || !actor.isLoaded)
			return;

		Gizmos.color = Color.red;
		Gizmos.matrix = actor.solver.transform.localToWorldMatrix;

		for (int i = 0; i < actor.solverIndices.Length; ++i)
		{
			int solverIndex = actor.solverIndices[i];
			Gizmos.DrawRay(actor.solver.positions[solverIndex],
					   actor.solver.velocities[solverIndex] * Time.fixedDeltaTime);
		}
	}



}
