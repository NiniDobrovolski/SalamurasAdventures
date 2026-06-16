using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private float _followSpeed = 8f;

    private Vector3 _offset;

    void Start()
    {
        // Calculate initial offset between camera and player
        _offset = transform.position - _player.position;
    }

    private void LateUpdate()
    {
        // Target position based on player position + initial offset
        Vector3 targetPosition = _player.position + _offset;

        // Smoothly move camera toward target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);
    }
}