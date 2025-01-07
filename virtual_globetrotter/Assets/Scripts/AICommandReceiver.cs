using UnityEngine;

public class AICommandReceiver : MonoBehaviour
{
    public EnvironmentManager environmentManager;
    public SeasonSphereManager seasonSphereManager;

    // private void ProcessCommand(string json)
    // {
    //     var command = JsonUtility.FromJson<AICommand>(json);

    //     switch (command.action)
    //     {
    //         case "change_weather":
    //             environmentManager.ChangeWeather(command.weather);
    //             break;

    //         case "change_season":
    //             seasonSphereManager.ChangeSeason(command.season);
    //             break;

    //         default:
    //             Debug.LogError("Unknown action: " + command.action);
    //             break;
    //     }
    // }

    [System.Serializable]
    private class AICommand
    {
        public string action; // e.g., "change_weather" or "change_season"
        public string weather; // e.g., "rain", "snow", "sunny"
        public string season;  // e.g., "autumn", "winter", "spring", "summer"
    }
}
