using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : NetworkBehaviour
{
    [SerializeField] private TMP_Text lobbyCodeText;

    private NetworkVariable<FixedString32Bytes> lobbyCode = new NetworkVariable<FixedString32Bytes>("");
    public void LeaveGame()
    {

        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Shutdown();
        }

        ClientSingleton.Instance.GameManager.Disconnect();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyCode.OnValueChanged += UpdateLobbyCode; 
            UpdateLobbyCode("", lobbyCode.Value);
        }
        if (!IsHost)
        {
            return;
        }
        lobbyCode.Value = HostSingleton.Instance.GameManager.JoinCode;
    }

    public override void OnNetworkDespawn()
    {
        lobbyCode.OnValueChanged -= UpdateLobbyCode;

    }

    private void UpdateLobbyCode(FixedString32Bytes oldCode, FixedString32Bytes newCode)
    {
        lobbyCodeText.text = newCode.Value;
    }
}
