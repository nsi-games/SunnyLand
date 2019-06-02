using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

[RequireComponent(typeof(Player))]
public class OnDeathEvent : MonoBehaviour
{
  public UnityEvent onDeath;

  private Player player;
  void Start()
  {
    player = GetComponent<Player>();
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
    if (col.name.Contains("Enemy"))
    {
      Enemy enemy = col.GetComponent<Enemy>();
      Vector3 dir = enemy.transform.position - player.transform.position;
      player.Hurt(enemy.damage, new Vector2(-dir.normalized.x, 0));
    }

    if (col.name.Contains("DeathZone"))
    {
      if (onDeath != null)
      {
        onDeath.Invoke();
      }
    }
  }
}
