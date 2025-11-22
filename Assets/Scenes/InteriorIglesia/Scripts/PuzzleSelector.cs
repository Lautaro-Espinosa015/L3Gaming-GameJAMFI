using UnityEngine;
using UnityEngine.UI;
using TMPro;   // Para usar TextMeshPro

public class PuzzleSelector : MonoBehaviour
{
    [Header("Piezas disponibles (izquierda)")]
    public GameObject[] puzzlePieces;

    [Header("Slots destino (derecha)")]
    public GameObject[] puzzleSlots;

    [Header("Colores de selección")]
    public Color highlightColor = Color.cyan;
    public Color normalColor = Color.white;

    [Header("Mensaje de feedback")]
    public TMP_Text feedbackText;   // Asignar en el Inspector

    private int pieceIndex = 0;
    private int slotIndex = 0;
    private bool selectingPiece = true;
    private GameObject selectedPiece = null;

    void Start()
    {
        UpdateSelection();
        if (feedbackText != null) feedbackText.text = "";
    }

    void Update()
    {
        // Navegación con A/D/W/S
        if (Input.GetKeyDown(KeyCode.A)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.D)) MoveRight();
        if (Input.GetKeyDown(KeyCode.W)) MoveUp();
        if (Input.GetKeyDown(KeyCode.S)) MoveDown();

        // Confirmar con E
        if (Input.GetKeyDown(KeyCode.E)) ConfirmSelection();
    }

    void MoveLeft()
    {
        if (selectingPiece)
            pieceIndex = (pieceIndex - 1 + puzzlePieces.Length) % puzzlePieces.Length;
        else
            slotIndex = (slotIndex - 1 + puzzleSlots.Length) % puzzleSlots.Length;

        UpdateSelection();
    }

    void MoveRight()
    {
        if (selectingPiece)
            pieceIndex = (pieceIndex + 1) % puzzlePieces.Length;
        else
            slotIndex = (slotIndex + 1) % puzzleSlots.Length;

        UpdateSelection();
    }

    void MoveUp()
    {
        if (selectingPiece)
            pieceIndex = (pieceIndex - 1 + puzzlePieces.Length) % puzzlePieces.Length;
        else
            slotIndex = (slotIndex - 2 + puzzleSlots.Length) % puzzleSlots.Length;

        UpdateSelection();
    }

    void MoveDown()
    {
        if (selectingPiece)
            pieceIndex = (pieceIndex + 1) % puzzlePieces.Length;
        else
            slotIndex = (slotIndex + 2) % puzzleSlots.Length;

        UpdateSelection();
    }

    void UpdateSelection()
    {
        // Resetear colores
        foreach (GameObject piece in puzzlePieces)
            piece.GetComponent<Image>().color = normalColor;

        foreach (GameObject slot in puzzleSlots)
            slot.GetComponent<Image>().color = normalColor;

        // Resaltar actual
        if (selectingPiece)
            puzzlePieces[pieceIndex].GetComponent<Image>().color = highlightColor;
        else
            puzzleSlots[slotIndex].GetComponent<Image>().color = highlightColor;
    }

    void ConfirmSelection()
    {
        if (selectingPiece)
        {
            // Guardar pieza seleccionada
            selectedPiece = puzzlePieces[pieceIndex];
            selectingPiece = false;
        }
        else
        {
            // Colocar pieza en slot
            GameObject targetSlot = puzzleSlots[slotIndex];
            selectedPiece.transform.SetParent(targetSlot.transform);
            selectedPiece.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // Validar si es la pieza correcta
            PuzzleSlot slotScript = targetSlot.GetComponent<PuzzleSlot>();
            if (selectedPiece.name != slotScript.correctPieceName)
            {
                // ❌ Incorrecta → devolver al pool
                selectedPiece.transform.SetParent(GameObject.Find("PiecesPool").transform);
                selectedPiece.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                // Mostrar mensaje temporal
                if (feedbackText != null)
                {
                    feedbackText.text = "Pieza incorrecta";
                    CancelInvoke(nameof(ClearFeedback));
                    Invoke(nameof(ClearFeedback), 2f); // Borra el mensaje en 2 segundos
                }

                Debug.Log("Pieza incorrecta, devuelta al pool.");
            }

            // Resetear estado
            selectedPiece.GetComponent<Image>().color = normalColor;
            selectedPiece = null;
            selectingPiece = true;

            // Validar puzzle
            FindObjectOfType<PuzzleValidator>().CheckCompletion();
        }

        UpdateSelection();
    }

    void ClearFeedback()
    {
        if (feedbackText != null) feedbackText.text = "";
    }
}