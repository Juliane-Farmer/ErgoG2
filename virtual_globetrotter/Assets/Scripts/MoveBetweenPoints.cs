using UnityEngine;

public class MoveToCheckpoints : MonoBehaviour
{
    public Transform checkpoint1; // Assign the first checkpoint in the Inspector
    public Transform checkpoint2; // Assign the second checkpoint in the Inspector
    public float speed = 1f; // Speed of movement

    private bool reachedCheckpoint1 = false; // Tracks if the GameObject has reached checkpoint1

    void Update()
    {
        if (reachedCheckpoint1 || checkpoint1 == null)
        {
            return; // Stop moving once checkpoint1 is reached or if no checkpoint1 is assigned
        }

        // Move towards checkpoint1
        float step = speed * Time.deltaTime; // Calculate step size based on speed and time
        transform.position = Vector3.MoveTowards(transform.position, checkpoint1.position, step);

        // Check if the GameObject has reached checkpoint1
        if (Vector3.Distance(transform.position, checkpoint1.position) < 0.01f)
        {
            reachedCheckpoint1 = true; // Stop further movement
        }
    }
    void OnEnable()
    {
        reachedCheckpoint1 = false;
    }

    void OnDisable()
    {
        if (checkpoint2 != null)
        {
            // Teleport to checkpoint2
            transform.position = checkpoint2.position;
        }
    }
}