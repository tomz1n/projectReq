using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject pausePanel;

    private bool _isPaused = false;

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        LockCursor(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (KeypadUI.IsOpen)
            {
                FindObjectOfType<KeypadUI>()?.CloseKeypad();
                return;
            }

            if (!gameOverPanel.activeSelf && !victoryPanel.activeSelf)
            {
                TogglePause();
            }
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        PauseGameAndUnlockCursor();
    }
    
    public void ShowVictory()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        PauseGameAndUnlockCursor();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (pausePanel != null) pausePanel.SetActive(_isPaused);

        if (_isPaused)
        {
            PauseGameAndUnlockCursor();
        }
        else
        {
            ResumeGameAndLockCursor();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("MainMenu"); 
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    private void PauseGameAndUnlockCursor()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;
        LockCursor(false);
    }
    
    public void PlayGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene("LevelOne"); 
    }

    private void ResumeGameAndLockCursor()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        
        if (KeypadUI.IsOpen)
        {
            LockCursor(false);
        }
        else
        {
            LockCursor(true);
        }
    }

    private void LockCursor(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}