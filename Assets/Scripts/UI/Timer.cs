using System;
using UnityEngine;
using TMPro;

// Displays and updates the timer on the screen
public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float timer = 0;

    private void Start()
    {
        if (timerText == null)
        {
            timerText = GetComponent<TextMeshProUGUI>();
        }
    }

    // Update the timer value and display it on the screen
    void Update()
    {
        timer += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(Mathf.Round(timer));
        string formattedTime = time.ToString(@"mm\:ss");
        UpdateTimerText(formattedTime);
    }

    // Set the timer text with a formatted string
    private void UpdateTimerText(string formattedTime)
    {
        timerText.text = $"Timer: <color=green>{formattedTime}</color>";
    }
}
