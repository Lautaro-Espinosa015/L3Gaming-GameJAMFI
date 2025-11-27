using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PuzzleUIController : MonoBehaviour
{
    [Header("Botones")]
    public Button btnReiniciar;
    public Button btnSalir;
    public Button btnContinuar;

    [Header("Referencias")]
    public PuzzleManager puzzleManager;
    public GameObject canvasPuzzle;
    public GameObject PlayerArmature;

    // Referencia al script CinematicaLauncher
    [Header("Cinemática")]
    // **Importante:** Asegúrate de arrastrar el objeto 'Cinematica' aquí en el Inspector
    public CinematicaLauncher cinematicaLauncher;

    void Start()
    {
        // Buscar el objeto del jugador por tag
        PlayerArmature = GameObject.FindGameObjectWithTag("Player");

        // Conectar listeners a los botones
        btnReiniciar.onClick.AddListener(ReiniciarPuzzle);
        btnSalir.onClick.AddListener(SalirPuzzle);

        // Conecta el botón Continuar a la función que iniciará la cinemática
        btnContinuar.onClick.AddListener(ContinuarJuego);

        // Ocultar botón Continuar al inicio
        btnContinuar.gameObject.SetActive(false);
    }

    public void ReiniciarPuzzle()
    {
        if (puzzleManager != null)
        {
            puzzleManager.InicializarPuzzle();
            btnContinuar.gameObject.SetActive(false);
            Debug.Log("Puzzle reiniciado.");
        }
        else
        {
            Debug.LogWarning("PuzzleManager no asignado en PuzzleUIController.");
        }
    }

    public void SalirPuzzle()
    {
        if (canvasPuzzle != null) canvasPuzzle.SetActive(false);

        if (PlayerArmature != null)
        {
            var controller = PlayerArmature.GetComponent<ThirdPersonController>();
            if (controller != null)
            {
                controller.enabled = true;
                Debug.Log("ThirdPersonController reactivado al salir del puzzle.");
            }
            else
            {
                Debug.LogWarning("No se encontró ThirdPersonController en PlayerArmature.");
            }
        }
        else
        {
            Debug.LogWarning("PlayerArmature no asignado en PuzzleUIController.");
        }
    }

    
    // FUNCIÓN CLAVE: Inicia la cinemática al hacer clic en el botón Continuar
    
    public void ContinuarJuego()
    {
        Debug.Log("Continuar: iniciando cinemática.");

        // 1. Iniciar la reproducción del video
        if (cinematicaLauncher != null)
        {
            cinematicaLauncher.PlayVideo();
        }
        else
        {
            Debug.LogError("CinematicaLauncher no asignado. El video no se reproducirá.");
        }

        // 2. Desactivar el Canvas del Puzzle
        // Esto oculta la UI del puzzle para que el video pueda ser visible
        if (canvasPuzzle != null)
        {
            canvasPuzzle.SetActive(false);
        }
    }

    
    // Muestra el botón Continuar después de ganar el puzzle
    
    public void MostrarContinuar()
    {
        btnContinuar.gameObject.SetActive(true);
    }
}