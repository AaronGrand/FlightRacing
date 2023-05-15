using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LobbyCameraBehaviour : NetworkBehaviour
{
    public Transform target = null; // The game object to rotate around
    public float rotateSpeed = 1f; // Speed of rotation
    private float distance = 0f; // Distance between camera and target

    private Vector3 offset; // Distance between camera and target
    private float currentRotation = 0f; // Current rotation angle

    private void Start()
    {
        StartCoroutine(FindLocalPlayer(0.5f));
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        // Calculate the desired rotation angle
        float targetRotation = currentRotation + rotateSpeed * Time.deltaTime;

        // Rotate the camera around the target on the xz-plane
        Vector3 newPosition = target.position + Quaternion.Euler(0f, targetRotation, 0f) * offset;
        transform.position = newPosition;

        // Update the current rotation angle
        currentRotation = targetRotation;

        // Ensure the camera maintains a constant distance from the target
        transform.LookAt(target.position);
        transform.Translate(Vector3.forward * -distance);
    }

    private IEnumerator FindLocalPlayer(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        foreach (NetworkObject identity in FindObjectsOfType<NetworkObject>())
        {
            if (identity.IsLocalPlayer)
            {
                target = identity.transform;
                offset = transform.position - target.position;
                break;
            }
        }
    }
}
