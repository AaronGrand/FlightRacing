using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using JetBrains.Annotations;
using System.Linq;
using static Menu;

public class Menu : NetworkBehaviour
{
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

            } else if(GameState.Instance.GetState() == STATE.INGAME)
            {
                countdownText.text = "";
            }
        }
        if (GameState.Instance.GetState() == STATE.INGAME)
        {
            timer.text = checkPointScript.timer.ToString("0.00");
        }
    }

    #endregion

    #region private methods

    #endregion

    #region public methods

    public void SetPlayer(float time, FixedString32Bytes name)
    {


        //host wins
        if (IsHost)
        {
            timer.text = "";
            GameObject player = Instantiate(playerScorePrefab, scoreBoard.transform);
            player.GetComponent<ScoreBoardPlayer>().SetPlayer(new PlayerStats(time, name));
            SetPlayerClientRpc(time, name);
        }
        //client wins
        if(IsClient && !IsHost)
        {
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
            scoreBoard.SetActive(true);
        }
    }

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
        /*
        foreach(PlayerStats playerStats in playerScores)
        {
            Debug.Log(playerStats.name);
            GameObject player = Instantiate(playerScorePrefab, scoreBoard.transform);
            player.GetComponent<ScoreBoardPlayer>().SetPlayer(playerStats);
        }*/
    }

    #endregion

    #region Buttons
    public void Leave()
    {
        NetworkManager.Singleton.Shutdown();
        Application.Quit();
    }

    #endregion
}
