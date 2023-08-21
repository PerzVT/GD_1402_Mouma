using UnityEngine;
using TMPro;

public class KeyCollectable : Collectable
{
    [SerializeField] private PlayerController player; // Reference to the PlayerController script
    [SerializeField] private TextMeshProUGUI keyCounterText; // Reference to the TextMeshProUGUI component to display the number of keys

    // Called when the key collectable's trigger collider overlaps with another collider
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        // Check if the overlapping object has the "Player" tag
        if (other.gameObject.CompareTag("Player"))
        {
            // Increment the player's key count
            player.IncrementKeyCount();

            // Update the displayed number of keys
            UpdateKeyCounter();
        }
    }

    // Update the displayed number of keys
    private void UpdateKeyCounter()
    {
        int currentKeys = player.GetKeys();
        keyCounterText.text = "x <color=green>" + currentKeys.ToString() + "</color>";
    }
}
