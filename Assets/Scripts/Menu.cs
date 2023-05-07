using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class Menu : NetworkBehaviour
{
    #region Variables

    [Header("JoinCode")]
    [SerializeField] TextMeshProUGUI joinCode;

    [Header("Timer")]
    [SerializeField] private GameObject escape;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private CheckpointScript checkPointScript;


    [Header("Scoreboard")]
    [SerializeField] private GameObject scoreBoard;
    [SerializeField] private GameObject playerScorePrefab;

    //public NetworkVariable<PlayerStats> playerScores = new NetworkVariable<PlayerStats>();

    public Queue<PlayerStats> playerScores = new Queue<PlayerStats>();

    [System.Serializable]
    public struct PlayerStats
    {
        public float time;
        public FixedString32Bytes name;

        public PlayerStats(float time, FixedString32Bytes name)
        {
            this.time = time;
            this.name = name;
        }
    }

    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    private float countdownTime = 3.0f;
    private float timeRemaining;

    #endregion

    #region Unity Methods

    private void Start()
    {
        joinCode.text = HostManager.Instance.joinCode;

        timeRemaining = countdownTime;

        escape.SetActive(false);
        scoreBoard.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escape.SetActive(!escape.activeSelf);
        }

        //TIMER
        if(GameState.Instance.GetState() == STATE.TIMER)
        {
            timeRemaining -= Time.deltaTime;

            int seconds = Mathf.CeilToInt(timeRemaining);

            string secondsString = seconds.ToString("0");

            countdownText.text = secondsString;

            if (timeRemaining <= 0f)
            {
                if (IsHost)
                {
                    GameState.Instance.SetState(STATE.INGAME);
                }
                countdownText.text = "";
            }
        }

        //AFTER TIMER
        if (GameState.Instance.GetState() != STATE.TIMER)
        {
            countdownText.text = "";
        }
        
        //INGAME AND LOCAL PLAYER INGAME
        if (GameState.Instance.GetState() == STATE.INGAME && !GameState.Instance.localGameFinished)
        {
            timer.text = checkPointScript.timer.ToString("0.00");
        }
    }

    #endregion

    #region public methods

    public void SetPlayer(float time, FixedString32Bytes name)
    {
        //host finishes
        if (IsHost)
        {
            timer.text = "";
            GameObject player = Instantiate(playerScorePrefab, scoreBoard.transform);
            player.GetComponent<ScoreBoardPlayer>().SetPlayer(new PlayerStats(time, name));
            SetPlayerClientRpc(time, name);
        }
        //client finishes
        if(IsClient && !IsHost)
        {
            timer.text = "";
            SetPlayerServerRpc(time, name);
        }

        //UpdateScoreBoard();
        scoreBoard.SetActive(true);
    }

    #endregion

    #region RPC

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerServerRpc(float time, FixedString32Bytes name)
    {
        GameObject player = Instantiate(playerScorePrefab, scoreBoard.transform);
        player.GetComponent<ScoreBoardPlayer>().SetPlayer(new PlayerStats(time, name));

        SetPlayerClientRpc(time, name);
    }

    [ClientRpc]
    private void SetPlayerClientRpc(float time, FixedString32Bytes name)
    {
        if (!IsHost)
        {
            timer.text = "";
            GameObject player = Instantiate(playerScorePrefab, scoreBoard.transform);
            player.GetComponent<ScoreBoardPlayer>().SetPlayer(new PlayerStats(time, name));
        }
    }
    /*
    private void UpdateScoreBoard()
    {
        timer.text = "";
        foreach (Transform child in scoreBoard.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for(int i = 0; i < playerScores.Count; i++)
        {
            GameObject player = Instantiate(playerScorePrefab, scoreBoard.transform);
            player.GetComponent<ScoreBoardPlayer>().SetPlayer(playerScores.Dequeue());
        }
    }*/

    #endregion

    #region Buttons
    public void Leave()
    {
        NetworkManager.Singleton.Shutdown();
        Application.Quit();
    }

    #endregion
}
