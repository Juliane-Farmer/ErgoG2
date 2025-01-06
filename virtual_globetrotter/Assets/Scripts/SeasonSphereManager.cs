using UnityEngine;

public class SeasonSphereManager : MonoBehaviour
{
    public Renderer sphereRenderer; 
    public Material autumnMaterial;
    public Material winterMaterial;
    public Material springMaterial;
    public Material summerMaterial;

    public void ChangeSeason(string season)
    {
        Debug.Log("Changing season to: " + season);

        switch (season.ToLower())
        {
            case "autumn":
                sphereRenderer.material = autumnMaterial;
                break;

            case "winter":
                sphereRenderer.material = winterMaterial;
                break;

            case "spring":
                sphereRenderer.material = springMaterial;
                break;

            case "summer":
                sphereRenderer.material = summerMaterial;
                break;

            default:
                Debug.LogError("Unknown season: " + season);
                break;
        }
    }
}
