using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Checkpoint : MonoBehaviour
{
    public int id;

    public Checkpoint(int id)
    {
        this.id = id;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        CheckpointScript.Instance.NextIndex();
    }
}
