using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Paddle"))
        {
            this.ApplyEffect();
        }

        if (collision.CompareTag("Paddle") || collision.CompareTag("DeathCollider"))
            Destroy(this.gameObject);
    }

    protected abstract void ApplyEffect();
}
