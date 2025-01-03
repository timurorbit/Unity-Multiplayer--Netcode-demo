using System;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;

    [SerializeField] private TMP_Text findMatchButtonText;
    [SerializeField] private TMP_Text timeInQueueText;
    [SerializeField] private TMP_Text queueStatusText;
    [SerializeField] private Toggle teamToggle;
    [SerializeField] private Toggle privateToggle;

    private bool isBusy;

    private bool isMatchmaking;

    private bool isCancelling;

    private float timeInQueue;

    private void Start()
    {
        if (ClientSingleton.Instance == null)
        {
            return;
        }

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        queueStatusText.text = string.Empty;
        timeInQueueText.text = string.Empty;
    }

    private void Update()
    {
        if (isMatchmaking)
        {
            timeInQueue += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeInQueue);
            timeInQueueText.text = string.Format("{0:0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }

    public async void FindMatchPressed()
    {
        if (isCancelling)
        {
            return;
        }

        if (isMatchmaking)
        {
            queueStatusText.text = "Cancelling...";
            isCancelling = true;

            await ClientSingleton.Instance.GameManager.CancelMatchmaking();

            isCancelling = false;
            isMatchmaking = false;
            isBusy = false;
            findMatchButtonText.text = "Find Match";
            queueStatusText.text = string.Empty;
            timeInQueueText.text = string.Empty;
            return;
        }

        if (isBusy)
        {
            return;
        }

        ClientSingleton.Instance.GameManager.MatchmakeAsync(teamToggle.isOn, OnMatchMade);
        findMatchButtonText.text = "Cancel";
        queueStatusText.text = "Searching...";
        timeInQueue = 0f;

        isBusy = true;
        isMatchmaking = true;
    }

    private void OnMatchMade(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success:
                queueStatusText.text = "Connecting...";
                break;
            case MatchmakerPollingResult.TicketCreationError:
                queueStatusText.text = "TicketCreationError";
                break;
            case MatchmakerPollingResult.TicketCancellationError:
                queueStatusText.text = "TicketCancellationError";
                break;
            case MatchmakerPollingResult.TicketRetrievalError:
                queueStatusText.text = "TicketRetrievalError";
                break;
            case MatchmakerPollingResult.MatchAssignmentError:
                queueStatusText.text = "MatchAssignmentError";
                break;
        }
    }

    public async void StartHost()
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;

        await HostSingleton.Instance.GameManager.StartHostAsync(privateToggle.isOn);

        isBusy = false;
    }

    public async void StartClient()
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;

        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);

        isBusy = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

        isBusy = false;
    }
}