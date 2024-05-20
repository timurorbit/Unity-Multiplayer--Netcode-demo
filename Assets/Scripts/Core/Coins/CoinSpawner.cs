using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin coinPrefab;

    [SerializeField] private int maxCoins = 50;
    [SerializeField] private int coinValue = 10;
    [SerializeField] private Vector2 xSpawnRange;
    [SerializeField] private Vector2 ySpawnRange;

    [SerializeField] private LayerMask _layerMask;

    private float coinRadius;
    
    private Collider2D[] coinBuffer = new Collider2D[1];


    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < maxCoins; i++)
        {
            SpawnCoin();
        }
        
    }

    private void SpawnCoin()
    {
        RespawningCoin respawningCoin = Instantiate(coinPrefab, GetSpawnPoing(), Quaternion.identity);
        respawningCoin.SetValue(coinValue);
        respawningCoin.GetComponent<NetworkObject>().Spawn();

        respawningCoin.OnCollected += HandleCoinCollected;
    }

    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoing();
        coin.Reset();
    }

    private Vector2 GetSpawnPoing()
    {
        float x = 0;
        float y = 0;
        while (true)
        {
            x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            y = Random.Range(ySpawnRange.x, ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x,y);
            int numColliders = Physics2D.OverlapCircle(spawnPoint, coinRadius, new ContactFilter2D().NoFilter(), coinBuffer);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }
}
