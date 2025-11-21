using UnityEngine;

public class CamaraTerceraPersonaConZoomYColision : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador; // El Transform del personaje a seguir

    [Header("Posición base")]
    // X = lateral, Y = altura del pivote, Z = distancia inicial (usamos su valor absoluto)
    public Vector3 offsetBase = new Vector3(0, 1.5f, -5);

    [Header("Controles")]
    public float sensibilidadRotacion = 100f;
    public float sensibilidadZoomLateral = 2f; // Zoom al mover el mouse lateralmente
    [Range(0.01f, 0.5f)]
    public float smoothTime = 0.05f; // Tiempo de suavizado. ¡Clave anti-parpadeo!

    [Header("Zoom")]
    public float distanciaMin = 2f;
    public float distanciaMax = 8f;

    [Header("Colisión")]
    // ¡IMPORTANTE! Asegúrate de EXCLUIR la capa del jugador en el Inspector.
    public LayerMask capasObstaculo;
    public float margenColision = 0.3f; // Distancia a la que la cámara se detiene antes del obstáculo

    private float rotacionX = 15f; // Ángulo vertical (Pitch)
    private float rotacionY = 0f;  // Ángulo horizontal (Yaw)
    private float distanciaActual;

    // Variable para almacenar la velocidad actual, necesaria para SmoothDamp
    private Vector3 currentVelocity;


    void Start()
    {
        // Inicializa la distancia con el valor absoluto de Z del offsetBase
        distanciaActual = Mathf.Abs(offsetBase.z);

        // CORRECCIÓN: Usamos CursorLockMode.Locked para evitar el error.
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

        // Define el punto de pivote (origen del rayo)
        Vector3 puntoDePivote = jugador.position + Vector3.up * offsetBase.y;

        // Vector que apunta hacia donde la cámara QUIERE estar
        Vector3 offsetDeseado = rotacion * new Vector3(offsetBase.x, 0, -distanciaActual);

        // Posición ideal, sin colisión.
        Vector3 posicionDeseada = puntoDePivote + offsetDeseado;
        Vector3 posicionTarget = posicionDeseada; // Usaremos esta como objetivo

        // 4. COLISIÓN CON RAYCAST
        // ----------------------------------------------------
        RaycastHit hit;
        float distanciaIdeal = offsetDeseado.magnitude;

        // Lanzamos el rayo desde el punto de pivote en la dirección deseada
        if (Physics.Raycast(puntoDePivote, offsetDeseado.normalized, out hit, distanciaIdeal, capasObstaculo))
        {
            // Colisión detectada: Acortamos la posición objetivo.
            posicionTarget = puntoDePivote + offsetDeseado.normalized * (hit.distance - margenColision);
        }

        // 5. APLICAR POSICIÓN Y ROTACIÓN (¡Con Suavizado!)
        // ----------------------------------------------------

        // SmoothDamp: Mueve la cámara suavemente a la posición objetivo (ideal o corregida).
        // Esto elimina el parpadeo causado por los saltos de colisión.
        transform.position = Vector3.SmoothDamp(transform.position, posicionTarget, ref currentVelocity, smoothTime);

        // La cámara siempre rota para mirar hacia el punto de pivote
        transform.LookAt(puntoDePivote);
    }
}