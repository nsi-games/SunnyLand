using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyController : MonoBehaviour
    {
        public int health = 100;
        public int damage = 50;
        public float hurtForce = 2f;
        [Header("Movement")]
        public float speed = 5f;
        public float maxVelocity = 5f;
        [Header("Slope")]
        public float maxSlopeAngle = 45f;
        public bool isOnSlope = false;
        [Header("Ground")]
        public float groundRayDistance = 0.2f;
        public bool isGrounded = false;
        [Header("Edge Detection")]
        public float edgeRayDistance = 0.4f;
        public bool isOnEdge = false;
        [Header("References")]
        public Transform edgeDetector;


        // Delegates
        public FloatCallback onMove;
        public BoolCallback onGroundedChanged;
        public BoolCallback onEdgeChanged;
        public RaycastHit2DCallback onGroundRayHit;
        
        protected Vector3 groundNormal = Vector3.up;

        protected SpriteRenderer rend;
        protected Rigidbody2D rigid;
        protected float horizontal;

        #region Unity Functions
        // Use this for initialization
        void Start()
        {
            rigid = GetComponent<Rigidbody2D>();
            rend = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void FixedUpdate()
        {
            PerformMove();

            DetectGround();
            DetectEdge();
        }
        void OnDrawGizmos()
        {
            // Ground Ray
            Gizmos.color = Color.red;
            Ray groundRay = new Ray(transform.position, Vector3.down);
            Gizmos.DrawLine(groundRay.origin, groundRay.origin + groundRay.direction * groundRayDistance);

            // Edge Ray
            if (edgeDetector != null)
            {
                Gizmos.color = Color.cyan;
                Ray edgeRay = new Ray(edgeDetector.position, Vector3.down);
                Gizmos.DrawLine(edgeRay.origin, edgeRay.origin + edgeRay.direction * edgeRayDistance);
            }
            
            // Draw direction
            Vector3 right = Vector3.Cross(groundNormal, Vector3.forward);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position - right * 1f, transform.position + right * 1f);
        }
        //void OnCollisionEnter2D(Collision2D collision)
        //{
        //    Collider2D col = collision.collider;
        //    ContactPoint2D contact = collision.contacts[0];
        //    if(col.name.Contains("Player"))
        //    {
        //        PlayerController player = col.GetComponent<PlayerController>();
        //        if(player != null)
        //        {
        //            HitPlayer(player, -contact.normal);
        //        }
        //    }    
        //}
        #endregion

        #region Custom Functions
        private void CheckGround(RaycastHit2D hit)
        {
            // Check if it hit the ground
            if (hit.collider != null &&
                hit.collider.name != name &&
                hit.collider.isTrigger == false)
            {
                // Reset the jump
                isGrounded = true;
                if (onGroundedChanged != null)
                {
                    onGroundedChanged.Invoke(isGrounded);
                }

                // Set ground's normal (for slopes)
                groundNormal = hit.normal;

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

                if (slopeAngle >= maxSlopeAngle)
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
                if (onGroundRayHit != null)
                {
                    onGroundRayHit.Invoke(hit);
                }
            }
        }
        private void DetectEdge()
        {
            // Create a ray going from empty GameObject set in front of player
            Ray edgeRay = new Ray(edgeDetector.position, Vector3.down);
            // Shoot ray below the player
            RaycastHit2D[] hits = Physics2D.RaycastAll(edgeRay.origin, edgeRay.direction, edgeRayDistance);

            bool wasOnEdge = isOnEdge;

            if (hits.Length == 0)
            {
                isOnEdge = true;
            }
            else
            {
                isOnEdge = false;
            }

            // If isOnEdge changed
            if(wasOnEdge != isOnEdge)
            {
                // Invoke event
                if(onEdgeChanged != null)
                {
                    onEdgeChanged.Invoke(isOnEdge);
                }
            }
        }
        private void DetectGround()
        {
            // Create ray going in the direction of down
            Ray groundRay = new Ray(transform.position, Vector3.down);
            // Shoot ray below the player
            RaycastHit2D[] hits = Physics2D.RaycastAll(groundRay.origin, groundRay.direction, groundRayDistance);
            foreach (var hit in hits)
            {
                CheckGround(hit);

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
        private void HitPlayer(PlayerController player, Vector2 hitNormal)
        {
            player.Hurt(damage, new Vector2(hitNormal.x, 1));
        }
        

        protected void LimitVelocity()
        {
            Vector3 vel = rigid.velocity;
            if(vel.magnitude > maxVelocity)
            {
                vel = vel.normalized * maxVelocity;
            }
            rigid.velocity = vel;
        }
        public virtual void PerformMove() { }

        public void Reset()
        {
            health = 100;
        }
        public void Move(float horizontal)
        {
            this.horizontal = horizontal;

            if(horizontal != 0)
            {
                //rend.flipX = horizontal > 0;
                if(horizontal > 0)
                    transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                if (horizontal < 0)
                    transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
            }

            if (onMove != null)
            {
                onMove.Invoke(horizontal);
            }
        }
        public void Hurt(int damage, Vector2? hurtDirection = null)
        {
            health -= damage;

            Vector2 force = Vector2.zero;
            if(hurtDirection != null)
            {
                force = hurtDirection.Value;
            }
            rigid.AddForce(force * hurtForce, ForceMode2D.Impulse);
        }
        #endregion
    }
}