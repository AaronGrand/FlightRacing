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
        GameState.Instance.SetState(STATE.NOT_STARTED);
        NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);

        /*
        if (IsHost)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                Debug.Log(player.name);
                player.GetComponent<FlightController3D>().SetSpawnLocation(1f);
            }
        }*/

    }
}
