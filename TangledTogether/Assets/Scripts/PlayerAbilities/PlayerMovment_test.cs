using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
public class PlayerMovment_test : MonoBehaviour
{
    public Rigidbody     player_1;
    private PlayerInput  player_1_Input;
    public Transform     player_1_groundCheck;
    private Vector3      player_1_direction;
    public bool         player_1_isGrounded;
    private bool         player_1_jump;
    private bool         player_1_jumped;
    public bool         player_1_climing;
    public bool         player_1_grabRope;
    private bool         player_1_sand_ground;
    public int          player_1_ropeIndex;
    private float        player_1_startMass;
    [Space]
    public Rigidbody     player_2;
    private PlayerInput  player_2_Input;
    public Transform     player_2_groundCheck;
    private Vector3      player_2_direction;
    private bool         player_2_isGrounded;
    private bool         player_2_jump;
    private bool         player_2_jumped;
    private bool         player_2_climing;
    private bool         player_2_grabRope;
    private bool         player_2_sand_ground;
    private int          player_2_ropeIndex;
    private float        player_2_startMass;
    [Space]
    [Space]
    public LayerMask groundMask;
    public float     groundDistance = 0.4f;
    [Space]
    [Space]
    public float maxSpeed       = 7f;
    public float friction       = 15f;
    [Space]
    [Space]
    public Vector3 jumpVelocity = new Vector3(0f, 1f, 0f);
    [Space]
    [Space]
    public float swingPushVelocity = 2.5f;
    [Space]
    [Space]
    public float anchorWeight = 100f;
    public float aireWeight   = 1f;
    [Space]
    [Space]
    public  float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    [Space]
    [Space]
    public float maxDistanBetweenPlayers = 15f;
    [Space]
    [Space]
    public ObiActor rope;
    public float climeSpeed = 2.5f;
    
    void Start()
    {
        player_1_Input = player_1.gameObject.GetComponent<PlayerInputHolder>().playerInput;
        player_2_Input = player_2.gameObject.GetComponent<PlayerInputHolder>().playerInput;

        player_2_startMass = player_2.mass;
        player_1_startMass = player_1.mass;


    }


    void Update()
    {

        player_1_ropeIndex = getClosesParticle(player_1);

         player_1_sand_ground = Input.GetKey(player_1_Input.anchor);
         player_2_sand_ground = Input.GetKey(player_2_Input.anchor);

        player_1.mass = (player_1_isGrounded)                 ? player_1_startMass : aireWeight;
        player_1.mass = (player_1_sand_ground)                ? anchorWeight       : player_1.mass;
      

        player_2.mass = (player_2_isGrounded)                 ? player_2_startMass : aireWeight;
        player_2.mass = (player_2_sand_ground)                ? anchorWeight       : player_2.mass;


        if (!player_2_sand_ground)
        {
            MoveInput(player_2_Input, ref player_2_direction, ref player_2_jump, player_2_isGrounded, ref player_2_grabRope);
            GroundCheck(out player_2_isGrounded, ref player_2_direction, player_2_groundCheck);
        }
        if (!player_1_sand_ground)
        {
            MoveInput(player_1_Input, ref player_1_direction, ref player_1_jump, player_1_isGrounded, ref player_1_grabRope);
            GroundCheck(out player_1_isGrounded, ref player_1_direction, player_1_groundCheck);
        }
    }


    private void FixedUpdate()
    {
        if (!player_2_sand_ground)
            Move(    player_2, 
                     player_2_direction, 
                     player_2_isGrounded, 
                     player_2_jump, 
                 ref player_2_jumped, 
                     player_2_grabRope,
                 ref player_2_climing,
                 ref player_2_ropeIndex);
        if (!player_1_sand_ground)
            Move(    player_1, 
                     player_1_direction,
                     player_1_isGrounded, 
                     player_1_jump, 
                 ref player_1_jumped, 
                     player_1_grabRope, 
                 ref player_1_climing,
                 ref player_1_ropeIndex);


    }
    private float slopeAngle;

    void MoveInput(PlayerInput playerInput, ref Vector3 direction, ref bool jump, bool isGrounded, ref bool dragRope)
    {

        direction = Vector3.zero;
        if (isGrounded)
        {
            if (Input.GetKey(playerInput.left))
                direction += Vector3.left;
            if (Input.GetKey(playerInput.right))
                direction += Vector3.right;
            if (Input.GetKey(playerInput.up))
                direction += Vector3.forward;
            if (Input.GetKey(playerInput.down))
                direction += Vector3.back;

 
        }
        else
        {
            if (Input.GetKeyDown(playerInput.left))
                direction += Vector3.left;
            if (Input.GetKeyDown(playerInput.right))
                direction += Vector3.right;
            if (Input.GetKeyDown(playerInput.up))
                direction += Vector3.forward;
            if (Input.GetKeyDown(playerInput.down))
                direction += Vector3.back;

        
        }



        direction.Normalize();
        jump     = Input.GetKeyDown(playerInput.jump);
        dragRope = Input.GetKey(playerInput.grab);



    }



    void GroundCheck(out bool isGrounded, ref Vector3 direction, Transform groundCheck)
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Ray castpoint = new Ray(groundCheck.position, Vector3.down);
        RaycastHit hit;



        if (Physics.Raycast(castpoint, out hit, 1.5f, groundMask))
        {
            direction = Vector3.ProjectOnPlane(direction, hit.normal);     
            direction.Normalize();
        }
        //Debug.DrawLine(groundCheck.position, groundCheck.position + direction * 4f, Color.green);

        if (!isGrounded)
        {
            slopeAngle = 90;
        }
        else if (hit.normal != Vector3.up)
        {
            slopeAngle = Vector3.Angle(Vector3.back, direction);
        }
    }

    void Move(Rigidbody rb, Vector3 direction, bool isGrounded, bool jump, ref bool jumped, bool grabRope, ref bool climing, ref int ropeIndex)
    {
        Vector3 vel = rb.velocity;

        if (isGrounded)
        {
           


            if (direction.magnitude > 0.1f)
            {

                Rotate(direction, rb);

                float acceleration = maxSpeed * friction;
                vel.x -= friction     * Time.deltaTime * vel.x;
                vel.x += acceleration * Time.deltaTime * direction.x;
                vel.y -= friction     * Time.deltaTime * vel.y;
                vel.y += acceleration * Time.deltaTime * direction.y;
                vel.z -= friction     * Time.deltaTime * vel.z;
                vel.z += acceleration * Time.deltaTime * direction.z;
            }


            Climing(rb, grabRope, ref climing, ref ropeIndex);


            if (!jumped && jump)
            {
                vel += jumpVelocity;
                jumped = true;
            }
            else if (isGrounded)
                jumped = false;
        }
        else
        {
            if (direction.magnitude > 0.1f)
            {
                Rotate(direction, rb);

                vel.x += swingPushVelocity * direction.x;      
                vel.z += swingPushVelocity * direction.z;

   

            }


            if (climing && jump)
            {
                vel += jumpVelocity;

                climing = false;
            }
        }


        rb.velocity = vel;


    }
    public bool temp = false;

    private void Climing(Rigidbody rb, bool grabRope, ref bool climing,ref int ropeIndex)
    {
        //if (!climing)
        //{
        //    //gets closest rope partikle
        //    if (grabRope)
        //    {
        //        ropeIndex = getClosesParticle(rb);
        //        climing = true;

        //    }

        //}
        //else
        //{
        //    //dragsRope to you
        //    if ((grabRope))
        //        //  ropeIndex = (ropeIndex + 1 < rope.activeParticleCount) ? ropeIndex + 1 : 0;
        //        ropeIndex = (ropeIndex - 1 != -1) ? ropeIndex - 1 : 0;
        //    int i = rope.solverIndices[ropeIndex];
        //    rb.transform.position = rope.GetParticlePosition(ropeIndex);
        //}



        IEnumerator drag =  dragRope(rb, grabRope);
        if (grabRope)
        {


            StartCoroutine(drag);


        }
    

    }

    private IEnumerator dragRope(Rigidbody rb, bool grabRope)
    {
       int ropeIndex = getClosesParticle(rb);

        bool direction = (ropeIndex < (int)rope.activeParticleCount * 0.5f);
        int dir = (direction) ? 1 : -1;


        while (grabRope)
        {
            grabRope = Input.GetKey(player_1_Input.grab);
            rb.MovePosition(rope.GetParticlePosition(ropeIndex));
            rb.transform.position = Vector3.MoveTowards(rb.transform.position, rope.GetParticlePosition(ropeIndex), Time.deltaTime * climeSpeed);
           // rb.transform.position =  rope.GetParticlePosition(ropeIndex);
            Debug.Log("here");
            Debug.DrawLine(transform.position, rope.GetParticlePosition(ropeIndex), Color.white);
            yield return rb.position == rope.GetParticlePosition(ropeIndex);
            WaitForSeconds pauseTime = new WaitForSeconds(2.0f);
            yield return pauseTime;

            ropeIndex = Mathf.Clamp(ropeIndex + dir, 0, rope.activeParticleCount);


        }




    }


 
    private int getClosesParticle(Rigidbody rb)
    {
        float dist           = float.MaxValue;
        int returnIndex      = -1;
        Transform transfrome = rb.transform;
        for(int i = 0; i < rope.solverIndices.Length; ++i)
        {
            int solverIndex = rope.solverIndices[i];
            float disttmep = Vector3.Distance(transfrome.position, rope.solver.positions[solverIndex]);
           
          //  Debug.Log(transfrome.position +"[      ]"+rope.transform.worldToLocalMatrix*rope.solver.positions[solverIndex]);
            Debug.DrawLine(transfrome.position,rope.GetParticlePosition(solverIndex), Color.blue);
            if (dist > disttmep+2)
            {
                dist        = disttmep;
                returnIndex = solverIndex;
            }
        }
        if(returnIndex != -1)
            Debug.DrawLine(transfrome.position, rope.GetParticlePosition(returnIndex), Color.red);
        return returnIndex;
    }

    void Rotate(Vector3 direction, Rigidbody player)
    {
        Transform playerTransform = player.transform;
        float targetAngle         = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle               = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        playerTransform.rotation  = Quaternion.Euler(0f, angle, 0f);

    }

    //void OnDrawGizmos()
    //{

    //    if (rope == null || !rope.isLoaded || player_1 == null)
    //        return;

       
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(player_1.transform.position, rope.solver.positions[getClosesParticle(player_1)]);


    //    Gizmos.color = Color.red;
    //    Gizmos.matrix = rope.solver.transform.localToWorldMatrix;
    //    for (int i = 0; i < rope.solverIndices.Length; ++i)
    //    {
    //        int solverIndex = rope.solverIndices[i];
    //        Gizmos.DrawRay(rope.solver.positions[solverIndex], Vector3.up * 5f);
    //        Gizmos.DrawRay(rope.solver.positions[solverIndex], -Vector3.up * 5f);
    //    }


    //}

    

}
