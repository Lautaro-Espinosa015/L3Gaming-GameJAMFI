using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // --- Referencias a los Paneles de UI ---
    public GameObject startMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;

    // --- Variables de Estado del Juego ---
    private bool isGamePaused = false;
    private bool isGameOver = false;

    void Start()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 0f;
        isGamePaused = true;

        // --- AÑADIDO: Mostrar el cursor en el Menú de Inicio ---
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentScene();
        }

        // (Línea nueva)
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver && (startMenuPanel == null || !startMenuPanel.activeSelf))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- Funciones para el Menú de Inicio ---
    public void StartGame()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        ResumeGame(); // ResumeGame() ya se encarga de ocultar el cursor
    }

    // --- Funciones para Pausar/Despausar ---
    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;

        // --- AÑADIDO: Mostrar el cursor en Pausa ---
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;

        // --- AÑADIDO: Ocultar el cursor al Jugar ---
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // --- Funciones para Game Over ---
    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        isGameOver = true;

        // --- AÑADIDO: Mostrar el cursor en Game Over ---
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // --- Función para Reiniciar ---
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isGamePaused = false;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // --- Función para Salir del Juego ---
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}