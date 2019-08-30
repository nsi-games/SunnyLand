using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController2D))]
public class Player : MonoBehaviour
{
    public float jumpHeight = 5f;   // How high the Character Jumps (in units)
    public float climbSpeed = 10f;  // How fast the Character Climbs
    public float moveSpeed = 10f;   // How fast the Character Moves
    public float portalDistance = 1f; // How far from the Portal the player needs to be to press Up & Interact

    private CharacterController2D controller;
    private Transform currentPortal; // Reference to Current Portal

    // Start is called before the first frame update
    void Start()
    {
        // Gather components at the start of the game to save processing! (Cache-ing)
        controller = GetComponent<CharacterController2D>();
    }

    void OnDrawGizmos()
    {
        // If player is over a portal (Trigger)
        if(currentPortal != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentPortal.position, portalDistance);
        }
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

        // If over a portal
        if(currentPortal != null)
        {
            // If Up Arrow is pressed once this frame
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // Get distance between player and portal
                float distance = Vector2.Distance(transform.position, currentPortal.position);
                // If Distance is Less than PortalDistance
                if (distance < portalDistance)
                {
                    // Fire off Interact function
                    currentPortal.SendMessage("Interact");
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Detect hitting item
        if (col.CompareTag("Item"))
        {
            //  Add 1 to score
            GameManager.Instance.AddScore(1);
            //  Play chime sound - Requires an Audio Source
            //  Destroy item
            Destroy(col.gameObject);
        }
        // Detect hitting portal
        if (col.CompareTag("Portal"))
        {
            // Store the current portal
            currentPortal = col.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        // Detect hitting portal
        if (col.CompareTag("Portal"))
        {
            // Store the current portal
            currentPortal = null;
        }
    }
}
