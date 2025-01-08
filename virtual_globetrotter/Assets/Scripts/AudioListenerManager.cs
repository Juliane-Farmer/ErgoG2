using UnityEngine;

public class AudioListenerManager : MonoBehaviour
{
    void Awake()
    {
        // Check for other Audio Listeners in the scene
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();

        // If there's more than one, disable this one
        if (listeners.Length > 1)
        {
            GetComponent<AudioListener>().enabled = false;
        }
    }
}
