using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public GameObject rainEffect;    // Particle system for rain
    public GameObject snowEffect;    // Particle system for snow
    public Light sun;                // Directional light for day/sunny
    public Material[] seasonalSkyboxes; // Skybox materials for seasons (autumn, winter, spring, summer)

    public void ChangeWeather(string weatherType)
    {
        Debug.Log("Changing weather to: " + weatherType);

        // Toggle weather effects
        rainEffect.SetActive(weatherType == "rain");
        snowEffect.SetActive(weatherType == "snow");

        // Adjust lighting for sunny/cloudy conditions
        sun.enabled = weatherType == "sunny";
    }

    public void ChangeSeason(string seasonType)
    {
        Debug.Log("Changing season to: " + seasonType);

        // Change skybox based on the season
        switch (seasonType.ToLower())
        {
            case "autumn":
                RenderSettings.skybox = seasonalSkyboxes[0];
                break;

            case "winter":
                RenderSettings.skybox = seasonalSkyboxes[1];
                break;

            case "spring":
                RenderSettings.skybox = seasonalSkyboxes[2];
                break;

            case "summer":
                RenderSettings.skybox = seasonalSkyboxes[3];
                break;

            default:
                Debug.LogError("Unknown season: " + seasonType);
                break;
        }

        // Optional: Adjust lighting intensity or color for different seasons
        DynamicGI.UpdateEnvironment();
    }
}
