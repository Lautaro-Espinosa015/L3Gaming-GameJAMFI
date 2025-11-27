
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Arrastra aquí tus prefabs (Antonio, Sosa, Sam, Quispe) en el MISMO ORDEN que en el menú.")]
    public GameObject[] characterPrefabs;

    [Tooltip("Punto donde aparece el jugador.")]
    public Transform spawnPoint;

    [Header("Cámara")]
    [Tooltip("Arrastra aquí tu Main Camera (la que tiene el script Camara3Persona).")]
    public GameObject mainCameraObject;

    // Jugador actual instanciado
    private GameObject currentPlayer;

    void Start()
    {
        // Al iniciar, cargar el personaje guardado (por defecto 0)
        int savedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        SpawnCharacter(savedIndex);
    }

    void Update()
    {
        // Lee teclas 1..9 y Numpad1..Numpad9
        int? pressedIndex = GetPressedIndexFromKeyboard();

        if (pressedIndex.HasValue)
        {
            int idx = pressedIndex.Value;

            // Protección por rango
            if (idx >= 0 && idx < characterPrefabs.Length)
            {
                // Guarda la selección y respawnea
                PlayerPrefs.SetInt("SelectedCharacter", idx);
                PlayerPrefs.Save();

                SpawnCharacter(idx);
            }
            else
            {
                Debug.LogWarning($"Tecla de selección '{idx + 1}' fuera de rango. Prefabs cargados: {characterPrefabs.Length}");
            }
        }
    }

    /// <summary>
    /// Devuelve el índice (base 0) según la tecla presionada 1..9 o Numpad1..Numpad9.
    /// Si no se presionó ninguna tecla válida, devuelve null.
    /// </summary>
    private int? GetPressedIndexFromKeyboard()
    {
        // Teclas de la fila superior
        if (Input.GetKeyDown(KeyCode.Alpha1)) return 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) return 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) return 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) return 3;

        // Teclado numérico
        if (Input.GetKeyDown(KeyCode.Keypad1)) return 0;
        if (Input.GetKeyDown(KeyCode.Keypad2)) return 1;
        if (Input.GetKeyDown(KeyCode.Keypad3)) return 2;
        if (Input.GetKeyDown(KeyCode.Keypad4)) return 3;

        return null;
    }

    // Esta función se llama al inicio y también desde el teclado
    public void SpawnCharacter(int index)
    {
        // 1) Si ya hay un jugador, borrarlo
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        // Protege y normaliza el índice
        if (index < 0 || index >= characterPrefabs.Length)
        {
            index = 0; // fallback
        }

        // 2) Instanciar
        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        currentPlayer = Instantiate(characterPrefabs[index], pos, rot);

        // 3) Conectar cámara personalizada
        Transform cameraRoot = currentPlayer.transform.Find("PlayerCameraRoot");

        if (cameraRoot != null && mainCameraObject != null)
        {
            var cameraScript = mainCameraObject.GetComponent<Camara3Persona>();

            if (cameraScript != null)
            {
                cameraScript.jugador = cameraRoot;
                Debug.Log($"Cámara conectada al personaje {index} ({currentPlayer.name}).");
            }
            else
            {
                Debug.LogError("La Main Camera no tiene el script 'Camara3Persona'.");
            }
        }
        else
        {
            Debug.LogError("No se encontró 'PlayerCameraRoot' en el personaje o falta la cámara.");
        }
    }
}
