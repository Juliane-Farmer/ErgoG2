using UnityEngine;

public class Animation1 : MonoBehaviour
{
    public Transform centerObject; // Assign the empty GameObject in the Inspector
    public float rotationSpeed = -10f;

    void Update()
    {
        if (centerObject != null)
        {
            // Rotate the parent object
            centerObject.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}