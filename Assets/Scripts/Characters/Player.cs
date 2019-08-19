using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public float jumpHeight = 5f;
    public float climbSpeed = 10f;
    public float moveSpeed = 10f;

    private CharacterController2D controller;
    
    void Start()
    {
        // Gather components at the start of the game to save processing! (Cache-ing)
        controller = GetComponent<CharacterController2D>();
    }

    void Update()
    {
        /* 
         * --- Unity Tip ---
         * Input.GetAxis returns a value between -1 to 1 (with smoothing applied)
         * Input.GetAxisRaw returns either -1 or 1 rounded number (no smoothing)
         */

        // Inputs
        float inputH = Input.GetAxisRaw("Horizontal");
        float inputV = Input.GetAxisRaw("Vertical");
        bool isJumping = Input.GetButtonDown("Jump");
        bool isRunning = inputH != 0;
        
        if (isJumping)
        {
            controller.Jump(jumpHeight);
        }

        // Horizontal & Vertical Movement
        float horizontal = inputH * moveSpeed;
        float vertical = inputV * climbSpeed;

        // Climb with Vertical Inputs
        controller.Climb(vertical);

        // Move with Horizontal Inputs
        controller.Move(horizontal); // CharacterController2D.Move must be last!
    }
}
