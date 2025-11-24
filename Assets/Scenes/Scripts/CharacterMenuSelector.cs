using UnityEngine;
using TMPro; // Necesario para el Dropdown TextMeshPro

public class CharacterMenuSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown; // Arrastra el Dropdown aquí
    public CharacterSpawner spawner; // Arrastra el @PlayerManager aquí

    void Start()
    {
        // 1. Cargar el valor guardado en el Dropdown visual
        int savedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        dropdown.value = savedIndex;

        // 2. Escuchar cambios: Cuando el jugador elija otro, ejecutar "OnCharacterChanged"
        dropdown.onValueChanged.AddListener(OnCharacterChanged);
    }

    public void OnCharacterChanged(int index)
    {
        // Guardar en memoria
        PlayerPrefs.SetInt("SelectedCharacter", index);
        PlayerPrefs.Save();

        // Decirle al Spawner que cambie el modelo en tiempo real
        if (spawner != null)
        {
            spawner.SpawnCharacter(index);
        }
    }
}