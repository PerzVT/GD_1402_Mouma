using UnityEngine;
/// <summary>
/// For things like collectables it may make a bit more sense to keep track of the collection in the PlayerController or move that code to another script on the Player that has access to the PlayerController, just because a collectable object should not have to know about what object collects it. I would like to see the audio levels adjusted a bit better for these, as right now it is really loud.
/// 
/// </summary>
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
