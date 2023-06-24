using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Header("UI Manager")]
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject menu;

    void Start()
    {
        hud.SetActive(true);
        menu.SetActive(false);
    }
}
