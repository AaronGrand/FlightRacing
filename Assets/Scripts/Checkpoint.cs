using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Checkpoint : MonoBehaviour
{
    public int id;
    public bool isLast = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        /*if (isLast)
        {
            CheckpointScript.Instance.StopTimer(other.gameObject.GetComponentInParent<FlightController3D>());
        }*/
        CheckpointScript.Instance.NextIndex();
    }
}
