using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField] List<GameObject> checkpoints;

    [SerializeField] private Menu menu;

    private int currentIndex;

    public bool gameIsFinished;
    public bool gameStarted;

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
        if (GameState.Instance.GetState() == STATE.INGAME && !GameState.Instance.localGameFinished)
        {
            timer = timer + Time.deltaTime;
        }
    }

    public void NextIndex(FlightController3D flightController)
    {
        checkpoints[currentIndex].SetActive(false);
        currentIndex++;
        //GAME FINISHED
        if (currentIndex == checkpoints.Count)
        {
            flightController.EndGame();

            Debug.Log("Game Finished");

            menu.SetPlayer(timer, flightController.userName.Value);
        } else if(GameState.Instance.GetState() != STATE.ENDED)
        {
            checkpoints[currentIndex].SetActive(true);
        }
    }

    public bool CheckIfCurrentIndex(int id)
    {
        return currentIndex == id;
    }
}
