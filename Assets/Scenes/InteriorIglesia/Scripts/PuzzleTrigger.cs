using StarterAssets;
using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    public GameObject canvasPuzzle;
    public GameObject PlayerArmature; // Asignar en el Inspector

    private bool playerInside = false;
    private ThirdPersonController movementScript;
    private StarterAssetsInputs inputScript;

    void Start()
    {
        // Asegurar que el CanvasPuzzle arranque desactivado
        if (canvasPuzzle != null)
        {
            canvasPuzzle.SetActive(false);
        }

        // Buscar los componentes en PlayerArmature
        movementScript = PlayerArmature.GetComponent<ThirdPersonController>();
        inputScript = PlayerArmature.GetComponent<StarterAssetsInputs>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            EnterPuzzle();
        }
    }

    public void EnterPuzzle()
    {
        Debug.Log("Entrando al puzzle: activando CanvasPuzzle");
        canvasPuzzle.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (movementScript != null) movementScript.enabled = false;
        if (inputScript != null) inputScript.enabled = false;
    }

    public void ExitPuzzle()
    {
        Debug.Log("Saliendo del puzzle: desactivando CanvasPuzzle");
        canvasPuzzle.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (movementScript != null) movementScript.enabled = true;
        if (inputScript != null) inputScript.enabled = true;
    }
}