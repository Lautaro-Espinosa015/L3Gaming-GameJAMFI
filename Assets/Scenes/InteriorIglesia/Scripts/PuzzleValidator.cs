using UnityEngine;
using TMPro;   // Importar TextMeshPro

public class PuzzleValidator : MonoBehaviour
{
    [Header("Slots del Puzzle")]
    public GameObject[] puzzleSlots;   // Asignar Slot01…Slot06 en el Inspector

    [Header("Mensaje de completado")]
    public TMP_Text completionText;    // Asignar el objeto CompletionTextTMP en el Inspector

    public void CheckCompletion()
    {
        bool allCorrect = true;

        foreach (GameObject slot in puzzleSlots)
        {
            PuzzleSlot slotScript = slot.GetComponent<PuzzleSlot>();

            // Si el slot está vacío → puzzle incompleto
            if (slot.transform.childCount == 0)
            {
                allCorrect = false;
                break;
            }

            // Verificar si la pieza colocada coincide con la esperada
            GameObject placedPiece = slot.transform.GetChild(0).gameObject;
            if (placedPiece.name != slotScript.correctPieceName)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            completionText.text = "¡Puzzle completado!";
            Debug.Log("Puzzle completado correctamente.");
            FindObjectOfType<PuzzleUIController>().MostrarContinuar();

        }
        else
        {
            completionText.text = "";
        }
    }
}