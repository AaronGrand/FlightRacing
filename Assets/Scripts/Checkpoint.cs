using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Checkpoint : MonoBehaviour
{
    public int id;
    public bool isLast = false;

    private void OnTriggerEnter(Collider other)
    {
        FlightController3D flightController = other.gameObject.GetComponentInParent<FlightController3D>();

        //correct checkpoint
        if (flightController.IsLocalPlayer && CheckpointScript.Instance.CheckIfCurrentIndex(id))
        {
            CheckpointScript.Instance.NextIndex(flightController);
        }
        //not the right checkpoint
        //not used, since checkpoints are not visible, but prevents errors
        else
        {
            return;
        }
    }
}
