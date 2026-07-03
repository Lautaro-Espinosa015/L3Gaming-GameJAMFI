using UnityEngine;
using System;
using System.Linq;

public class PuzzleManager : MonoBehaviour
{
    [Header("Pool de piezas")]
    public Transform piecesPool;
    public GameObject[] puzzlePieces;

    [Header("Configuración de grilla del pool")]
    public int columnas = 2;
    public int filas = 3;

    private Camara3Persona camaraJugador;

    public void InicializarPuzzle()
    {
        // Pausa player, físicas, enemigos y todo lo que use Time.deltaTime.
        Time.timeScale = 0f;

        // Busca automáticamente la cámara que tenga este script.
        camaraJugador = FindFirstObjectByType<Camara3Persona>();

        // Desactiva el script: la cámara no puede leer Mouse X ni Mouse Y.
        if (camaraJugador != null)
        {
            camaraJugador.enabled = false;
        }
        else
        {
            Debug.LogWarning("No se encontró ningún objeto con el script Camara3Persona.");
        }

        // Como el puzzle no usa mouse, dejamos el cursor bloqueado y oculto.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Mezclar aleatoriamente el orden de las piezas.
        System.Random rng = new System.Random();
        puzzlePieces = puzzlePieces.OrderBy(_ => rng.Next()).ToArray();

        int index = 0;

        foreach (GameObject piece in puzzlePieces)
        {
            piece.transform.SetParent(piecesPool);

            RectTransform rt = piece.GetComponent<RectTransform>();

            if (rt != null)
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);

                float espacioX = rt.rect.width;
                float espacioY = rt.rect.height;

                int col = index % columnas;
                int fila = index / columnas;

                float x = (col - (columnas - 1) / 2f) * espacioX;
                float y = -((fila - (filas - 1) / 2f) * espacioY);

                rt.anchoredPosition = new Vector2(x, y);
            }

            index++;
        }

        Debug.Log("Puzzle iniciado: juego y cámara congelados.");
    }

    public void PuzzleCompletado()
    {
        Debug.Log("PuzzleManager: puzzle completo.");

        // Reanuda el juego.
        Time.timeScale = 1f;

        // Reactiva el control de la cámara.
        if (camaraJugador != null)
        {
            camaraJugador.enabled = true;
        }

        // Mantiene el cursor preparado para el juego normal.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PuzzleUIController ui = FindFirstObjectByType<PuzzleUIController>();

        if (ui != null)
        {
            ui.MostrarContinuar();
        }
    }
}

