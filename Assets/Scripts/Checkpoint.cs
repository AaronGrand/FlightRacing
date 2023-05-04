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

        if (flightController.IsLocalPlayer)
        {
            CheckpointScript.Instance.NextIndex(flightController);
        }
    }
}
