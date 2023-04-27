using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameState : NetworkBehaviour
{
    public NetworkVariable<STATE> gameState = new NetworkVariable<STATE>(STATE.NOT_STARTED, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public static GameState Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        Debug.Log("GameState: " + gameState.Value.ToString());
    }

    public STATE GetState()
    {
        return gameState.Value;
    }

    public void SetState(STATE state)
    {
        gameState.Value = state;
    }
}

public enum STATE
{
    NOT_STARTED,
    INGAME,
    ENDED
}
