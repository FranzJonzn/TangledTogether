using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment_test : MonoBehaviour
{
    public Rigidbody     player_1;
    private PlayerInput  player_1_Input;
    public Transform     player_1_groundCheck;
    private Vector3      player_1_direction;
    private bool         player_1_isGrounded;
    private bool         player_1_jump;
    private bool         player_1_jumped;
    private float        player_1_startMass;
    [Space]
    public Rigidbody     player_2;
    private PlayerInput  player_2_Input;
    public Transform     player_2_groundCheck;
    private Vector3      player_2_direction;
    private bool         player_2_isGrounded;
    private bool         player_2_jump;
    private bool         player_2_jumped;
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

    public float maxDistanBetweenPlayers = 15f;
    
    void Start()
    {
        player_1_Input = player_1.gameObject.GetComponent<PlayerInputHolder>().playerInput;
        player_2_Input = player_2.gameObject.GetComponent<PlayerInputHolder>().playerInput;

        player_2_startMass = player_2.mass;
        player_1_startMass = player_1.mass;


    }


    bool player_1_sand_ground;
    bool player_2_sand_ground;
    void Update()
    {

         player_1_sand_ground = Input.GetKey(player_1_Input.anchor);
         player_2_sand_ground = Input.GetKey(player_2_Input.anchor);

        player_1.mass = (player_1_isGrounded)                 ? player_1_startMass : aireWeight;
        player_1.mass = (player_1_sand_ground)                ? anchorWeight       : player_1.mass;
      

        player_2.mass = (player_2_isGrounded)                 ? player_2_startMass : aireWeight;
        player_2.mass = (player_2_sand_ground)                ? anchorWeight       : player_2.mass;


        if (!player_2_sand_ground)
        {
            MoveInput(player_2_Input, ref player_2_direction, ref player_2_jump, player_2_isGrounded);
            GroundCheck(out player_2_isGrounded, ref player_2_direction, player_2_groundCheck);
        }
        if (!player_1_sand_ground)
        {
            MoveInput(player_1_Input, ref player_1_direction, ref player_1_jump, player_1_isGrounded);
            GroundCheck(out player_1_isGrounded, ref player_1_direction, player_1_groundCheck);
        }
    }


    private void FixedUpdate()
    {
        if (!player_2_sand_ground)
            Move(player_2, player_2_direction, player_2_isGrounded, player_2_jump, ref player_2_jumped);
        if (!player_1_sand_ground)
            Move(player_1, player_1_direction, player_1_isGrounded, player_1_jump, ref player_1_jumped);


    }
    private float slopeAngle;

    void MoveInput(PlayerInput playerInput, ref Vector3 direction, ref bool jump, bool isGrounded)
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
        jump = Input.GetKeyDown(playerInput.jump);
        
    

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

    void Move(Rigidbody rb, Vector3 direction, bool isGrounded, bool jump, ref bool jumped)
    {
        Vector3 vel = rb.velocity;

        if (isGrounded)
        {
            if (direction.magnitude > 0.1f)
            {

                Rotate(direction, rb);

                float acceleration = maxSpeed * friction;
                vel.x -= friction * Time.deltaTime * vel.x;
                vel.x += acceleration * Time.deltaTime * direction.x;
                vel.y -= friction * Time.deltaTime * vel.y;
                vel.y += acceleration * Time.deltaTime * direction.y;
                vel.z -= friction * Time.deltaTime * vel.z;
                vel.z += acceleration * Time.deltaTime * direction.z;
            }

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
                vel.y += swingPushVelocity * direction.y;
                vel.z += swingPushVelocity * direction.z;
            }
        }


        rb.velocity = vel;


    }



    void Rotate(Vector3 direction, Rigidbody player)
    {
        Transform playerTransform = player.transform;
        float targetAngle         = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle               = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        playerTransform.rotation  = Quaternion.Euler(0f, angle, 0f);

    }

}
