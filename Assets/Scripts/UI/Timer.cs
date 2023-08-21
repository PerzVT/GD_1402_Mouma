using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI loseText;

    // Set the timer to 3 minutes in seconds
    private float timer = 180;

    private void Start()
    {
        if (timerText == null)
        {
            timerText = GetComponent<TextMeshProUGUI>();
        }

        if (loseText != null)
        {
            loseText.gameObject.SetActive(false);
        }

        // Format the initial timer value
        UpdateTimerText();
    }

    void Update()
    {
        // Decrease the timer value by delta time
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // When timer reaches zero, display the lose text
            timer = 0;
            if (loseText != null)
            {
                loseText.text = "You <color=red>Lose!</color>";
                loseText.gameObject.SetActive(true);
            }

            // Pause the game
            Time.timeScale = 0;
        }

        // Update the timer text
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        TimeSpan time = TimeSpan.FromSeconds(Mathf.Round(timer));
        string formattedTime = time.ToString(@"mm\:ss");
        timerText.text = $"Timer: <color=green>{formattedTime}</color>";
    }
}
