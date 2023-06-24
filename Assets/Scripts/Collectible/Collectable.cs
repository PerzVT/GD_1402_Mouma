using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private AudioSource collectAudio; // Reference to the AudioSource component to play collect sound

    [Header("Rotation")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 50, 0); // Define the rotation speed (degrees per second)

    private void Update()
    {
        // Rotate the collectible continuously
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    // Called when the collectable's trigger collider overlaps with another collider
    public virtual void OnTriggerEnter(Collider other)
    {
        // Check if the overlapping object has the "Player" tag
        if (other.gameObject.CompareTag("Player"))
        {
            // Destroy the collectable object and play the collect sound
            Collect(other);
        }
    }

    // Destroy the collectable object and play the collect sound
    protected void Collect(Collider playerCollider)
    {
        Destroy(gameObject);
        PlayCollectAudio();
    }

    // Play the collect audio
    private void PlayCollectAudio()
    {
        collectAudio.Play();
    }
}
