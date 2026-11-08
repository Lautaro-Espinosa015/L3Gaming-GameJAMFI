using UnityEngine;
using UnityEngine.UI; // Necesario para controlar la UI -> Image
using TMPro;

public class CharacterMenuSelector : MonoBehaviour
{
    [Header("Configuración Original")]
    public TMP_Dropdown dropdown;
    public CharacterSpawner spawner;

    [Header("Configuración Visual 2D")]
    public Image marcoImagen;
    public Sprite[] retratosPersonajes;

    void Start()
    {
        // 1. Cargar la selección guardada
        int savedIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        dropdown.value = savedIndex;

        // 2. Mostrar la foto correcta al iniciar
        ActualizarFoto(savedIndex);

        // 3. Escuchar los cambios del Dropdown
        dropdown.onValueChanged.AddListener(OnCharacterChanged);
    }

    public void OnCharacterChanged(int index)
    {
        PlayerPrefs.SetInt("SelectedCharacter", index);
        PlayerPrefs.Save();

        // Actualizar la foto al momento
        ActualizarFoto(index);

        if (spawner != null)
        {
            spawner.SpawnCharacter(index);
        }
    }

    private void ActualizarFoto(int index)
    {
        // Verifica que la imagen exista en la lista para evitar errores
        if (marcoImagen != null && retratosPersonajes != null && index >= 0 && index < retratosPersonajes.Length)
        {
            marcoImagen.sprite = retratosPersonajes[index];
        }
    }
}