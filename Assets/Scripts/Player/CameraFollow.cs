using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    Transform target;            // The position that that camera will be following.
    public float Smoothing = 5f;        // The speed with which the camera will be following.
    public bool playerRendered = false;
    Vector3 offset;                     // The initial offset from the target.

    void Start()
    {
    }

    public void SetOffset(Transform playerPos)
    {
        // Calculate the initial offset.
        target = playerPos;
        offset = transform.position - target.position;
        playerRendered = true;
    }

    void FixedUpdate()
    {
        if (playerRendered)
        {
            // Create a postion the camera is aiming for based on the offset from the target.
            Vector3 targetCamPos = target.position + offset;
            // Smoothly interpolate between the camera's current position and it's target position.
            transform.position = Vector3.Lerp(transform.position, targetCamPos, Smoothing * Time.deltaTime);
        }
    }
}
