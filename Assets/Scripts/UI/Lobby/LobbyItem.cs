using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    
    [SerializeField] private TMP_Text lobbyPlayersText;

    private LobbiesList lobbiesList;
    private Lobby lobby;

    public void Initialize(LobbiesList _lobbiesList, Lobby _lobby)
    {
        this.lobbiesList = _lobbiesList;
        this.lobby = _lobby;
        
        lobbyNameText.text = _lobby.Name;
        lobbyPlayersText.text = $"{_lobby.Players.Count}/{_lobby.MaxPlayers}";
    }

    public void Join()
    {
        lobbiesList.JoinAsync(lobby);
    }
}
