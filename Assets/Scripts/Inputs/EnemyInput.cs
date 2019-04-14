using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(EnemyController))]
    public class EnemyInput : MonoBehaviour
    {
        public float horizontal = 1;

        EnemyController enemy;
                
        // Use this for initialization
        void Start()
        {
            enemy = GetComponent<EnemyController>();
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