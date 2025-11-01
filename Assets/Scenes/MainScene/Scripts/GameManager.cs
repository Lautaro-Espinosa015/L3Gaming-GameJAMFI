using UnityEngine;
// �MUY IMPORTANTE! Debes a�adir esta l�nea para poder controlar las escenas.
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Update se llama una vez por frame
    void Update()
    {
        // 1. Revisa si el jugador presion� la tecla "R"
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 2. Si la presion�, llama a nuestra funci�n de reinicio
            RestartCurrentScene();
        }
    }

    // Esta es nuestra funci�n de reinicio
    public void RestartCurrentScene()
    {
        // IMPORTANTE: Si el juego est� en pausa (Time.timeScale = 0f) por el Game Over,
        // debemos asegurarnos de que el tiempo vuelva a la normalidad ANTES de recargar.
        Time.timeScale = 1f;

        // 3. Obtiene la escena que est� activa AHORA MISMO
        Scene currentScene = SceneManager.GetActiveScene();

        // 4. Carga esa misma escena por su nombre, reinici�ndola.
        SceneManager.LoadScene(currentScene.name);
    }
}