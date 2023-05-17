using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public GameObject target = null; // The game object to rotate around
    public float rotateSpeed = 1f; // Speed of rotation
    public float distance = 0f; // Distance between camera and target

    public Vector3 offset; // Distance between camera and target
    private float currentRotation = 0f; // Current rotation angle

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        // Calculate the desired rotation angle
        float targetRotation = currentRotation + rotateSpeed * Time.deltaTime;

        // Rotate the camera around the target on the xz-plane
        Vector3 newPosition = target.transform.position + Quaternion.Euler(0f, targetRotation, 0f) * offset;
        transform.position = newPosition;

        // Update the current rotation angle
        currentRotation = targetRotation;

        // Ensure the camera maintains a constant distance from the target
        transform.LookAt(target.transform.position);
        transform.Translate(Vector3.forward * -distance);
    }
}
