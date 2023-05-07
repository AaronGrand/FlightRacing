using Unity.Netcode;
using Unity.Collections;

public class GameState : NetworkBehaviour
{
    public NetworkVariable<STATE> gameState = new NetworkVariable<STATE>(STATE.LOBBY, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public FixedString32Bytes userName;

    public bool localGameFinished = false;

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

    public STATE GetState()
    {
        return gameState.Value;
    }

    public void SetState(STATE state)
    {
        if (IsHost)
        {
            gameState.Value = state;
        } else
        {
            SetStateServerRpc(state);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateServerRpc(STATE state)
    {
        gameState.Value = state;
    }
}

public enum STATE
{
    LOBBY,
    NOT_STARTED,
    TIMER,
    INGAME,
    ENDED
}
