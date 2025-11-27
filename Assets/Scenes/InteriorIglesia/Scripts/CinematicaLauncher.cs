using UnityEngine;
using UnityEngine.Video;

public class CinematicaLauncher : MonoBehaviour
{
    // Arrastra el componente Video Player de este mismo objeto 'Cinematica' aquí en el Inspector
    [SerializeField] private VideoPlayer videoPlayer;

    // Opcional: Referencia al GameObject CanvasCinematica para activarlo/desactivarlo
    [SerializeField] private GameObject videoCanvas;

    void Start()
    {
        // Asegúrate de que el componente VideoPlayer esté asignado
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Asignar el Canvas (si no está asignado) asumiendo que es el padre
        if (videoCanvas == null && transform.parent != null)
        {
            videoCanvas = transform.parent.gameObject;
        }

        // Ocultar el CanvasCinematica al inicio, ya que solo debe aparecer cuando se reproduce el video
        if (videoCanvas != null)
        {
            videoCanvas.SetActive(false);
        }
    }

    // Esta función es llamada públicamente por PuzzleUIController al hacer clic en "Continuar"
    public void PlayVideo()
    {
        if (videoPlayer != null)
        {
            // 1. Activar el Canvas que contiene el RawImage para mostrar el video
            if (videoCanvas != null)
            {
                videoCanvas.SetActive(true);
            }

            // 2. Iniciar la reproducción
            Debug.Log("Iniciando la reproducción de la Cinemática.");
            videoPlayer.Play();

            // Opcional: Suscribirse a un evento para saber cuándo termina el video
            videoPlayer.loopPointReached += OnVideoFinished;
        }
        else
        {
            Debug.LogError("VideoPlayer no asignado en el CinematicaLauncher.");
        }
    }

    // Función que se llama automáticamente cuando el video termina (si no está en loop)
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Cinemática finalizada.");

        // 1. Desactivar el Canvas del video
        if (videoCanvas != null)
        {
            videoCanvas.SetActive(false);
        }

        // 2. Lógica de avance del juego después de la cinemática
        // Ejemplo: Cargar la siguiente escena
        // UnityEngine.SceneManagement.SceneManager.LoadScene("NombreDeTuEscenaSiguiente");

        // 3. Desuscribirse del evento
        vp.loopPointReached -= OnVideoFinished;
    }
}