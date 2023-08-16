using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MuteButtonController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourceToControl;
    [SerializeField] private TMP_Text buttonText;

    private bool isMuted = false;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(ToggleMute);

        if (!audioSourceToControl)
        {
            Debug.LogError("No AudioSource assigned to the MuteButtonController.");
        }

        if (!buttonText)
        {
            Debug.LogError("No TMP_Text assigned to the MuteButtonController.");
        }

        UpdateButtonText();
    }

    public void ToggleMute()
    {
        if (audioSourceToControl)
        {
            isMuted = !isMuted;
            audioSourceToControl.mute = isMuted;
            UpdateButtonText();
        }
    }

    private void UpdateButtonText()
    {
        if (buttonText)
        {
            buttonText.text = isMuted ? "Unmute" : "Mute";
        }
    }
}
