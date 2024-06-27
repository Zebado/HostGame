using System.Collections;
using System;
using UnityEngine;
using Fusion;

public class PlayerHealth : NetworkBehaviour
{
    public int health = 3;
    public float invulnerabilityDuration = 1f;
    private bool isInvulnerable = false;

    public event Action OnDead;

    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;

        health -= amount;
        if (health <= 0)
        {
            Death();
        }

        StartCoroutine(SetInvulnerable(invulnerabilityDuration));
    }

    public void Death(){
        health = 0;
        OnDead?.Invoke();
    }

    private IEnumerator SetInvulnerable(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    
}