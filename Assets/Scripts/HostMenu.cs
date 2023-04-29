using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HostMenu : NetworkBehaviour
{

    private void Start()
    {
        if (!IsHost)
        {
            gameObject.SetActive(false);
        }
    }


    public void StartGame()
    {
        GameState.Instance.SetState(STATE.TIMER);
        gameObject.SetActive(false);
    }

}
