using Unity.Mathematics;
using UnityEngine;

public class MoveWithDelay : MonoBehaviour
{
    public GameObject pointA; // Starting point (GameObject A)
    public GameObject pointB; // Destination point (GameObject B)
    public float speed = 1f; // Speed of movement
    private AudioSource audioSource; // Audio source component

    private bool isMoving = false; // Flag to track movement state

    private void Awake()
    {
        // Get or attach the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Ensure the audio source is disabled initially
        audioSource.enabled = false;
    }

    private void OnEnable()
    {
        // Start the coroutine when the script is enabled
        transform.position = pointA.transform.position;
        audioSource.enabled = false;
        StartCoroutine(WaitAndMove());
    }

    private System.Collections.IEnumerator WaitAndMove()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        // Enable the audio source and start playing
        audioSource.enabled = true;
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }

        // Set the movement flag to true
        isMoving = true;
    }

    private void Update()
    {
        // If the script is in moving state, move the GameObject
        if (isMoving && pointA != null && pointB != null)
        {
            // Move towards point B from point A
            transform.position = Vector3.MoveTowards(transform.position, pointB.transform.position, speed * Time.deltaTime);

            // Stop moving if the object reaches point B
            if (Vector3.Distance(transform.position, pointB.transform.position) < 0.01f)
            {
                audioSource.enabled = false;
                isMoving = false;
            }
        }
    }
}