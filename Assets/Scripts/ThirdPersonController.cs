using System;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float walkSpeed = 2, runSpeed = 4, speed = 0;

    public Transform modelMesh;

    private Rigidbody rb;
    
    //MoveVector -- direction of movement
    //playerDirection -- direction for the modelMesh.
    //pD will chase mV
    private Vector3 movementVector, playerDirection;

    public float jumpForce = 4;
    public bool grounded = true;

    //object references are good to put within Awake() rather than Start()
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerDirection = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //Perform a boxcast straight down to see if I'm touching the ground
        grounded = Physics.BoxCast(transform.position + Vector3.up, Vector3.one * 0.5f, Vector3.down, modelMesh.rotation, 0.7f);
        
        //Flattened versions of the Camera's direction. Removing their y-axis from play
        Vector3 forwardFlat = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
        Vector3 sideFlat = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
        
        //Calculation of movementVector using WASD.
        //Normalized to avoid inappropriate speeds (diagonals)
        movementVector = (forwardFlat * Input.GetAxis("Vertical")) + (sideFlat * Input.GetAxis("Horizontal"));
        
        //when we Normalize makes something within 0 - 1 range
        movementVector.Normalize();
        
        //Rotating player direction towards the movement vector
        //Locking rotation forward if RMB is held
        if (Input.GetMouseButton(1))
        {
            playerDirection = Vector3.Slerp(playerDirection, forwardFlat, 5 * Time.deltaTime);
        }
        else
        {
            //slerp is spherical interpolation try to use slerps for changing movement 
            playerDirection = Vector3.Slerp(playerDirection,
                movementVector.magnitude > 0 ? movementVector : playerDirection, 5 * Time.deltaTime);
        }

        modelMesh.rotation = Quaternion.LookRotation(playerDirection);
        
        //Jumping if SPACE is pressed AND we're grounded
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        }
        
        //Lerping of SPEED towards 0, walkspeed and runspeed, given condition
        //MOVE TOWARDS -- lerping with a set step
        if (movementVector.magnitude > 0)
        {
            speed = Mathf.MoveTowards(speed, Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed, 1 * Time.deltaTime);
        }
        else
        {
            speed = Mathf.MoveTowards(speed, 0, 5 * Time.deltaTime);
        }
    }
    
    //FixedUpdate() doesn't happen per frame instead updates at a fixed interval, use FixedUpdate() for physics based operations
    void FixedUpdate()
    {
        //use movementvector and speed to calculate my object's movement this FixedUpdate (0.02 sec)
        //read the object's y velocity to retain gravity
        rb.linearVelocity = movementVector * speed + Vector3.up * rb.linearVelocity.y;
    }
}