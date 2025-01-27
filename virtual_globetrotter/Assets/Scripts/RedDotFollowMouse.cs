using UnityEngine;

public class RedDotFollowMouse : MonoBehaviour
{
    public GameObject redDot; // Assign your red sphere here
    public float distanceFromCamera = 5f; // Distance from the camera
    public Camera mainCamera;

    void Update()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        Cursor.visible = false;

        // Convert the mouse position to a ray from the camera
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        // Calculate the position of the red dot
        Vector3 targetPosition = ray.origin + ray.direction * distanceFromCamera;

        // Move the red dot to the calculated position
        redDot.transform.position = targetPosition;
    }
}