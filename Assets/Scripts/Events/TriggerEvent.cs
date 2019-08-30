using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent onEnter, onStay, onExit;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Invoke Enter Event!
        onEnter.Invoke();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        // Invoke Stay Event!
        onStay.Invoke();
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        // Invoke Stay Event!
        onExit.Invoke();
    }
}
