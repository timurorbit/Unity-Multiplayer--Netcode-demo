using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinWallet : NetworkBehaviour
{
    
    [Header("References")]
    [SerializeField] private Health _health;

    [SerializeField] private BountyCoin coinPrefab;
    
    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    [Header("Settings")] [SerializeField] private float coinSpread = 3f;
    [Header("Settings")] [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;
    
    private float coinRadius;
    
    private Collider2D[] coinBuffer = new Collider2D[1];


    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        _health.onDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }

        _health.onDie -= HandleDie;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Coin>(out Coin coin))
        {
          int value = coin.Collect();
          if (!IsServer)
          {
              return;
          }

          totalCoins.Value += value;
        }
    }

    public void SpendCoins(int amount)
    {
        totalCoins.Value -= amount;
    }
    
    private void HandleDie(Health health)
    {
        int bountyValue = (int) (totalCoins.Value * (bountyPercentage / 100));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if (bountyCoinValue < minBountyCoinValue)
        {
            return;
        }

        for (int i = 0; i < bountyCoinCount; i++)
        {
            BountyCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }
    
    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 spawnPoint = (Vector2) transform.position + Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircle(spawnPoint, coinRadius, new ContactFilter2D().NoFilter(), coinBuffer);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
