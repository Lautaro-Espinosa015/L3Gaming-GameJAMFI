using UnityEngine;
// ¡MUY IMPORTANTE! Debes añadir esta línea para poder controlar las escenas.
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Update se llama una vez por frame
    void Update()
    {
        // 1. Revisa si el jugador presionó la tecla "R"
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 2. Si la presionó, llama a nuestra función de reinicio
            RestartCurrentScene();
        }
    }

    // Esta es nuestra función de reinicio
    public void RestartCurrentScene()
    {
        // IMPORTANTE: Si el juego está en pausa (Time.timeScale = 0f) por el Game Over,
        // debemos asegurarnos de que el tiempo vuelva a la normalidad ANTES de recargar.
        Time.timeScale = 1f;

        // 3. Obtiene la escena que está activa AHORA MISMO
        Scene currentScene = SceneManager.GetActiveScene();

        // 4. Carga esa misma escena por su nombre, reiniciándola.
        SceneManager.LoadScene(currentScene.name);
    }
}