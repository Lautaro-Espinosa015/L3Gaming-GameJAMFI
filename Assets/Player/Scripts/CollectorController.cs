using System.Collections.Generic;
using UnityEngine;

public class CollectorController : MonoBehaviour
{
    [Header("Recolección")]
    public List<Transform> dagasRecolectadas = new List<Transform>();
    public Transform puntoOrbita;
    public float radioOrbita = 1.5f;
    public float velocidadRotacion = 50f;

    [Header("Entrega")]
    public Transform puntoEntrega;
    public float radioEntrega = 1.2f;
    public float alturaEntrega = 1.0f;
    public float velocidadRotacionEntrega = 25f;
    public float velocidadMovimiento = 3f;

    // --- AÑADIDO: Variables de Victoria ---
    public int dagasParaGanar = 3; // ¡Define aquí cuántas dagas necesitas!
    private GameManager gameManager;
    private bool victoriaAlcanzada = false;
    // ---

    private bool entregando = false;
    private readonly List<Transform> dagasEntregadas = new();

    // --- AÑADIDO: Método Start para encontrar el GameManager ---
    private void Start()
    {
        // Busca el GameManager en la escena al iniciar
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("¡CollectorController no pudo encontrar el GameManager!");
        }
    }
    // ---

    private void Update()
    {
        if (!entregando)
        {
            OrbitarDagas(dagasRecolectadas, puntoOrbita, radioOrbita, velocidadRotacion);
        }
        OrbitarDagas(dagasEntregadas, puntoEntrega, radioEntrega, velocidadRotacionEntrega, alturaEntrega);
    }

    private void OrbitarDagas(List<Transform> lista, Transform centro, float radio, float velocidad, float alturaExtra = 0f)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            if (lista[i] == null) continue;
            float angulo = Time.time * velocidad + i * (360f / lista.Count);
            Vector3 destino = centro.position + Quaternion.Euler(0, angulo, 0) * Vector3.forward * radio;
            destino.y = centro.position.y + alturaExtra;
            lista[i].position = Vector3.Lerp(lista[i].position, destino, Time.deltaTime * 5f);
            lista[i].Rotate(Vector3.up * velocidad * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Daga"))
        {
            Debug.Log("Daga recogida: " + other.name);
            dagasRecolectadas.Add(other.transform);
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
            other.GetComponent<Collider>().enabled = false;
        }

        if (other.CompareTag("ZonaEntrega") && dagasRecolectadas.Count > 0 && !entregando)
        {
            Debug.Log("Entrando en zona de entrega...");
            StartCoroutine(EntregarDagas());
        }
    }

    private System.Collections.IEnumerator EntregarDagas()
    {
        entregando = true;

        foreach (Transform daga in dagasRecolectadas)
        {
            if (daga == null) continue;

            float t = 0f;
            Vector3 origen = daga.position;
            Vector3 destino = puntoEntrega.position + Random.insideUnitSphere * 0.3f;
            destino.y = puntoEntrega.position.y + alturaEntrega;

            while (t < 1f)
            {
                t += Time.deltaTime * velocidadMovimiento;
                daga.position = Vector3.Lerp(origen, destino, t);
                yield return null;
            }

            dagasEntregadas.Add(daga);
        }

        dagasRecolectadas.Clear();
        entregando = false;

        Debug.Log("Dagas entregadas, flotando sobre el altar.");

        // --- AÑADIDO: Chequeo de Victoria ---
        // Revisa si ya entregamos suficientes dagas y si ya no hemos ganado
        if (!victoriaAlcanzada && dagasEntregadas.Count >= dagasParaGanar)
        {
            victoriaAlcanzada = true; // Marca la victoria para no llamarla de nuevo
            Debug.Log("¡CONDICIÓN DE VICTORIA CUMPLIDA!");
            if (gameManager != null)
            {
                gameManager.ShowVictoryScreen(); // ¡Llama al GameManager!
            }
        }
        // ---
    }
}