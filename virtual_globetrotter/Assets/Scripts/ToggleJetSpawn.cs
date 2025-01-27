using UnityEngine;

public class ToggleJetSpawn : MonoBehaviour
{
    [Tooltip("The GameObject to toggle. If left empty, this script will toggle the GameObject it is attached to.")]
    public GameObject targetObject;

    [Tooltip("Time interval in seconds for toggling the GameObject on and off.")]
    public float timeSeconds = 1f;

    private bool isActive = true;

    private void OnEnable()
    {
        // Use the attached GameObject if no target is assigned
        if (targetObject == null)
        {
            targetObject = gameObject;
        }

        // Start the toggle coroutine
        StartCoroutine(ToggleCoroutine());
    }

    private System.Collections.IEnumerator ToggleCoroutine()
    {
        while (true)
        {
            if (targetObject.activeSelf == false)
            {
                targetObject.SetActive(true);
            }
            yield return new WaitForSeconds(timeSeconds);

            targetObject.SetActive(false);
        }
    }
}