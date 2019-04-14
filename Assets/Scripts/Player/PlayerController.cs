using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        public float speed = 5f;
        public int health = 100;
        public int damage = 50;
        public float hitForce = 4f;
        public float damageForce = 4f;
        public float maxVelocity;
        public float maxSlopeAngle = 45f;
        [Header("Grounding")]
        public float rayDistance = .5f;
        [Header("Crouch")]
        public bool isCrouching = false;
        [Header("Jump")]
        public float jumpHeight = 2f;
        public int maxJumpCount = 2;
        public bool isGrounded = false;
        public bool isJumping = false;
        [Header("Climb")]
        public float climbSpeed = 5f;
        public bool isClimbing = false;
        public bool isOnSlope = false;

        [Header("References")]
        public Collider2D defaultCollider;
        public Collider2D crouchCollider;


        // Delegates
        public EventCallback onJump;
        public EventCallback onHurt;
        public RaycastHit2DCallback onGroundRayHit;
        public BoolCallback onCrouchChanged;
        public BoolCallback onGroundedChanged;
        public BoolCallback onClimbChanged;
        public FloatCallback onMove;
        public FloatCallback onClimb;

        private Vector3 groundNormal = Vector3.up;
        private Vector3 moveDirection;
        private int currentJump = 0;

        private float inputH, inputV;

        // References
        private SpriteRenderer rend;
        private Animator anim;
        private Rigidbody2D rigid;
        private Collider2D currentCollider;
        private Climbable climbObject;

        private Vector3 spawnPoint;

        #region Unity Functions
        void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();

            spawnPoint = transform.position;
        }
        void Update()
        {
            moveDirection.y += Physics.gravity.y * Time.deltaTime;
            PerformClimb();
            PerformMove();
            PerformJump();
        }
        void FixedUpdate()
        {
            UpdateCollider();
            DetectGround();
            DetectClimbable();
        }
        void OnDrawGizmos()
        {
            Ray groundRay = new Ray(transform.position, Vector3.down);
            Gizmos.DrawLine(groundRay.origin, groundRay.origin + groundRay.direction * rayDistance);

            // Draw direction
            Vector3 right = Vector3.Cross(groundNormal, Vector3.forward);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position - right * 1f, transform.position + right * 1f);
        }
        #endregion

        #region Custom Functions
        private void PerformClimb()
        {
            // If the user is pressing on the vertical axis
            if (inputV != 0 && climbObject != null)
            {
                bool isAtTop = climbObject.IsAtTop(transform.position);
                bool isAtBottom = climbObject.IsAtBottom(transform.position);
                bool isAtTopAndPressingUp = isAtTop && inputV > 0;
                bool isAtBottomAndPressingDown = isAtBottom && inputV < 0;

                // Check if the player is:
                if (!isAtTopAndPressingUp && // not trying to climb up
                    !isAtBottomAndPressingDown && // not trying to climb down
                    !isClimbing) // not climbing
                {
                    // We are now in the climbing state!
                    isClimbing = true;
                    if(onClimbChanged != null)
                    {
                        onClimbChanged.Invoke(isClimbing);
                    }
                }

                if (isAtTopAndPressingUp || isAtBottomAndPressingDown)
                {
                    StopClimbing();
                }
            }

            // Is the player climbing?
            if (isClimbing && climbObject != null)
            {
                // Make sure physics is disabled before moving the player
                DisablePhysics();
                // Logic for moving the player up and down climbable
                float x = climbObject.GetX();
                Vector3 position = transform.position;
                position.x = x;
                position.y += inputV * climbSpeed * Time.deltaTime;
                transform.position = position;
            }
        }
        private void PerformMove()
        {
            if (isOnSlope)
            {
                if (inputH == 0 && isGrounded)
                {
                    rigid.velocity = Vector3.zero;
                }
            }

            Vector3 right = Vector3.Cross(groundNormal, Vector3.back);
            // Add force in the direction of horizontal movement 
            rigid.AddForce(right * inputH * speed);
            
            // Limit the velocity to max velocity
            LimitVelocity();
        }
        private void PerformJump()
        {
            if (isJumping)
            {
                if (isClimbing)
                {
                    StopClimbing();
                }

                if (currentJump < maxJumpCount)
                {
                    currentJump++;
                    rigid.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
                }

                isJumping = false;
            }
        }

        private void DetectClimbable()
        {
            if (isClimbing) return;

            // Perform a box cast that is the size of the player's collision
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, currentCollider.bounds.size, 0);
            foreach (var hit in hits)
            {
                // Check if:
                if (hit != null && // Something was hit 
                    hit.isTrigger) // That hit something is triggered *shakes screen*
                {
                    // We have found our climbable!
                    climbObject = hit.GetComponent<Climbable>();
                    return;
                }
            }

            // No climbable's were found
            climbObject = null;
        }
        private void CheckGround(RaycastHit2D hit)
        {
            // Check if it hit the ground
            if (hit.collider != null &&
                hit.collider.name != name &&
                hit.collider.isTrigger == false)
            {
                // Reset the jump
                currentJump = 0;
                isGrounded = true;
                if (onGroundedChanged != null)
                {
                    onGroundedChanged.Invoke(isGrounded);
                }

                // Set ground's normal (for slopes)
                groundNormal = -hit.normal;

                float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                if (slopeAngle > 0 && 
                    slopeAngle < maxSlopeAngle)
                {
                    isOnSlope = true;
                }
                else
                {
                    isOnSlope = false;
                }

                if(slopeAngle >= maxSlopeAngle)
                {
                    rigid.AddForce(Physics.gravity);
                }

                return;
            }
            else
            {
                isGrounded = false;
                if (onGroundedChanged != null)
                {
                    onGroundedChanged.Invoke(isGrounded);
                }
                if(onGroundRayHit != null)
                {
                    onGroundRayHit.Invoke(hit);
                }
            }
        }
        private void CheckEnemy(RaycastHit2D hit)
        {
            // Check if it hit the ground
            if (hit.collider != null &&
                hit.collider.name != name &&
                hit.collider.isTrigger == false &&
                // Detect enemy
                hit.collider.name.Contains("Player"))
            {
                //EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                //enemy.Hurt(damage, new Vector2(-hit.normal.x, 1));
                PlayerController hitPlayer = hit.collider.GetComponent<PlayerController>();
                hitPlayer.Hurt(damage);

                rigid.AddForce(Vector3.up * hitForce, ForceMode2D.Impulse);
            }
        }
        private void DetectGround()
        {
            // Create ray going in the direction of down
            Ray groundRay = new Ray(transform.position, Vector3.down);
            // Shoot ray below the player
            RaycastHit2D[] hits = Physics2D.RaycastAll(groundRay.origin, groundRay.direction, rayDistance);
            foreach (var hit in hits)
            {
                CheckGround(hit);
                CheckEnemy(hit);
                if (Mathf.Abs(hit.normal.x) > 0.1f)
                {
                    rigid.gravityScale = 0;
                }
                else
                {
                    rigid.gravityScale = 1;
                }
            }
        }
        private void LimitVelocity()
        {
            Vector3 vel = rigid.velocity;
            if (vel.magnitude >= maxVelocity)
            {
                vel = vel.normalized * maxVelocity;
            }
            rigid.velocity = vel;
        }
        private void StopClimbing()
        {
            climbObject = null;

            // We are no longer climbing
            isClimbing = false;
            if (onClimbChanged != null)
            {
                onClimbChanged.Invoke(isClimbing);
            }

            // Re-enable physics
            EnablePhysics();
        }
        private void DisablePhysics()
        {
            rigid.gravityScale = 0;
            rigid.simulated = false;
            rigid.velocity = Vector2.zero;
        }
        private void EnablePhysics()
        {
            rigid.simulated = true;
            rigid.gravityScale = 1;
        }
        private void UpdateCollider()
        {
            if (isCrouching)
            {
                defaultCollider.enabled = false;
                currentCollider = crouchCollider;
            }
            else
            {
                crouchCollider.enabled = false;
                currentCollider = defaultCollider;
            }
            currentCollider.enabled = true;
        }

        #region Accessors
        public void Reset()
        {
            transform.position = spawnPoint;
            health = 100;
        }
        public void Jump()
        {
            isJumping = true;
            
            if (onJump != null)
            {
                onJump.Invoke();
            }
        }
        public void Crouch()
        {
            isCrouching = true;

            // Invoke event
            if(onCrouchChanged != null)
            {
                onCrouchChanged.Invoke(isCrouching);
            }
        }
        public void UnCrouch()
        {
            isCrouching = false;

            // Invoke event
            if (onCrouchChanged != null)
            {
                onCrouchChanged.Invoke(isCrouching);
            }
        }
        public void Move(float horizontal)
        {
            // Don't do any horizontal movement if we're climbing OR crouching
            if (isClimbing || isCrouching)
                return;

            // If there is horizontal input
            if (horizontal != 0)
            {
                // Flip the sprite in the correct direction
                rend.flipX = horizontal < 0;
            }

            inputH = horizontal;

            // Invoke Event
            if (onMove != null)
            {
                onMove.Invoke(horizontal);
            }
        }
        public void Climb(float vertical)
        {
            inputV = vertical;
            
            // Invoke Event
            if (onClimb != null)
            {
                onClimb.Invoke(vertical);
            }
        }
        public void Hurt(int damage, Vector2? hitNormal = null)
        {
            if(isClimbing)
            {
                StopClimbing();
            }

            // Deal damage to player
            health -= damage;

            // Add impulse in damage direction
            Vector2 force = Vector2.zero;
            if (hitNormal != null)
            {
                force = hitNormal.Value;
            }
            rigid.AddForce(force * damageForce, ForceMode2D.Impulse);

            // Invoke event
            if(onHurt != null)
            {
                onHurt.Invoke();
            }
        }

        public void IsDummy()
        {
            defaultCollider.enabled = false;
            crouchCollider.enabled = false;
            rigid.gravityScale = 0;
        }
        #endregion
        #endregion
    }
}