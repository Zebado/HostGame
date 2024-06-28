using System.Collections;
using System;
using UnityEngine;
using Fusion;

public class PlayerHealth : NetworkBehaviour
{
    public event Action OnDead;

    public void TakeDamage(int amount)
    {
        Death();
    }

    public void Death(){
        OnDead?.Invoke();
    }

    
}