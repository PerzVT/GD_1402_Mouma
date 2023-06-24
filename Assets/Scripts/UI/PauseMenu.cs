using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public AudioSource backgroundMusic; // Add this line to reference the background music AudioSource

    private PlayerController playerController;

    private void Start()
    {
        // Find the PlayerController script in the scene
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If the game is paused, resume it
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                // If the game is not paused, pause it
                Pause();
            }
        }
    }

    // Resume the game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        playerController.SetCursorState(CursorLockMode.Locked, false);

        // Resume the background music
        backgroundMusic.Play();
    }

    // Pause the game
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        playerController.SetCursorState(CursorLockMode.None, true);

        // Pause the background music
        backgroundMusic.Pause();
    }

    // Load the main menu (not implemented yet)
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        Debug.Log("Loading Menu!");
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quitting Game!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Restart the game
    public void RestartGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Restarting Game!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
