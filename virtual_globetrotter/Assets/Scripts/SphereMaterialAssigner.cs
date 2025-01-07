using UnityEngine;
using UnityEngine.SceneManagement;

public class SphereMaterialAssigner : MonoBehaviour
{
    // Egypt Materials
    public Material egyptSunnyMaterial;
    public Material egyptRainyMaterial;
    public Material egyptSnowyMaterial;

    // San Francisco Materials
    public Material sanFranciscoSunnyMaterial;
    public Material sanFranciscoRainyMaterial;
    public Material sanFranciscoSnowyMaterial;

    // Twin Towers Materials
    public Material twinTowersSunnyMaterial;
    public Material twinTowersRainyMaterial;
    public Material twinTowersSnowyMaterial;

    // Eiffel Tower Materials
    public Material eiffelSunnyMaterial;
    public Material eiffelRainyMaterial;
    public Material eiffelSnowyMaterial;

    // Times Square Materials
    public Material timeSquareSunnyMaterial;
    public Material timeSquareRainyMaterial;
    public Material timeSquareSnowyMaterial;

    // Weather effects
    public GameObject rainEffect;
    public GameObject snowEffect;

    void Start()
    {
        // Get the current scene name and weather
        string sceneName = SceneManager.GetActiveScene().name;
        string weather = GetWeatherFromAI();

        // Find the ImageSphere GameObject
        GameObject imageSphere = GameObject.Find("ImageSphere");
        if (imageSphere != null)
        {
            Renderer sphereRenderer = imageSphere.GetComponent<Renderer>();

            // Assign materials based on the scene and weather
            switch (sceneName)
            {
                case "EgyptSunnyScene":
                    AssignWeatherMaterial(sphereRenderer, weather, egyptSunnyMaterial, egyptRainyMaterial, egyptSnowyMaterial);
                    break;

                case "SanFranciscoSunnyScene":
                    AssignWeatherMaterial(sphereRenderer, weather, sanFranciscoSunnyMaterial, sanFranciscoRainyMaterial, sanFranciscoSnowyMaterial);
                    break;

                case "SanFranciscoRainScene":
                    AssignWeatherMaterial(sphereRenderer, weather, sanFranciscoSunnyMaterial, sanFranciscoRainyMaterial, sanFranciscoSnowyMaterial);
                    break;

                case "TwinTowersScene":
                    AssignWeatherMaterial(sphereRenderer, weather, twinTowersSunnyMaterial, twinTowersRainyMaterial, twinTowersSnowyMaterial);
                    break;

                case "EiffelTowerSunnyScene":
                    AssignWeatherMaterial(sphereRenderer, weather, eiffelSunnyMaterial, eiffelRainyMaterial, eiffelSnowyMaterial);
                    break;

                case "EiffelTowerSnowyScene":
                    AssignWeatherMaterial(sphereRenderer, weather, eiffelSunnyMaterial, eiffelRainyMaterial, eiffelSnowyMaterial);
                    break;

                case "TimesSquareSunnyScene":
                    AssignWeatherMaterial(sphereRenderer, weather, timeSquareSunnyMaterial, timeSquareRainyMaterial, timeSquareSnowyMaterial);
                    break;

                default:
                    Debug.LogWarning("No materials assigned for this scene: " + sceneName);
                    break;
            }
        }
        else
        {
            Debug.LogError("ImageSphere not found in the scene!");
        }
    }

    void AssignWeatherMaterial(Renderer sphereRenderer, string weather, Material sunny, Material rainy, Material snowy)
    {
        // Assign the correct material and activate/deactivate weather effects
        switch (weather)
        {
            case "sunny":
                sphereRenderer.material = sunny;
                rainEffect.SetActive(false);
                snowEffect.SetActive(false);
                break;

            case "rainy":
                sphereRenderer.material = rainy;
                rainEffect.SetActive(true);
                snowEffect.SetActive(false);
                break;

            case "snowy":
                sphereRenderer.material = snowy;
                rainEffect.SetActive(false);
                snowEffect.SetActive(true);
                break;

            default:
                Debug.LogWarning("Unknown weather type: " + weather);
                break;
        }
    }

    string GetWeatherFromAI()
    {
        // Placeholder for AI integration: Replace this with real weather data from AI or other sources
        return "sunny"; // Example: default to "sunny"
    }
}
