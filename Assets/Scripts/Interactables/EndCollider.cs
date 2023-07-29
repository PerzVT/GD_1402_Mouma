using UnityEngine;
/// <summary>
/// consider making an Interactable class that these inherit from that may do something rather than just have them in a separate folder (though having them in a separate folder is good as well).You will want to look into Unity Events for handling what happens with interactables and upon what they do. You will want to create events that function similarly to UI Events (like, for example, where you can subscribe to a button press and do something when it happens).  https://docs.unity3d.com/2021.3/Documentation/Manual/UnityEvents.html
/// </summary>
// Represents a trigger collider that ends the level or restarts the game when the player enters it
public class EndCollider : MonoBehaviour
{
    [SerializeField] private PlayerController player; // Reference to the PlayerController script
    
    // Called when the EndCollider's trigger collider overlaps with another collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the overlapping object has the "Player" tag
        if (other.gameObject.CompareTag("Player"))
        {
            // Restart the game after 3 seconds
            RestartGameAfterDelay(3);
        }
    }

    // Restart the game after the specified delay in seconds
    private void RestartGameAfterDelay(int delayInSeconds)
    {
        StartCoroutine(player.Restart(delayInSeconds));
    }
}
