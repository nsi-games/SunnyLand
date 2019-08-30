using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController2D))]
public class Player : MonoBehaviour
{
    public float jumpHeight = 5f;   // How high the Character Jumps (in units)
    public float climbSpeed = 10f;  // How fast the Character Climbs
    public float moveSpeed = 10f;   // How fast the Character Moves

    private CharacterController2D controller;

    // Start is called before the first frame update
    void Start()
    {
        // Gather components at the start of the game to save processing! (Cache-ing)
        controller = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * --- Unity Tip ---
         * Input.GetAxis - 
         * Input.GetAxisRaw - 
         */

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isJumping = Input.GetButtonDown("Jump");

        if (isJumping)
        {
            controller.Jump(jumpHeight);
        }

        controller.Climb(vertical * climbSpeed);

        // Move controller horizontally
        controller.Move(horizontal * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Detect hitting item
        if (col.gameObject.tag == "Item")
        {
            //  Add 1 to score
            GameManager.Instance.AddScore(1);
            //  Play chime sound - Requires an Audio Source
            //  Destroy item
            Destroy(col.gameObject);
        }
    }
}
