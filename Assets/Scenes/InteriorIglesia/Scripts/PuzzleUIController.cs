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
    public GameObject playerArmature; // Asignar directamente el objeto del jugador

    void Start()
    {
        // Conectar listeners
        btnReiniciar.onClick.AddListener(ReiniciarPuzzle);
        btnSalir.onClick.AddListener(SalirPuzzle);
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

        if (playerArmature != null)
        {
            var controller = playerArmature.GetComponent<ThirdPersonController>();
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

    public void ContinuarJuego()
    {
        Debug.Log("Continuar: puzzle completado, avanzar en el flujo.");
        // Ejemplo: cargar otra escena
        // SceneManager.LoadScene("EscenaSiguiente");
    }

    public void MostrarContinuar()
    {
        btnContinuar.gameObject.SetActive(true);
    }
}