using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float mouseSensitivity = 120f;
    public float distance = 4f;
    public float height = 1.5f;

    float xRotation = 15f;
    float yRotation = 0f;

    public LayerMask collisionLayers;
    public float collisionOffset = 0.2f;

    public GameObject questionPanel;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Handle escape key cursor unlock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Re-lock cursor on left click
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Control cursor state based on question panel visibility
        if (questionPanel.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Clamp vertical rotation to prevent extreme angles
        xRotation = Mathf.Clamp(xRotation, 0f, 80f);

        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0f);

        Vector3 pivot = target.position + Vector3.up * height;
        Vector3 desiredPosition = pivot + rotation * Vector3.back * distance;

        // Handle camera collision with environment
        if (Physics.Linecast(pivot, desiredPosition, out RaycastHit hit, collisionLayers))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.LookAt(pivot);
    }
}