using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(PlayerController))]
    public class UserInput : MonoBehaviour
    {
        private PlayerController player;
        // Use this for initialization
        void Start()
        {
            player = GetComponent<PlayerController>();
        }
        // Update is called once per frame
        void Update()
		{
			float inputH = Input.GetAxisRaw("Horizontal");
			float inputV = Input.GetAxisRaw("Vertical");
            player.Move(inputH);
            player.Climb (inputV);
            if(Input.GetKeyDown(KeyCode.Space))
            {
                player.Jump();
            }
            if(Input.GetKey(KeyCode.LeftControl))
            {
                player.Crouch();
            }
            if(Input.GetKeyUp(KeyCode.LeftControl))
            {
                player.UnCrouch();
            }

            /// TEST
            if(Input.GetKeyDown(KeyCode.J))
            {
                player.Hurt(10, new Vector3(1, 1));
            }
        }
    }
}