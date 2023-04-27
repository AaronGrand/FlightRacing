using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField] List<GameObject> checkpoints;

    private int currentIndex;

    public bool gameIsFinished;
    public bool gameStarted;

    //public NetworkVariable<float> endTime = new NetworkVariable<float>(0f);
    //public float endTime;
    public float timer;

    public static CheckpointScript Instance { get; private set; }

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

    private void Start()
    {
        // Disable all checkpoints except the first one
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].SetActive(false);
            checkpoints[i].GetComponent<Checkpoint>().id = i;
        }

        checkpoints[checkpoints.Count-1].GetComponent<Checkpoint>().isLast = true;

        checkpoints[0].SetActive(true);
        currentIndex = 0;
    }

    private void Update()
    {
        if (GameState.Instance.GetState() == STATE.INGAME)
        {
            timer = timer + Time.deltaTime;
        }
    }

    public void NextIndex()
    {
        checkpoints[currentIndex].SetActive(false);
        currentIndex++;
        if (currentIndex == checkpoints.Count)
        {
            GameState.Instance.SetState(STATE.ENDED);
            Debug.Log("Game Finished");
        } else if(GameState.Instance.GetState() != STATE.ENDED)
        {
            checkpoints[currentIndex].SetActive(true);
        }
    }
    /*
    public void StartTimer()
    {
        gameStarted = true;
    }

    public void StopTimer(FlightController3D flightController3D)
    {
        if (!gameIsFinished)
        {
            gameIsFinished = true;
            //endTime.Value = timer;
            stopGameClientRpc();
            //endTime = timer;
        }
    }

    [ClientRpc]
    private void stopGameClientRpc()
    {
        endTime.Value = timer;
    }*/
}
