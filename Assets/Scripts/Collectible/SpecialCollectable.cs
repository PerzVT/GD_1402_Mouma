using UnityEngine;
using TMPro;

public class SpecialCollectable : Collectable
{
    [SerializeField] private PlayerController player; // Reference to the PlayerController script
    [SerializeField] private int speedBoost = 5; // The amount of speed to boost
    [SerializeField] private float boostDuration = 5f; // The duration of the speed boost
    [SerializeField] private TextMeshProUGUI boostText; // The on-screen text for displaying the speed boost

    // Called when the special collectable's trigger collider overlaps with another collider
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        // Check if the overlapping object has the "Player" tag
        if (other.gameObject.CompareTag("Player"))
        {
            // Boost the player's speed
            BoostPlayerSpeed();

            // Display the speed boost message
            DisplayBoostMessage();
        }
    }

    // Boost the player's speed
    private void BoostPlayerSpeed()
    {
        player.BoostSpeed(speedBoost, boostDuration);
    }

    // Display the speed boost message
    private void DisplayBoostMessage()
    {
        boostText.text = "Boost : <color=orange>Active</color>";
        boostText.gameObject.SetActive(true);

        // Hide the speed boost message after a delay
        Invoke("HideBoostMessage", boostDuration);
    }

    // Hide the speed boost message
    private void HideBoostMessage()
    {
        boostText.gameObject.SetActive(false);
    }
}
