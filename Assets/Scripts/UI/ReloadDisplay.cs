using UnityEngine;
using TMPro;

public class ReloadDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reloadDisplayText;

    private void Start()
    {
        if (reloadDisplayText == null)
        {
            reloadDisplayText = GetComponent<TextMeshProUGUI>();
        }
    }

    public void UpdateReloadDisplayText(int currentZaps, int maxZaps)
    {
        if (currentZaps > 0)
        {
            reloadDisplayText.text = $"Zaps: <color=orange>{currentZaps}/{maxZaps}</color>";
        }
        else
        {
            reloadDisplayText.text = "R to <color=orange>Reload!</color>";
        }
    }
}
