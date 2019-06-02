using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyInput : MonoBehaviour
    {
        public float horizontal = 1;

        Enemy enemy;
                
        // Use this for initialization
        void Start()
        {
            enemy = GetComponent<Enemy>();
            enemy.onEdgeChanged += OnEdgeChanged;
        }

        void OnEdgeChanged(bool isOnEdge)
        {
            if (isOnEdge)
            {
                horizontal *= -1;
            }
        }

        // Update is called once per frame
        void Update()
        {
            enemy.Move(horizontal);
        }
    }
}