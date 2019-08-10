using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public float climbSpeed = 10f;
    public float moveSpeed = 10f;

    private CharacterController2D controller;
    private SpriteRenderer rend;
    private Animator anim;

    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Gathers Left and Right input
        float inputH = Input.GetAxisRaw("Horizontal");
        // Gathers Up and Down input
        float inputV = Input.GetAxisRaw("Vertical");
        // Get Spacebar input
        bool isJumping = Input.GetButtonDown("Jump");
        // Set bool to true is input is pressed
        bool isRunning = inputH != 0;
        
        // 1.
        anim.SetBool("IsGrounded", controller.IsGrounded);
        // 2. 
        anim.SetBool("IsClimbing", controller.IsClimbing);
        // 3.
        anim.SetFloat("JumpY", controller.Rigidbody.velocity.y);
        // 4.
        anim.SetFloat("ClimbSpeed", inputV);
        // 5. Animate the player to running if input is pressed
        anim.SetBool("IsRunning", isRunning);

        //Move(inputH);
        //Climb(inputV);

        Vector3 movement = new Vector3(inputH * moveSpeed, inputV * climbSpeed) * Time.deltaTime;
        controller.Move(movement.x, movement.y);

        if(isJumping)
        {
            controller.Jump();
        }
    }
}
