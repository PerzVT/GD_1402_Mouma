using UnityEngine;

// Inherits from the Collectable class, representing a Coin collectable in the game
public class CoinCollectable : Collectable
{
    [SerializeField] private PlayerController player; // Reference to the PlayerController script
    [SerializeField] private int coinValue = 1; // Value of the coin when collected

    // Called when the coin's trigger collider overlaps with another collider
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        // Check if the overlapping object has the "Player" tag
        if (other.gameObject.CompareTag("Player"))
        {
            // Call the AddScore method from the PlayerController script and pass the coin's value
            player.AddScore(coinValue);
        }
    }

    // Called every frame
    private void Update()
    {
        // Rotate the coin around the Y-axis
        RotateCoin();
    }

    // Rotate the coin with a fixed speed around the Y-axis
    private void RotateCoin()
    {
        transform.Rotate(new Vector3(0f, 1f, 0f));
    }
}
