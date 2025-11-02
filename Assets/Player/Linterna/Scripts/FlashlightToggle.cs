using UnityEngine; // Importa el espacio de nombres necesario para acceder a componentes de Unity como Light, MonoBehaviour, etc.
public class FlashlightToggle : MonoBehaviour // Define la clase FlashlightToggle, que hereda de MonoBehaviour para funcionar como script en Unity.
{
    private static Light flashlight; // Variable estática que almacena el componente Light de la linterna. Se comparte entre instancias.
    private static bool isOn = true; // Estado actual de la linterna. true = encendida, false = apagada.
    void Awake() // Método llamado automáticamente por Unity al iniciar el objeto. Se usa para inicialización temprana.
    {
        flashlight = GetComponent<Light>(); // Busca y asigna el componente Light en el mismo GameObject donde está este script.
        if (flashlight == null) // Verifica si no se encontró el componente Light.
        {
            Debug.LogError("FlashlightToggle: No se encontró componente Light en el objeto."); // Muestra error en consola si falta el componente.
            enabled = false; // Desactiva el script para evitar errores posteriores.
            return; // Sale del método Awake.
        }
        flashlight.enabled = isOn; // Aplica el estado inicial de la linterna (encendida o apagada).
    }
    void Update() // Método llamado una vez por frame. Se usa para detectar entrada del jugador.
    {
        if (Input.GetKeyDown(KeyCode.F)) // Verifica si el jugador presionó la tecla F en este frame.
        {
            ToggleFlashlight(); // Llama al método que alterna el estado de la linterna.
        }
    }
    public static void ToggleFlashlight() // Método público y estático para alternar el estado de la linterna.
    {
        if (flashlight == null) return; // Si no hay linterna asignada, no hace nada.

        isOn = !isOn; // Invierte el estado actual: si estaba encendida, la apaga; si estaba apagada, la enciende.
        flashlight.enabled = isOn; // Aplica el nuevo estado al componente Light.
    }
    public static void SetFlashlight(bool state) // Método público para forzar el estado de la linterna desde otros scripts.
    {
        if (flashlight == null) return; // Si no hay linterna asignada, no hace nada.

        isOn = state; // Asigna el estado recibido (true o false).
        flashlight.enabled = isOn; // Aplica el estado al componente Light.
    }
    public static bool IsFlashlightOn() // Método público que devuelve el estado actual de la linterna.
    {
        return isOn; // Retorna true si está encendida, false si está apagada.
    }
}
