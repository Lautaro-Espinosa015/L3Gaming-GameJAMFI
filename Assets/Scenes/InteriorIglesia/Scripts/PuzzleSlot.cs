using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    public string correctPieceName; 

    public bool IsCorrect(GameObject piece)
    {
        return piece.name == correctPieceName;
    }
}