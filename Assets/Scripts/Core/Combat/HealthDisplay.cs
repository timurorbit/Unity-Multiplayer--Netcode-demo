using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")] [SerializeField]
    private Health health;

    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if (!IsClient)
        {
            return;
        }

        health.CurrentHealth.OnValueChanged += HandlehealthChange;
        
        HandlehealthChange(0, health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient)
        {
            return;
        }

        health.CurrentHealth.OnValueChanged -= HandlehealthChange;
    }

    private void HandlehealthChange(int oldHealth, int newHealth)
    {
        healthBarImage.fillAmount = (float) newHealth / health.maxHealth;
    }
    
}
