using UnityEngine;

public class Camara3Persona : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;

    [Header("Posición base")]
    public Vector3 offsetBase = new Vector3(1.2f, 1.8f, -3.5f);

    [Header("Controles")]
    public float sensibilidadRotacion = 80f;
    public float sensibilidadZoomLateral = 2f;
    [Range(0.01f, 0.5f)]
    public float smoothTime = 0.05f; // Suavizado

    [Header("Zoom")]
    public float distanciaMin = 1.5f;
    public float distanciaMax = 4.0f;

    [Header("Colisión Avanzada")]
    public LayerMask capasObstaculo;
    public float radioColision = 0.2f; // ¡NUEVO! Grosor de la cámara (La "Pelota")
    public float margenColision = 0.1f; // Espacio extra para que no atraviese

    private float rotacionX = 15f;
    private float rotacionY = 0f;
    private float distanciaActual;
    private Vector3 currentVelocity;

    void Start()
    {
        distanciaActual = Mathf.Abs(offsetBase.z);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (jugador == null) return;

        // 1. INPUT
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotacionY += mouseX * sensibilidadRotacion * Time.deltaTime;
        rotacionX -= mouseY * sensibilidadRotacion * Time.deltaTime;
        rotacionX = Mathf.Clamp(rotacionX, -30f, 60f);

        // 2. ZOOM
        distanciaActual -= Mathf.Sign(mouseX) * Mathf.Abs(mouseX) * sensibilidadZoomLateral * Time.deltaTime;
        distanciaActual = Mathf.Clamp(distanciaActual, distanciaMin, distanciaMax);

        // 3. CÁLCULO DE POSICIÓN
        Quaternion rotacion = Quaternion.Euler(rotacionX, rotacionY, 0);
        Vector3 puntoDePivote = jugador.position + Vector3.up * offsetBase.y;

        // Dirección hacia donde queremos ir
        Vector3 direccionCamara = (rotacion * new Vector3(offsetBase.x, 0, -distanciaActual)).normalized;
        float distanciaDeseada = (rotacion * new Vector3(offsetBase.x, 0, -distanciaActual)).magnitude;

        // 4. COLISIÓN MEJORADA (SPHERECAST)
        // Lanzamos una "pelota" en lugar de un rayo.
        RaycastHit hit;
        Vector3 posicionFinal;

        if (Physics.SphereCast(puntoDePivote, radioColision, direccionCamara, out hit, distanciaDeseada, capasObstaculo))
        {
            // Si golpeamos algo, nos quedamos en el punto de impacto (menos un margen)
            // Mathf.Max evita que la cámara se meta DENTRO del jugador (mínimo 0.2f de distancia)
            float distanciaChoque = Mathf.Max(hit.distance - margenColision, 0.2f);
            posicionFinal = puntoDePivote + (direccionCamara * distanciaChoque);
        }
        else
        {
            // Si no hay obstáculo, vamos a la posición ideal
            posicionFinal = puntoDePivote + (direccionCamara * distanciaDeseada);
        }

        // 5. APLICAR
        transform.position = Vector3.SmoothDamp(transform.position, posicionFinal, ref currentVelocity, smoothTime);
        transform.LookAt(puntoDePivote);
    }
}