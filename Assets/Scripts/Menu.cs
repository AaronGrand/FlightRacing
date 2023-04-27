using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Menu : NetworkBehaviour
{
    [SerializeField] private GameObject escape;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private CheckpointScript checkPointScript;

    private float time;
    private bool gameFinished;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escape.SetActive(!escape.activeSelf);
        }
        if (GameState.Instance.GetState() == STATE.INGAME)
        {
            timer.text = checkPointScript.timer.ToString();
        }
    }
    public void Leave()
    {
        //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.Shutdown();
        Application.Quit();
    }
}
