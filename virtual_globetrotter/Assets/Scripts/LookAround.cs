using UnityEngine;

public class LookAround : MonoBehaviour
{
    public float sensitivity = 100f; // Mouse sensitivity

    private float xRotation = 0f; // For vertical rotation

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Rotate up and down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical rotation to prevent flipping

        // Apply rotations
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Vertical rotation
        transform.parent.Rotate(Vector3.up * mouseX);                 // Horizontal rotation
    }
}
