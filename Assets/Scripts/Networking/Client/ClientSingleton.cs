using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;

    public ClientGameManager GameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = Object.FindAnyObjectByType<ClientSingleton>();

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

    public async Task<bool> CreateClient()
    {
       GameManager = new ClientGameManager();

       return await GameManager.InitAsync();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
