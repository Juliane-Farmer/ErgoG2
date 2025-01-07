using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    // GameObjects for weather effects
    public GameObject rainEffect;  // Drag your RainEffect prefab here in the Inspector
    public GameObject snowEffect; // Drag your SnowEffect prefab here in the Inspector (optional)

    // The ImageSphere Renderer
    public Renderer sphereRenderer; // Drag the ImageSphere's Renderer here in the Inspector

    // Materials for each weather type
    public Material sunnyMaterial;
    public Material rainyMaterial;
    public Material snowyMaterial;

    // Method to change the weather
    public void ChangeWeather(string weather)
    {
        // Disable all weather effects initially
        if (rainEffect != null) rainEffect.SetActive(false);
        if (snowEffect != null) snowEffect.SetActive(false);

        // Handle weather changes
        switch (weather.ToLower())
        {
            case "sunny":
                sphereRenderer.material = sunnyMaterial; // Set the sphere material to sunny
                Debug.Log("Weather changed to Sunny.");
                break;

            case "rainy":
                sphereRenderer.material = rainyMaterial; // Set the sphere material to rainy
                if (rainEffect != null) rainEffect.SetActive(true); // Enable rain particles
                Debug.Log("Weather changed to Rainy.");
                break;

            case "snowy":
                sphereRenderer.material = snowyMaterial; // Set the sphere material to snowy
                if (snowEffect != null) snowEffect.SetActive(true); // Enable snow particles
                Debug.Log("Weather changed to Snowy.");
                break;

            default:
                Debug.LogWarning("Unknown weather type: " + weather);
                break;
        }
    }

    // Example: Simulate weather at the start of the game
    void Start()
    {
        ChangeWeather("rainy"); // Start with rainy weather for testing
    }
}
