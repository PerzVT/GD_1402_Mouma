using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openDistance = 5f; // Distance to move down into the floor.
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float openTime = 5f; // Time taken to move into the floor.

    [Header("Key Requirement")]
    [SerializeField] private int requiredKeys = 1;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI keyCounterText;

    [Header("Tooltip")]
    [SerializeField] private TextMeshPro tooltipText;

    private bool isOpening = false;
    private float openStartTime;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        closedPosition = transform.position;
        openPosition = transform.position - new Vector3(0, openDistance, 0);

        if (player == null)
        {
            Debug.LogError("PlayerController not found!");
        }

        if (keyCounterText == null)
        {
            Debug.LogError("keyCounterText not assigned!");
        }
    }

    private void Update()
    {
        if (isOpening)
        {
            float t = (Time.time - openStartTime) / openTime;
            transform.position = Vector3.Lerp(closedPosition, openPosition, t);

            if (t >= 1f)
            {
                isOpening = false;
            }
        }
    }

    public void OpenDoor()
    {
        if (player != null && keyCounterText != null && player.GetKeys() >= requiredKeys)
        {
            isOpening = true;
            openStartTime = Time.time;
            player.UseKeys(requiredKeys);
            keyCounterText.text = "x <color=green>" + player.GetKeys().ToString() + "</color>";
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && player != null && keyCounterText != null)
        {
            if (player.GetKeys() >= requiredKeys)
            {
                // Show the tooltip
                if (tooltipText != null)
                {
                    tooltipText.text = "Press F to open";
                    tooltipText.gameObject.SetActive(true);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Hide the tooltip
            if (tooltipText != null)
            {
                tooltipText.gameObject.SetActive(false);
            }
        }
    }
}
