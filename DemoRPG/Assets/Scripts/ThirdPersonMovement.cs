using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    private Vector3 velocity;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float gravity;

    [SerializeField] private float jumpHeight;
    Animator anim;
    

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        
        Move();
    }

    bool isPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    private void Move()
    {
        //Checks if grounded and stop applying gravity if true
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
            velocity.x = 0;
            velocity.z = 0;
        }
        
        //Y-Axis
        float horizontal = Input.GetAxisRaw("Horizontal");
        //Z-Axis
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (isGrounded)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);


            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

          
            if (direction.magnitude >= 0.1f && !isPlaying(anim, "Jump"))
            {
                controller.Move(moveDirection.normalized * speed * Time.deltaTime);
                anim.SetBool("Run", true);

            }
            else
            {
                anim.SetBool("Run", false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity = moveDirection.normalized * speed;
                //Jump();

                anim.SetTrigger("Jump");
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
    

                anim.SetTrigger("Jab");
            }
        }
        


        //Calculate Gravity
        velocity.y += gravity * Time.deltaTime;
        //Apply gravity
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }
}

