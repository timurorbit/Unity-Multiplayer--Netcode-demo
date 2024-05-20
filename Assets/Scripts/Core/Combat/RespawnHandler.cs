using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private PlanePlayer playerPrefab;

    [SerializeField] private float keptCoinPercentage;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        PlanePlayer[] players = FindObjectsByType<PlanePlayer>(FindObjectsSortMode.None);
        foreach (PlanePlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        PlanePlayer.OnPlayerSpawned += HandlePlayerSpawned;
        PlanePlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }
        PlanePlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        PlanePlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }
    private void HandlePlayerSpawned(PlanePlayer player)
    {
        player.Health.onDie += (Health) => HandlePlayerDie(player);
    }
    private void HandlePlayerDespawned(PlanePlayer player)
    {
        player.Health.onDie -= (Health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(PlanePlayer player)
    {
        int coinsRemaining = (int) (player.Wallet.totalCoins.Value * (keptCoinPercentage / 100f));
        
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, coinsRemaining));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int coinsRemaining)
    {
        yield return null;

        PlanePlayer playerInstance = Instantiate(
            playerPrefab, SpawnPoint.GetRandomSpawnPos(),
            Quaternion.identity);

        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
        
        playerInstance.Wallet.totalCoins.Value += coinsRemaining;
    }
}
