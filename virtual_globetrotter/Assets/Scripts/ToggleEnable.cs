using UnityEngine;

public class ToggleEnable : MonoBehaviour
{
    // The component to toggle (can be any MonoBehaviour-derived component)
    public MonoBehaviour targetComponent;

    // Time interval for toggling (in seconds)
    public float toggleInterval = 4.0f;

    private void Start()
    {
        // Start the repeating toggle function
        if (targetComponent != null)
        {
            InvokeRepeating(nameof(ToggleComponent), 0f, toggleInterval);
        }
    }

    // Function to toggle the enabled state of the target component
    private void ToggleComponent()
    {
        if (targetComponent != null)
        {
            targetComponent.enabled = !targetComponent.enabled;
        }
    }

    private void OnDestroy()
    {
        // Stop the toggling when the object is destroyed
        CancelInvoke(nameof(ToggleComponent));
    }
}