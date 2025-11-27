using UnityEngine;
using UnityEngine.Video;

public class CinematicaLauncher : MonoBehaviour
{
    [Header("Configuración Video")]
    public VideoPlayer videoPlayer;
    public GameObject videoCanvas; // El panel negro donde se ve el video

    [Header("Configuración Victoria")]
    [Tooltip("Arrastra aquí tu Panel de Victoria (UI)")]
    public GameObject victoryPanel; // <--- ¡NUEVO!

    void Start()
    {
        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();

        // Asegurarnos de que los paneles empiecen apagados
        if (videoCanvas != null) videoCanvas.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Suscribirse al evento de fin de video
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // Esta función la llamas desde el evento OnDeath del enemigo
    public void PlayVideo()
    {
        if (videoPlayer != null)
        {
            // 1. Mostrar pantalla de video
            if (videoCanvas != null) videoCanvas.SetActive(true);

            // 2. Reproducir
            videoPlayer.Play();
            Debug.Log("Reproduciendo cinemática...");
        }
    }

    // Se ejecuta AUTOMÁTICAMENTE cuando el video termina
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video terminado. Activando Victoria.");

        // 1. Ocultar pantalla de video
        if (videoCanvas != null) videoCanvas.SetActive(false);

        // 2. MOSTRAR VICTORIA (Directamente aquí)
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            // --- LÓGICA DE FINAL DE JUEGO ---
            // Como no usamos el GameManager, lo hacemos aquí manualmente:
            Time.timeScale = 0f; // Pausar el juego

            // Liberar el mouse para que puedan hacer clic en los botones
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Debug.LogError("¡Olvidaste arrastrar el Panel de Victoria al script de la Cinemática!");
        }
    }
}