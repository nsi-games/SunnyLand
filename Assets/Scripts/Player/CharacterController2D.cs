using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float jumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool airControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask whatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private float rayDistance = 2.1f;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform ceilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D crouchCollider;				// A collider that will be disabled when crouching

	const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool grounded;            // Whether or not the player is grounded.
	const float ceilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D rigid;
	private bool facingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;
    private Vector3 groundNormal = Vector3.up;


	[Header("Events")]
	[Space]
	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool wasCrouching = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * rayDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position - groundNormal * rayDistance);


        Vector3 right = Vector3.Cross(groundNormal, Vector3.forward);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position - right * rayDistance);
    }

    private void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = grounded;
		grounded = false;
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, rayDistance, whatIsGround);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != gameObject)
            {
                groundNormal = hit.normal;
                grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
        if(move == 0)
        {
            rigid.velocity = Vector3.zero;
            rigid.isKinematic = true;
            return;
        }
        else
        {
            rigid.isKinematic = false;
        }

        // If crouching, check to see if the character can stand up
        if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, whatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (grounded || airControl)
		{

			// If crouching
			if (crouch)
			{
				if (!wasCrouching)
				{
					wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= crouchSpeed;

				// Disable one of the colliders when crouching
				//crouchCollider.enabled = false;
			}
            else
			{
				// Enable the collider when not crouching
				//crouchCollider.enabled = true;

				if (wasCrouching)
				{
					wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

            Vector3 right = Vector3.Cross(groundNormal, Vector3.forward);

            // Move the character by finding the target velocity
            Vector3 targetVelocity = right * move;
            targetVelocity.y = rigid.velocity.y;
            // And then smoothing it out and applying it to the character
            rigid.velocity = Vector3.SmoothDamp(rigid.velocity, targetVelocity, ref velocity, movementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !facingRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && facingRight)
				// ... flip the player.
				Flip();
		}
		// If the player should jump...
		if (grounded && jump)
		{
			// Add a vertical force to the player.
			grounded = false;
			rigid.AddForce(new Vector2(0f, jumpForce));
		}
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
