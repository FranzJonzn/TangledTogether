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
    private Camera       player_1_camera;

    [Space]
    public Rigidbody     player_2;
    private PlayerInput  player_2_Input;
    public Transform     player_2_groundCheck;
    private Vector3      player_2_direction;
    private bool         player_2_isGrounded;
    private bool         player_2_jump;
    private bool         player_2_jumped;
    private float        player_2_startMass;
    private Camera       player_2_camera;
    [Space]
    [Space]
    public LayerMask groundMask;
    public float     groundDistance = 0.4f;
    [Space]
    [Space]
    public float maxSpeed       = 7f;
    public float friction       = 15f;
    public Vector3 jumpVelocity = new Vector3(0f, 1f, 0f);
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
    public bool fps = false;
    public Camera camera3D;
    
    void Start()
    {
        player_1_Input = player_1.gameObject.GetComponent<PlayerInputHolder>().playerInput;
        player_2_Input = player_2.gameObject.GetComponent<PlayerInputHolder>().playerInput;

        player_2_startMass = player_2.mass;
        player_1_startMass = player_1.mass;

        player_2_camera = player_2.gameObject.GetComponentInChildren<Camera>();
        player_1_camera = player_1.gameObject.GetComponentInChildren<Camera>();

    }


    void Update()
    {

        player_1.mass = (player_1_isGrounded)                 ? player_1_startMass : aireWeight;
        player_1.mass = (Input.GetKey(player_1_Input.anchor)) ? anchorWeight       : player_1.mass;
      

        player_2.mass = (player_2_isGrounded)                 ? player_2_startMass : aireWeight;
        player_2.mass = (Input.GetKey(player_2_Input.anchor)) ? anchorWeight       : player_2.mass;


        if (fps)
        {
            camera3D.gameObject.SetActive(false);
            player_2_camera.gameObject.SetActive(true);
            player_1_camera.gameObject.SetActive(true);
            MoveInputFPS(player_2_Input, ref player_2_direction, ref player_2_jump, player_2_camera);
            MoveInputFPS(player_1_Input, ref player_1_direction, ref player_1_jump, player_1_camera);
        }
        else
        {
            camera3D.gameObject.SetActive(true);
            player_2_camera.gameObject.SetActive(false);
            player_1_camera.gameObject.SetActive(false);
            MoveInput(player_2_Input, ref player_2_direction, ref player_2_jump);
            MoveInput(player_1_Input, ref player_1_direction, ref player_1_jump);
        }




        GroundCheck(out player_2_isGrounded, ref player_2_direction, player_2_groundCheck);
        GroundCheck(out player_1_isGrounded, ref player_1_direction, player_1_groundCheck);
    }


    private void FixedUpdate()
    {
        Move(player_2, player_2_direction, player_2_isGrounded, player_2_jump, ref player_2_jumped);
        Move(player_1, player_1_direction, player_1_isGrounded, player_1_jump, ref player_1_jumped);
        //Jump(ref player_2_jump, player_2_isGrounded, player_2_Input, jumpVelocity, player_2);
        //Jump(ref player_1_jump, player_1_isGrounded, player_1_Input, jumpVelocity,player_1);


    }
    private float slopeAngle;

    void MoveInput(PlayerInput playerInput, ref Vector3 direction, ref bool jump)
    {
        direction = Vector3.zero;
        if (Input.GetKey(playerInput.left))
            direction += Vector3.left;
        if (Input.GetKey(playerInput.right))
            direction += Vector3.right;
        if (Input.GetKey(playerInput.up))
            direction += Vector3.forward;
        if (Input.GetKey(playerInput.down))
            direction += Vector3.back;

        direction.Normalize();


       
            jump = Input.GetKeyDown(playerInput.jump);
        
    

    }
    void MoveInputFPS(PlayerInput playerInput, ref Vector3 direction, ref bool jump, Camera camera)
    {
        direction = Vector3.zero;
        if (Input.GetKey(playerInput.left))
            direction += -camera.transform.right;// Vector3.left;
        if (Input.GetKey(playerInput.right))
            direction += camera.transform.right;// Vector3.right;
        if (Input.GetKey(playerInput.up))
            direction += camera.transform.forward;// Vector3.forward;
        if (Input.GetKey(playerInput.down))
            direction += -camera.transform.forward;// Vector3.back;

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

        if (direction.magnitude > 0.1f && isGrounded)
        {
            if(!fps)
                Rotate(direction, rb);

            float acceleration = maxSpeed * friction;
            vel.x -= friction     * Time.deltaTime * vel.x;
            vel.x += acceleration * Time.deltaTime * direction.x;
            vel.y -= friction     * Time.deltaTime * vel.y;
            vel.y += acceleration * Time.deltaTime * direction.y;
            vel.z -= friction     * Time.deltaTime * vel.z;
            vel.z += acceleration * Time.deltaTime * direction.z;
        }

        if (!jumped && jump)
        {
            vel   += jumpVelocity ;
            jumped = true;
        }
        else if(isGrounded)
            jumped = false;



        rb.velocity = vel;


    }
    //void Jump(ref bool jump, bool isGrounded, PlayerInput playerInput, Vector3 jumpVelocity, Rigidbody player)
    //{

    //    if (jump)
    //    {
    //        jump = isGrounded;
    //    }
    //    else if(isGrounded && Input.GetKeyDown(playerInput.jump))
    //    {
    //        player.velocity += jumpVelocity;
    //    }
     
    //}



    void Rotate(Vector3 direction, Rigidbody player)
    {
        Transform playerTransform = player.transform;
        float targetAngle         = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle               = Mathf.SmoothDampAngle(playerTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        playerTransform.rotation  = Quaternion.Euler(0f, angle, 0f);

    }

}
