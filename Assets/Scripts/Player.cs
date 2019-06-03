using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Prime31;

public class Player : MonoBehaviour
{
  public float gravity = -10;
  public float moveSpeed = 10f;
  public float jumpHeight = 7f;
  public float centreRadius = .1f;

  private CharacterController2D controller;
  private SpriteRenderer rend;
  private Animator anim;

  private Vector3 velocity;
  private bool isClimbing = false; // Is in climbing state


  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, centreRadius);
  }

  void Start()
  {
    controller = GetComponent<CharacterController2D>();
    rend = GetComponent<SpriteRenderer>();
    anim = GetComponent<Animator>();
  }

  void Update()
  {
    // Gathers Left and Right input
    float inputH = Input.GetAxis("Horizontal");
    // Gathers Up and Down input
    float inputV = Input.GetAxis("Vertical");
    // If controller is NOT grounded
    if (!controller.isGrounded)
    {
      // Apply delta to gravity
      velocity.y += gravity * Time.deltaTime;
    }
    // Get Spacebar input
    bool isJumping = Input.GetButtonDown("Jump");
    // If Player pressed jump
    if (isJumping)
    {
      // Make the controller jump
      Jump();
    }


    // 1.
    anim.SetBool("IsGrounded", controller.isGrounded);
    // 2.
    anim.SetFloat("JumpY", velocity.y);

    Move(inputH);
    Climb(inputV);

    // Applies velocity to controller (to get it to move)
    controller.move(velocity * Time.deltaTime);
  }

  void Move(float inputH)
  {
    // Move the character controller left / right with input
    velocity.x = inputH * moveSpeed;

    // Set bool to true is input is pressed
    bool isRunning = inputH != 0;

    // Animate the player to running if input is pressed
    anim.SetBool("IsRunning", isRunning);
       
    // Check if input is pressed
    if (isRunning)
    {
      // Flip character depending on left/right input
      rend.flipX = inputH < 0;
    }
  }

  void Climb(float inputV)
  {
    bool isOverLadder = false; // Is overlapping ladder
    // Get a list of all hit objects overlapping point
    Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
    // Loop through each point
    foreach (var hit in hits)
    {
      // If point overlaps a climbable object
      if(hit.tag == "Ladder")
      {
        // Player is overlapping ladder!
        isOverLadder = true;
        break; // Exit foreach loop
      }
    }

    // If is overlapping ladder and input V has been made
    if(isOverLadder && inputV != 0)
    {
      //  Is Climbing
      isClimbing = true;
    }

    // If is climbing
    //  Perform logic for climbing
  }

  void Jump()
  {
    // 3.
    velocity.y = jumpHeight;
  }
}
