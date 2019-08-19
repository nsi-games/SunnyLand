using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class Opossum : MonoBehaviour
{
    public float speed = 2f;
    private CharacterController2D controller;

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!controller.IsGrounded || controller.IsFrontBlocked)
        {
            controller.Flip();
            speed *= -1f;
        }
        else
        {
            controller.Move(speed);
        }
    }
}
