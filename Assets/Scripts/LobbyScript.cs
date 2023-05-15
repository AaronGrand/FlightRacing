using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LobbyScript : NetworkBehaviour
{
    [SerializeField] private string nextSceneName = "Game";

    private void Start()
    {
        this.gameObject.SetActive(false);
        if (IsHost)
        {
            this.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        GameState.Instance.SetState(STATE.NOT_STARTED);
        NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
}
