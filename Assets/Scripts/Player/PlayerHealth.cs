using System.Collections;
using System;
using UnityEngine;
using Fusion;

public class PlayerHealth : NetworkBehaviour
{
    public int health = 3;
    public float invulnerabilityDuration = 2f;
    private bool isInvulnerable = false;

    public event Action OnDead;

    public void TakeDamage(int amount)
    {
        if (isInvulnerable) return;

        health -= amount;
        if (health <= 0)
        {
            health = 0;
            OnDead?.Invoke();
            RPC_HandlePlayerDeath();
        }

        StartCoroutine(SetInvulnerable(invulnerabilityDuration));
    }

    private IEnumerator SetInvulnerable(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_HandlePlayerDeath()
    {
        GameManager.Instance.ShowDefeatMenu();
    }
}