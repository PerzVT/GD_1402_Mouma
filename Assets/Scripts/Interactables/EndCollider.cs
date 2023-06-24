using UnityEngine;

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
