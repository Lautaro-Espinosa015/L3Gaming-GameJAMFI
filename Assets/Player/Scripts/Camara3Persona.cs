using UnityEngine;

public class Camara3Persona : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador; // El Transform del personaje a seguir

    [Header("Posición base")]
    // X = lateral, Y = altura del pivote, Z = distancia inicial (usamos su valor absoluto)
    // Para el efecto 'Over the Shoulder' (RE4), X debe ser 0.8 a 1.2
    public Vector3 offsetBase = new Vector3(0.8f, 1.6f, -3.0f);

    [Header("Controles")]
    public float sensibilidadRotacion = 100f;
    public float sensibilidadZoomLateral = 2f;
    [Range(0.01f, 0.5f)]
    public float smoothTime = 0.05f; // ¡CLAVE ANTI-PARPADEO! Un valor bajo la mantiene estable.

    [Header("Zoom")]
    public float distanciaMin = 1.5f; // Distancia mínima para el estilo 'cercano'
    public float distanciaMax = 4.0f; // Distancia máxima para el estilo 'cercano'

    [Header("Colisión")]
    // ¡IMPORTANTE! Excluir la capa del jugador en el Inspector.
    public LayerMask capasObstaculo;
    public float margenColision = 0.4f; // Margen de seguridad antes del obstáculo

    private float rotacionX = 15f; // Ángulo vertical (Pitch)
    private float rotacionY = 0f;  // Ángulo horizontal (Yaw)
    private float distanciaActual;

    // Variable para la velocidad de SmoothDamp
    private Vector3 currentVelocity;


    void Start()
    {
        // Inicializa la distancia con el valor absoluto de Z del offsetBase
        distanciaActual = Mathf.Abs(offsetBase.z);

        // CORRECCIÓN DEL ERROR: Bloquea y oculta el cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        // 1. INPUT Y ROTACIÓN
        // ----------------------------------------------------
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotacionY += mouseX * sensibilidadRotacion * Time.deltaTime;
        rotacionX -= mouseY * sensibilidadRotacion * Time.deltaTime;
        rotacionX = Mathf.Clamp(rotacionX, -30f, 60f);

        // 2. ZOOM LATERAL (Opcional)
        // ----------------------------------------------------
        distanciaActual -= Mathf.Sign(mouseX) * Mathf.Abs(mouseX) * sensibilidadZoomLateral * Time.deltaTime;
        distanciaActual = Mathf.Clamp(distanciaActual, distanciaMin, distanciaMax);

        // 3. CÁLCULO DE POSICIONES
        // ----------------------------------------------------
        Quaternion rotacion = Quaternion.Euler(rotacionX, rotacionY, 0);

        // Define el punto de pivote (origen del rayo, a la altura del hombro/cabeza)
        Vector3 puntoDePivote = jugador.position + Vector3.up * offsetBase.y;

        // Vector que apunta hacia donde la cámara QUIERE estar, incluyendo el offset X lateral
        Vector3 offsetDeseado = rotacion * new Vector3(offsetBase.x, 0, -distanciaActual);

        // Posición ideal, sin colisión.
        Vector3 posicionDeseada = puntoDePivote + offsetDeseado;
        Vector3 posicionTarget = posicionDeseada; // Objetivo de la posición

        // 4. COLISIÓN CON RAYCAST
        // ----------------------------------------------------
        RaycastHit hit;
        float distanciaIdeal = offsetDeseado.magnitude;

        // Lanza el rayo desde el pivote
        if (Physics.Raycast(puntoDePivote, offsetDeseado.normalized, out hit, distanciaIdeal, capasObstaculo))
        {
            // Colisión detectada: Acorta la posición objetivo.
            posicionTarget = puntoDePivote + offsetDeseado.normalized * (hit.distance - margenColision);
        }

        // 5. APLICAR POSICIÓN Y ROTACIÓN (¡Con Suavizado!)
        // ----------------------------------------------------

        // SmoothDamp: Mueve la cámara suavemente. Esto elimina el parpadeo.
        transform.position = Vector3.SmoothDamp(transform.position, posicionTarget, ref currentVelocity, smoothTime);

        // La cámara siempre rota para mirar hacia el punto de pivote
        transform.LookAt(puntoDePivote);
    }
}