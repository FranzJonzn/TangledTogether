using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
public class work_PlayerRigedBody : MonoBehaviour
{
    [SerializeField]
    private ObiRope mRope;


    public Rigidbody player_1;

    public bool test;
    [Space]
    public float restL;
    public float currentL;
    public float strain;
    public void Start()
    {
        restL = mRope.restLength;




    }
    public void Update()
    {
        currentL = mRope.CalculateLength();
        strain = restL / currentL;

        if(strain < 0.5f)
        {

        }


       // StrechingScaleCurrent = StrechingScaleOrigin*strain;

    }
}
