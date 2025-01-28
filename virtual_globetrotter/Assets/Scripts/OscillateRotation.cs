using UnityEngine;

public class OscillateRotation : MonoBehaviour
{
    public float maxRotationAngle = 30f; // Maximum angle on either side
    public float speed = 2f; // Speed of the oscillation

    private float startTime;

    void Start()
    {
        // Record the starting time
        startTime = Time.time;
    }

    void Update()
    {
        // Calculate the oscillation value based on time and speed
        float angle = maxRotationAngle * Mathf.Sin((Time.time - startTime) * speed);

        // Apply the rotation to the GameObject
        transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
}
