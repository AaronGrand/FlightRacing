using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField] List<GameObject> checkpoints;

    private int currentIndex;

    public bool gameIsFinished;

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

        checkpoints[0].SetActive(true);
        currentIndex = 0;
    }

    public void NextIndex()
    {
        checkpoints[currentIndex].SetActive(false);
        currentIndex++;
        if (currentIndex == checkpoints.Count)
        {
            gameIsFinished = true;
            Debug.Log("Game Finished");
        } else if(!gameIsFinished)
        {
            checkpoints[currentIndex].SetActive(true);
        }
    }


}
