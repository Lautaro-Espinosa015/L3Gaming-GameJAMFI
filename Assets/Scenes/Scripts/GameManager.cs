using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    // --- Referencias a los Paneles de UI ---
    public GameObject startMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel; // --- AÑADIDO ---

    public VideoPlayer victoryVideoPlayer; // --- AÑADIDO ---

    // --- Variables de Estado del Juego ---
    private bool isGamePaused = false;
    private bool isGameOver = false;
    private bool isGameWon = false; // --- AÑADIDO ---

    void Start()
    {
        // Activa/desactiva todos los paneles al inicio
        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false); // --- AÑADIDO ---

        // Pausa el juego para el menú de inicio
        Time.timeScale = 0f;
        isGamePaused = true;

        // Resetea los estados
        isGameOver = false;
        isGameWon = false;

        // Muestra el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        // Reiniciar con R
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCurrentScene();
        }

        // Pausar con P
        // --- AÑADIDO: No se puede pausar si ya ganaste o perdiste ---
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver && !isGameWon && (startMenuPanel == null || !startMenuPanel.activeSelf))
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

    // --- Funciones de Menús ---
    public void StartGame()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        ResumeGame();
    }

    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        isGameOver = true;
        isGamePaused = true; // Marca como pausado

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // --- AÑADIDA: Nueva función de Victoria ---
    public void ShowVictoryScreen()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
        isGameWon = true;
        isGamePaused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // --- AÑADIDO: Reproducir el video ---
        if (victoryVideoPlayer != null)
        {
            victoryVideoPlayer.Play();
        }
        // ---
    }

    // --- Función para Reiniciar ---
    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        isGameWon = false;
        isGamePaused = false;

        // --- AÑADIDO: Detener el video al reiniciar ---
        if (victoryVideoPlayer != null)
        {
            victoryVideoPlayer.Stop();
        }
        // ---

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