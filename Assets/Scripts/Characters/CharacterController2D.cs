using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    /* 
     * --- C# TIP ---
     * Use SerializeField to expose private variables
     * Private variables are not accessible through other scripts but will display in the Inspector
    */

    // Member Variables
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping
    [SerializeField] private bool m_StickToSlopes = true;                         // Whether or not a player can stick to slopes
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private LayerMask m_WhatIsLadder;                          // A mask determining what is ladder to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_LadderCheck;                           // A position marking where the starting point of ladder ray is.
    [SerializeField] private Transform m_FrontCheck;                            // A position makring where to check if the player is not hitting anything
    [SerializeField] private float m_GroundedRadius = .05f;                      // Radius of the overlap circle to determine if grounded
    [SerializeField] private float m_FrontCheckRadius = .05f;                      // Radius of the overlap circle to determine if front is blocked
    [SerializeField] private float m_GroundRayLength = .2f;                     // Length of the ray beneith controller
    [SerializeField] private float m_LadderRayLength = .5f;                     // Length of the ray above controller

    
    private float m_OriginalGravityScale;
    
    [Header("Events")]
    public UnityEvent OnLandEvent;

    // Public Getters / Setters (Parameters)
    public bool IsGrounded { get; private set; }
    public bool IsClimbing { get; private set; }
    public bool IsFrontBlocked { get; private set; }
    public bool IsFacingRight { get; private set; } = true;
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Anim { get; private set; }

    public bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }


    // Internal Methods
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        m_OriginalGravityScale = Rigidbody.gravityScale;

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(m_GroundCheck.position, m_GroundedRadius);
        Gizmos.DrawWireSphere(m_FrontCheck.position, m_FrontCheckRadius);

        Gizmos.color = Color.blue;
        Ray groundRay = new Ray(transform.position, Vector3.down);
        Gizmos.DrawLine(groundRay.origin, groundRay.origin + groundRay.direction * m_GroundRayLength);

        Gizmos.color = Color.red;
        Ray ladderRay = new Ray(m_LadderCheck.position, Vector3.up);
        Gizmos.DrawLine(ladderRay.origin, ladderRay.origin + ladderRay.direction * m_LadderRayLength);
    }
    private void FixedUpdate()
    {
        AnimateDefault();

        bool wasGrounded = IsGrounded;
        IsGrounded = false;
        IsFrontBlocked = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, m_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                IsGrounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

        colliders = Physics2D.OverlapCircleAll(m_FrontCheck.position, m_FrontCheckRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                IsFrontBlocked = true;
            }
        }
    }

    private void AnimateDefault()
    {
        if(HasParameter("IsGrounded", Anim))
            Anim.SetBool("IsGrounded", IsGrounded);

        if(HasParameter("IsClimbing", Anim))
            Anim.SetBool("IsClimbing", IsClimbing);

        if(HasParameter("JumpY", Anim))
            Anim.SetFloat("JumpY", Rigidbody.velocity.y);
    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        IsFacingRight = !IsFacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    
    // >> Custom methods go here <<

    public void Climb(float offsetY)
    {
        if (HasParameter("ClimbSpeed", Anim))
            Anim.SetFloat("ClimbSpeed", offsetY);

        RaycastHit2D ladderHit = Physics2D.Raycast(m_LadderCheck.position, Vector2.up, m_LadderRayLength, m_WhatIsLadder);
        if (ladderHit.collider != null)
        {
            if (offsetY != 0)
            {
                IsClimbing = true;
            }
        }
        else
        {
            IsClimbing = false;
        }

        if (IsClimbing)
        {
            Rigidbody.gravityScale = 0;
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, offsetY);
        }
        else
        {
            Rigidbody.gravityScale = m_OriginalGravityScale;
        }
    }
    public void Jump(float height)
    {
        // If the player should jump...
        if (IsGrounded)
        {
            // Add a vertical force to the player.
            IsGrounded = false;
            Rigidbody.AddForce(new Vector2(0f, height), ForceMode2D.Impulse);
        }
    }
    
    // Move must be called last!
    public void Move(float offsetX)
    {
        if (HasParameter("IsRunning", Anim))
            Anim.SetBool("IsRunning", offsetX != 0);

        //only control the player if grounded or airControl is turned on
        if (IsGrounded || m_AirControl)
        {
            if (m_StickToSlopes)
            {
                Ray groundRay = new Ray(transform.position, Vector3.down);
                RaycastHit2D groundHit = Physics2D.Raycast(groundRay.origin, groundRay.direction, m_GroundRayLength, m_WhatIsGround);
                if (groundHit.collider != null)
                {
                    Vector3 slopeDirection = Vector3.Cross(Vector3.up, Vector3.Cross(Vector3.up, groundHit.normal));
                    float slope = Vector3.Dot(Vector3.right * offsetX, slopeDirection);

                    offsetX += offsetX * slope;

                    float angle = Vector2.Angle(Vector3.up, groundHit.normal);
                    if (angle > 0)
                    {
                        Rigidbody.gravityScale = 0;
                        Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0f);
                    }
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(offsetX, Rigidbody.velocity.y);

            Vector3 velocity = Vector3.zero;
            // And then smoothing it out and applying it to the character
            Rigidbody.velocity = Vector3.SmoothDamp(Rigidbody.velocity, targetVelocity, ref velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (offsetX > 0 && !IsFacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (offsetX < 0 && IsFacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
    }
}