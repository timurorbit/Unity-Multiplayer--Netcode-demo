using Unity.Netcode;
using UnityEngine;

public class ConnectionButtons : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
