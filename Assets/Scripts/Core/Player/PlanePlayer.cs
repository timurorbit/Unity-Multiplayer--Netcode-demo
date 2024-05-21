using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlanePlayer : NetworkBehaviour
{
    [Header("References")] [SerializeField]
    private CinemachineVirtualCamera _camera;

    [field: SerializeField] public Health Health { get; private set; }

    [field: SerializeField] public CoinWallet Wallet { get; private set; }

    [SerializeField] private SpriteRenderer minimapIconRenderer;

    [SerializeField] private Texture2D crosshair;


    [Header("Settings")] [SerializeField] private int ownerPriority = 15;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public NetworkVariable<int> TeamIndex = new NetworkVariable<int>();

    public static event Action<PlanePlayer> OnPlayerSpawned;

    public static event Action<PlanePlayer> OnPlayerDespawned;

    [SerializeField] private Color ownerColor;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = null;
            if (IsHost)
            {
                userData =
                    HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }
            else
            {
                userData =
                    ServerSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            }

            PlayerName.Value = userData.userName;
            TeamIndex.Value = userData.teamIndex;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            _camera.Priority = ownerPriority;

            minimapIconRenderer.color = ownerColor;

            Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2f, crosshair.height / 2f), CursorMode.Auto);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}