using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraFollow : MonoBehaviour
{
    public Transform player;        // Reference to the player transform
    public float smoothSpeed = 5f;  // Smooth follow speed
    public Vector3 offset;

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        // Smoothly follow only the player's Y position with an offset
        pos.y = Mathf.Lerp(
            transform.position.y,
            player.position.y + offset.y,
            smoothSpeed * Time.deltaTime
        );

        transform.position = pos;
    }
}