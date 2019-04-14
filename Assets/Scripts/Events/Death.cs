using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace SunnyLand
{
    [RequireComponent(typeof(PlayerController))]
    public class Death : MonoBehaviour
    {
        public UnityEvent onDeath;

        private PlayerController player;
        void Start()
        {
            player = GetComponent<PlayerController>();   
        }

        private void Update()
        {
            if (player.health <= 0)
            {
                if (onDeath != null)
                {
                    onDeath.Invoke();
                }
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if(col.name.Contains("Enemy"))
            {
                EnemyController enemy = col.GetComponent<EnemyController>();
                Vector3 dir = enemy.transform.position - player.transform.position;
                player.Hurt(enemy.damage, new Vector2(-dir.normalized.x, 0));
            }

            if(col.name.Contains("DeathZone"))
            {
                if(onDeath != null)
                {
                    onDeath.Invoke();
                }
            }
        }
    }
}