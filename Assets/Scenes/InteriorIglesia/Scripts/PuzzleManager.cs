using UnityEngine;
using System;          // Para Random
using System.Linq;     // Para OrderBy

public class PuzzleManager : MonoBehaviour
{
    [Header("Pool de piezas")]
    public Transform piecesPool;        // Panel contenedor del pool
    public GameObject[] puzzlePieces;   // Piezas del puzzle

    [Header("Configuración de grilla del pool")]
    public int columnas = 2;            // Número de columnas (ej. 2 para 6 piezas)
    public int filas = 3;               // Número de filas (ej. 3 para 6 piezas)

    public void InicializarPuzzle()
    {
        // Mezclar aleatoriamente el orden de las piezas
        System.Random rng = new System.Random();
        puzzlePieces = puzzlePieces.OrderBy(_ => rng.Next()).ToArray();

        int index = 0;

        foreach (GameObject piece in puzzlePieces)
        {
            piece.transform.SetParent(piecesPool);

            RectTransform rt = piece.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Normalizar anchors/pivots para posiciones consistentes
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);

                // Usar el tamaño real de la pieza como espaciado
                float espacioX = rt.rect.width;
                float espacioY = rt.rect.height;

                int col = index % columnas;
                int fila = index / columnas;

                // Posición calculada en grilla del pool (centrada)
                float x = (col - (columnas - 1) / 2f) * espacioX;
                float y = -((fila - (filas - 1) / 2f) * espacioY);

                rt.anchoredPosition = new Vector2(x, y);
            }

            index++;
        }

        Debug.Log("Puzzle reiniciado con piezas distribuidas en grilla sin espacios extra.");
    }

    public void PuzzleCompletado()
    {
        Debug.Log("PuzzleManager: puzzle completo.");
        // Mostrar botón Continuar
        var ui = FindObjectOfType<PuzzleUIController>();
        if (ui != null) ui.MostrarContinuar();
    }
}