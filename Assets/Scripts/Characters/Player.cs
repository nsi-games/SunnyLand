using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Prime31;

public class Player : MonoBehaviour
{
  public int health = 100;
  public float gravity = -10;
  public float moveSpeed = 10f;

  private CharacterController2D controller;
  private Animator anim;


  void Start()
  {
    controller = GetComponent<CharacterController2D>();
    anim = GetComponent<Animator>();
  }

  void Update()
  {
    float inputH = Input.GetAxis("Horizontal");
    float inputV = Input.GetAxis("Vertical");
    Move(inputH);
    Climb(inputV);
  }

  void Move(float inputH)
  {
    controller.move(transform.right * inputH * moveSpeed * Time.deltaTime);
    anim.SetBool("IsRunning", inputH != 0);

    // Sneak Peak:
    // rend.flipX = inputH > 0
  }

  void Climb(float inputV)
  {

  }

  public void Hurt(int damage, Vector2 direction)
  {

  }
}
