using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{

    [field: SerializeField] public int maxHealth { get; private set; } = 100;
    
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    private bool isDead;

    public Action<Health> onDie;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        CurrentHealth.Value = maxHealth;
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(int value)
    {
        if (!isDead)
        {
            int tempHealth = CurrentHealth.Value + value;
            CurrentHealth.Value = Mathf.Clamp(tempHealth, 0, maxHealth);
        }

        if (CurrentHealth.Value == 0)
        {
            onDie?.Invoke(this);
            isDead = true;
        }
    }
}
