using UnityEngine;
// Ya no necesitamos "using Unity.Cinemachine;" porque usas tu propio script.

public class CharacterSpawner : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Arrastra aquí tus 4 prefabs (Antonio, Sosa, Sam, Quispe) EN EL MISMO ORDEN que en el menú.")]
    public GameObject[] characterPrefabs;

    [Tooltip("Arrastra aquí el objeto vacío donde quieres que nazca el jugador.")]
    public Transform spawnPoint;

    [Header("Cámara")]
    [Tooltip("Arrastra aquí tu Main Camera (la que tiene el script Camara3Persona).")]
    public GameObject mainCameraObject;

    // Variable para guardar al jugador actual y borrarlo si cambiamos
    private GameObject currentPlayer;

    void Start()
    {
        // Al iniciar, cargar el personaje guardado
        int savedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        SpawnCharacter(savedIndex);
    }

    // Esta función se llama al inicio y también desde el Menú al cambiar personaje
    public void SpawnCharacter(int index)
    {
        // 1. Si ya hay un jugador en escena, bórralo antes de crear el nuevo
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        // Protección por si el índice está fuera de rango
        if (index >= characterPrefabs.Length) index = 0;

        // 2. Crear (Instanciar) al nuevo personaje
        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        currentPlayer = Instantiate(characterPrefabs[index], pos, rot);

        // 3. CONECTAR TU CÁMARA PERSONALIZADA

        // Buscamos el punto de seguimiento (el cuello del personaje)
        Transform cameraRoot = currentPlayer.transform.Find("PlayerCameraRoot");

        if (cameraRoot != null && mainCameraObject != null)
        {
            // Buscamos TU script en la cámara
            var cameraScript = mainCameraObject.GetComponent<Camara3Persona>();

            if (cameraScript != null)
            {
                // --- AQUÍ ESTÁ LA MAGIA ---
                // Asignamos el nuevo jugador a la variable 'jugador' de tu script
                cameraScript.jugador = cameraRoot;

                Debug.Log("¡Cámara conectada exitosamente al nuevo jugador!");
            }
            else
            {
                Debug.LogError("Error: La Main Camera no tiene el script 'Camara3Persona'.");
            }
        }
        else
        {
            Debug.LogError("Error: No se encontró 'PlayerCameraRoot' en el personaje o falta la cámara.");
        }
    }
}