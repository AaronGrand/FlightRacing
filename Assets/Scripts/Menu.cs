using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Menu : NetworkBehaviour
{
    [SerializeField] private GameObject escape;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escape.SetActive(!escape.activeSelf);
        }
    }
    public void Leave()
    {
        NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
        Application.Quit();

    }
}
