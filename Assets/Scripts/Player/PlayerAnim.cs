using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(PlayerController))]
	[RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class PlayerAnim : MonoBehaviour
	{
		private PlayerController controller;
		private Rigidbody2D rigid;
		private Animator anim;

        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
            controller = GetComponent<PlayerController>();
			rigid = GetComponent<Rigidbody2D>();
            controller.onJump += OnJump;
            controller.onHurt += OnHurt;
            controller.onMove += OnMove;
            controller.onClimb += OnClimb;
        }

		void Update() 
		{
			anim.SetBool("IsGrounded", controller.isGrounded);
            anim.SetBool("IsClimbing", controller.isClimbing);
            anim.SetBool("IsCrouching", controller.isCrouching);
            anim.SetFloat("JumpY", rigid.velocity.normalized.y);
		}

        void OnJump()
        {

        }
        
        void OnHurt()
        {
            anim.SetTrigger("Hurt");
        }
        
        void OnMove(float input)
        {
            anim.SetBool("IsRunning", input != 0);
        }

        void OnClimb(float input)
        {
            anim.SetFloat("ClimbY", Mathf.Abs(input));
        }
    }
}