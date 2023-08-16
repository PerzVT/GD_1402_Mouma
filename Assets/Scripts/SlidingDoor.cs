using UnityEngine;
using TMPro;

public class SlidingDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float slideDistance = 5f; 
    [SerializeField] private float slideSpeed = 1f;
    [SerializeField] private TMP_Text keyCountText; // TextMeshPro reference

    [Header("Key Management")]
    [SerializeField] private int requiredKeys = 1;
    private int currentKeyCount;

    private bool isOpening = false;
    private Vector3 initialPosition;
    private Vector3 openPosition;

    private void Start()
    {
        initialPosition = transform.position;
        openPosition = initialPosition + Vector3.up * slideDistance;
        UpdateKeyCountDisplay();
    }

    private void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPosition, slideSpeed * Time.deltaTime);
            
            if (transform.position == openPosition)
            {
                isOpening = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && currentKeyCount >= requiredKeys)
        {
            currentKeyCount -= requiredKeys;
            UpdateKeyCountDisplay();
            isOpening = true;
        }
    }

    public void AddKey()
    {
        currentKeyCount++;
        UpdateKeyCountDisplay();
    }

    private void UpdateKeyCountDisplay()
    {
        if (keyCountText != null)
        {
            keyCountText.text = "Keys: " + currentKeyCount;
        }
        else
        {
            Debug.LogWarning("Key count TMP_Text component is not assigned.");
        }
    }
}
