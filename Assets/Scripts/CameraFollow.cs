using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraFollow : NetworkBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        StartCoroutine(FindLocalPlayer());
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Calculate the desired position and rotation of the camera
        Vector3 targetPosition = target.position + target.TransformDirection(offset);
        Quaternion targetRotation = Quaternion.LookRotation(target.position - targetPosition, target.up);

        // Smoothly interpolate the camera towards the desired position and rotation
        Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, targetPosition, ref velocity, smoothTime);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, targetRotation, smoothTime);
    }

    private IEnumerator FindLocalPlayer()
    {
        while (target == null)
        {
            foreach (NetworkObject identity in FindObjectsOfType<NetworkObject>())
            {
                if (identity.IsLocalPlayer)
                {
                    target = identity.transform;
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
