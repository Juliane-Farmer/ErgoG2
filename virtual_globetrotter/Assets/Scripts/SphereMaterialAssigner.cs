using UnityEngine;
using UnityEngine.SceneManagement;

public class SphereMaterialAssigner : MonoBehaviour
{
    public Material egyptSunnyMaterial;
    public Material egyptRainyMaterial;
    public Material egyptSnowyMaterial;

    public Material eiffelSunnyMaterial;
    public Material eiffelRainyMaterial;
    public Material eiffelSnowyMaterial;

    public GameObject rainEffect;
    public GameObject snowEffect;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string weather = GetWeatherFromAI();

        GameObject imageSphere = GameObject.Find("ImageSphere");
        if (imageSphere != null)
        {
            Renderer sphereRenderer = imageSphere.GetComponent<Renderer>();

            switch (sceneName)
            {
                case "EgyptScene":
                    AssignWeatherMaterial(sphereRenderer, weather, egyptSunnyMaterial, egyptRainyMaterial, egyptSnowyMaterial);
                    break;

                case "EiffelTowerScene":
                    AssignWeatherMaterial(sphereRenderer, weather, eiffelSunnyMaterial, eiffelRainyMaterial, eiffelSnowyMaterial);
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
        return "sunny"; 
    }
}
