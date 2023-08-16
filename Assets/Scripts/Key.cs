using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            SlidingDoor slidingDoor = FindObjectOfType<SlidingDoor>();
            if (slidingDoor != null)
            {
                slidingDoor.AddKey();
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("No SlidingDoor component found in the scene!");
            }
        }
    }
}
