using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    [Header("Paneles UI")]
    public GameObject startMenuPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    [Header("Videos")]
    public VideoPlayer victoryVideoPlayer;

    // --- NUEVO: Variables para la Intro ---
    [Tooltip("Arrastra aquí el Video Player de tu cinemática de inicio")]
    public VideoPlayer introVideoPlayer;
    [Tooltip("Arrastra aquí el Panel Negro (RawImage) donde se ve la intro")]
    public GameObject introVideoPanel;
    // -------------------------------------

    // Estados
    private bool isGamePaused = false;
    private bool isGameOver = false;
    private bool isGameWon = false;
    private bool isPlayingIntro = false; // Para saber si estamos viendo la intro

    void Start()
    {
        // 1. Configuración Inicial
        if (startMenuPanel != null) startMenuPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Aseguramos que el panel de intro empiece apagado
        if (introVideoPanel != null) introVideoPanel.SetActive(false);

        // 2. Pausar tiempo
        Time.timeScale = 0f;
        isGamePaused = true;

        isGameOver = false;
        isGameWon = false;

        UnlockCursor();
    }

    void Update()
    {
        // --- PROTECCIÓN DE CURSOR ---
        bool menuAbierto = (startMenuPanel != null && startMenuPanel.activeSelf) ||
                           isGamePaused || isGameOver || isGameWon || isPlayingIntro;

        if (menuAbierto)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Iniciar con G
        if (startMenuPanel != null && startMenuPanel.activeSelf && Input.GetKeyDown(KeyCode.G))
        {
            StartGame();
        }

        // Omitir Intro con Espacio (Opcional)
        if (isPlayingIntro && Input.GetKeyDown(KeyCode.Space))
        {
            SkipIntro();
        }

        // Reiniciar con R
        if (Input.GetKeyDown(KeyCode.R)) RestartCurrentScene();

        // Pausa con P (Solo si no hay intro, ni gameover, ni victoria)
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver && !isGameWon && !isPlayingIntro &&
           (startMenuPanel == null || !startMenuPanel.activeSelf))
        {
            if (isGamePaused) ResumeGame();
            else PauseGame();
        }
    }

    // --- MODIFICACIÓN: Lógica de Inicio ---

    public void StartGame()
    {
        // 1. Ocultar menú
        if (startMenuPanel != null) startMenuPanel.SetActive(false);

        // 2. ¿Tenemos video de intro?
        if (introVideoPlayer != null)
        {
            PlayIntro();
        }
        else
        {
            // Si no hay video, empezamos el juego directamente
            ResumeGame();
        }
    }

    void PlayIntro()
    {
        isPlayingIntro = true;

        // Mostrar pantalla de video
        if (introVideoPanel != null) introVideoPanel.SetActive(true);

        // Preparar evento de fin
        introVideoPlayer.loopPointReached += OnIntroFinished;

        // Reproducir
        introVideoPlayer.Play();
    }

    void OnIntroFinished(VideoPlayer vp)
    {
        // Limpieza
        vp.loopPointReached -= OnIntroFinished;
        SkipIntro();
    }

    public void SkipIntro()
    {
        // Detener video y ocultar panel
        if (introVideoPlayer != null) introVideoPlayer.Stop();
        if (introVideoPanel != null) introVideoPanel.SetActive(false);

        isPlayingIntro = false;

        // ¡AHORA SÍ EMPIEZA EL JUEGO!
        ResumeGame();
    }

    // --------------------------------------

    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        UnlockCursor();
    }

    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
        LockCursor(); // Ocultar mouse para jugar
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        isGameOver = true;
        isGamePaused = true;
    }

    public void ShowVictoryScreen()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
        isGameWon = true;
        isGamePaused = true;
        if (victoryVideoPlayer != null) victoryVideoPlayer.Play();
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

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