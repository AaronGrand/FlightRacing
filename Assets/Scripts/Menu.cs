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

    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    private float countdownTime = 3.0f;
    private float timeRemaining;


    private float time;
    private bool gameFinished;
    private bool timerStarted = false;

    #region Unity Methods
    private void Start()
    {
        timeRemaining = countdownTime;
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

    #region Buttons
    public void Leave()
    {
        //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.Shutdown();
        Application.Quit();
    }
    #endregion
}
