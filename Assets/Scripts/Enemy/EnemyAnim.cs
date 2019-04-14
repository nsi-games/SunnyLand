using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(EnemyController))]
    [RequireComponent(typeof(Animator))]
    public class EnemyAnim : MonoBehaviour
    {
        private EnemyController enemy;
        private Animator anim;

        // Use this for initialization
        void Start()
        {
            enemy = GetComponent<EnemyController>();
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            anim.SetBool("IsDead", enemy.health <= 0);
        }
    }
}