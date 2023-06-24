using UnityEngine;

// Represents a switch that destroys a door when the player enters its trigger collider
public class Switch : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private GameObject door; // Reference to the door GameObject

    [Header("Audio")]
    public AudioSource switchSound; // Reference to the switch sound AudioSource

    private bool doorDestroyed = false; // Determines if the door has been destroyed

    // OnTriggerEnter is a built-in Unity function called when the Switch's trigger collider overlaps with another collider
    public void OnTriggerEnter(Collider other)
    {
        // Check if the overlapping object has the "Player" tag
        if (other.gameObject.CompareTag("Player"))
        {
            // Destroy the door if it hasn't been destroyed yet
            if (!doorDestroyed)
            {
                // Play the switch sound
                switchSound.Play();
                // Call the DestroyDoor function to destroy the door
                DestroyDoor();
                // Set doorDestroyed to true to prevent destroying the door again
                doorDestroyed = true;
            }
        }
    }

    // Destroys the door GameObject
    private void DestroyDoor()
    {
        Destroy(door);
    }
}
