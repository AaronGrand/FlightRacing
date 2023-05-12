using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LobbyScript : NetworkBehaviour
{
    [SerializeField] private string nextSceneName = "Game";

    public void StartGame()
    {
        this.gameObject.SetActive(false);

        GameState.Instance.SetState(STATE.NOT_STARTED);
        NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);

        if (IsHost)
        {
            this.gameObject.SetActive(true);
        }
    }
}
