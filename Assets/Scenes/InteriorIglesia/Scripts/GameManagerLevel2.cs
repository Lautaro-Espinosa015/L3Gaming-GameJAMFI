using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLevel2 : MonoBehaviour
{
    [Header("UI del Nivel 2 (Puzzles)")]
    public GameObject victoryPanel;
    public GameObject pausePanel;
    // Ya no necesitamos gameOverPanel aquí

    private bool isPaused = false;
    private bool isVictory = false; // Para bloquear la pausa si ya ganaste

    void Start()
    {
        // 1. Asegurar que los paneles empiecen apagados
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        Time.timeScale = 1f; // El tiempo corre normal al inicio

        // 2. Ocultar cursor al iniciar (para mover la cámara cómodamente)
        LockCursor();
    }

    void Update()
    {
        // Tecla P para Pausar/Despausar (Solo si no has ganado aún)
        if (Input.GetKeyDown(KeyCode.P) && !isVictory)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // --- FUNCIONES DE PAUSA ---

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);

        Time.timeScale = 0f; // Congelar tiempo
        UnlockCursor();      // Mostrar cursor para hacer clic
    }

    public void ResumeGame() // Conectar al botón "Reanudar"
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);

        Time.timeScale = 1f; // Reanudar tiempo
        LockCursor();        // Ocultar cursor para volver a jugar
    }

    // --- FUNCIONES DE VICTORIA (Llamada por CinematicaLauncher) ---

    public void ShowVictoryScreen()
    {
        isVictory = true;
        if (victoryPanel != null) victoryPanel.SetActive(true);

        Time.timeScale = 0f; // Pausar juego
        UnlockCursor();
    }

    // --- FUNCIONES PARA BOTONES UI ---

    public void RestartLevel() // Conectar al botón "Reiniciar"
    {
        Time.timeScale = 1f; // Importante: Despausar antes de recargar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame() // Conectar al botón "Salir del Juego"
    {
        Debug.Log("Saliendo del juego...");

        // ESTE ES EL TRUCO: Funciona en el Editor y en el Juego final
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void GoToMainMenu() // Opcional: Si quieres volver al menú principal
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }

    // --- COMPATIBILIDAD CON PLAYER ---
    // Dejamos esta función vacía para que el script 'PlayerHealth' no de error
    // si intenta llamarla por accidente.
    public void ShowGameOver() { }

    // --- ÚTILES DE CURSOR ---
    void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}