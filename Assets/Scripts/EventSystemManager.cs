using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager : MonoBehaviour
{
    private void Awake()
    {
        // Find all EventSystems in the scene
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

        // If there is more than one EventSystem
        if (eventSystems.Length > 1)
        {
            // Destroy all other EventSystems except the first one
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
        }

        // Make sure the first EventSystem persists across scenes
        DontDestroyOnLoad(eventSystems[0].gameObject);
    }
}
