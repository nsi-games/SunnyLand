using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(CharacterController2D))]
    public class UserInput : MonoBehaviour
    {
        private CharacterController2D player;
        // Use this for initialization
        void Start()
        {
            player = GetComponent<CharacterController2D>();
        }
        // Update is called once per frame
        void Update()
		{
			float inputH = Input.GetAxisRaw("Horizontal");
			float inputV = Input.GetAxisRaw("Vertical");
            player.Move(inputH, false, false);
            //player.Climb (inputV);
            
        }
    }
}
