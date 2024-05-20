using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damage = 5;

    private ulong ownerClientId;
    
    public void SetOwner(ulong ownerLientId)
    {
        this.ownerClientId = ownerLientId;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody == null)
        {
            return;
        }
        
        if (other.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
        {
            if (networkObject.OwnerClientId == ownerClientId)
            {
                return;
            }
        }

        if (other.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage);
        }
    }
}
