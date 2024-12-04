using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 3, 0); 
    public float smoothSpeed = 0.2f;

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate the desired position
        Vector3 desiredPosition = player.position + offset;

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Set the camera's position to the smoothed position
        transform.position = smoothedPosition;

        // Ensure the camera looks at the player
        transform.LookAt(player);
    }
}
