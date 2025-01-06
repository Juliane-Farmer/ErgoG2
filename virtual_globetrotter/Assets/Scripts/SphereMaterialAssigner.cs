using UnityEngine;
using UnityEngine.SceneManagement;

public class SphereMaterialAssigner : MonoBehaviour
{
    public Material egyptMaterial;
    public Material eiffelTowerMaterial;
    public Material sanFranciscoMaterial;
    public Material timesquareMaterial;
    public Material twintowersMaterial;


    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        GameObject imageSphere = GameObject.Find("ImageSphere");
        if (imageSphere != null)
        {
            Renderer sphereRenderer = imageSphere.GetComponent<Renderer>();

            switch (sceneName)
            {
                case "EgyptScene":
                    sphereRenderer.material = egyptMaterial;
                    break;
                case "EiffelTowerScene":
                    sphereRenderer.material = eiffelTowerMaterial;
                    break;
                case "TimeSquareScene":
                    sphereRenderer.material = timesquareMaterial;
                    break;
                case "TwinTowersScene":
                    sphereRenderer.material = twintowersMaterial;
                    break;
                case "SanFranciscoScene":
                    sphereRenderer.material = sanFranciscoMaterial;
                    break;

                default:
                    Debug.LogWarning("No material assigned for this scene: " + sceneName);
                    break;
            }
        }
        else
        {
            Debug.LogError("ImageSphere not found in the scene!");
        }
    }
}
