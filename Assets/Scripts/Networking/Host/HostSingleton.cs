using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;

    public HostGameManager GameManager { get; private set; } 

    public static HostSingleton Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = Object.FindAnyObjectByType<HostSingleton>();

            if (instance == null)
            {
                return null;
            }

            return instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost(NetworkObject playerPrefab)
    {
       GameManager = new HostGameManager(playerPrefab);
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
