using UnityEngine;

public class Move2CheckContinuous : MonoBehaviour
{
    public Transform checkpoint1; // Assign the first checkpoint in the inspector
    public Transform checkpoint2; // Assign the second checkpoint in the inspector
    public float speed = 2f; // Speed of the movement

    private Transform target; // The current target checkpoint

    void Start()
    {
        if (checkpoint1 == null || checkpoint2 == null)
        {
            Debug.LogError("Checkpoints not assigned!");
            enabled = false;
            return;
        }

        // Start moving towards the first checkpoint
        target = checkpoint2;
    }

    void Update()
    {
        // Move towards the target checkpoint
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Switch to the other checkpoint if reached
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            target = target == checkpoint1 ? checkpoint2 : checkpoint1;
        }
    }
}