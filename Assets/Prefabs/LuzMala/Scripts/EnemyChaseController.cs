using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[AddComponentMenu("AI/Enemy Chase Controller")]
public class EnemyChaseController : MonoBehaviour
{
    #region Propiedades del Script

    [Header("Persecución Base")]
    [SerializeField] private Transform target;
    [SerializeField] private float baseDetectionRadius = 10f; // Rango base
    [SerializeField] private float baseMoveSpeed = 3f;        // Velocidad base

    // --- CORRECCIÓN CLAVE: Distancia de frenado ---
    [Tooltip("Distancia a la que frena para no empujar al jugador")]
    [SerializeField] private float stoppingDistance = 2f;

    [SerializeField] private bool canChase = false;

    [Header("Escalado por Dagas (Dificultad)")]
    public CollectorController collector; // Será autoasignado por tag
    [SerializeField] private string collectorTag = "Collector"; // <- Tag configurable
    [SerializeField] private float collectorSearchInterval = 0.75f; // <- Evita buscar cada frame
    private float nextCollectorSearchTime;
    public float detectionIncreasePerDaga = 1f; // Cuánto aumenta el rango por daga
    public float speedIncreasePerDaga = 0.5f;   // Cuánto aumenta la velocidad por daga
    private int dagas;

    [Header("Post-Processing (Terror)")]
    public Volume postProcessVolume;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;

    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Eventos")]
    public UnityEvent OnTargetEnterRange;
    public UnityEvent OnTargetExitRange;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private Color activeColor = Color.cyan;
    [SerializeField] private Color idleColor = Color.gray;

    private bool isTargetInRange = false;

    #endregion

    #region Ciclo de Vida

    private void Start()
    {
        // Buscar al jugador si no está asignado
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
            // Intentar asignar Collector por tag al empezar
            TryAssignCollectorByTag();

        }


       
        


        // Configurar Post-Processing
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out lensDistortion);
        }
    }

    private void Update()
    {

        // Reintento suave si Collector se crea más tarde
        if (collector == null && Time.time >= nextCollectorSearchTime)
        {
            TryAssignCollectorByTag();
            nextCollectorSearchTime = Time.time + collectorSearchInterval;
        }

        if (!canChase) return;

        // SEGURIDAD: Re-buscar al jugador si se perdió (por cambio de personaje)
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                TryAssignCollectorByTag();
            }
            else return; // Si no hay jugador, no hacemos nada
        }

        // 1. CALCULAR DIFICULTAD DINÁMICA
        dagas = collector != null ? collector.dagasRecolectadas.Count : 0;
        float currentDetectionRadius = baseDetectionRadius + (dagas * detectionIncreasePerDaga);
        float currentMoveSpeed = baseMoveSpeed + (dagas * speedIncreasePerDaga);

        // 2. CALCULAR DISTANCIA
        float distance = Vector3.Distance(transform.position, target.position);
        bool inRange = distance <= currentDetectionRadius;

        // 3. GESTIÓN DE ESTADOS Y EVENTOS
        if (inRange && !isTargetInRange)
        {
            isTargetInRange = true;
            OnTargetEnterRange?.Invoke();
        }
        else if (!inRange && isTargetInRange)
        {
            isTargetInRange = false;
            OnTargetExitRange?.Invoke();
            ResetVisualEffects(); // Apagar efectos si escapas
        }

        // 4. EJECUTAR PERSECUCIÓN
        if (isTargetInRange)
        {
            ChaseTarget(distance, currentMoveSpeed);
            ApplyVisualEffects(dagas);
        }
    }

    #endregion

    #region Lógica de Métodos



    private void TryAssignCollectorByTag()
    {
        // Evitar trabajo si ya está asignado
        if (collector != null) return;

        // Buscar por tag (asegurate que el GameObject tenga el tag "Collector")
        GameObject collectorGO = GameObject.FindGameObjectWithTag(collectorTag);
        if (collectorGO != null)
        {
            collector = collectorGO.GetComponent<CollectorController>();
            if (collector == null)
            {
                Debug.LogWarning($"El objeto con tag '{collectorTag}' no tiene CollectorController.");
            }
            else
            {
                // Opcional: log para confirmar
                // Debug.Log("Collector asignado por tag correctamente.");
            }
        }
    }

    private void ChaseTarget(float distanceToTarget, float speed)
    {
        // Calcular dirección hacia el jugador
        Vector3 direction = (target.position - transform.position).normalized;

        // --- CORRECCIÓN FÍSICA: EL FRENO ---
        // Solo nos movemos si estamos LEJOS. Si estamos cerca (<= 2m), frenamos.
        // Esto evita que el enemigo empuje al jugador y lo mande al cielo.
        if (distanceToTarget > stoppingDistance)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
        // -----------------------------------

        // La rotación siempre ocurre para mirarte amenazantemente
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyVisualEffects(int dagasCantidad)
    {
        // Aumentar efectos visuales según cantidad de dagas
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Clamp(dagasCantidad * 0.2f, 0f, 1f);

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Clamp(-dagasCantidad * 0.1f, -0.5f, 0f);
    }

    private void ResetVisualEffects()
    {
        if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;
        if (lensDistortion != null) lensDistortion.intensity.value = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;

        // Dibujar Rango de Detección (Variable según dagas no visible en editor, muestra base)
        Gizmos.color = canChase ? activeColor : idleColor;
        Gizmos.DrawWireSphere(transform.position, baseDetectionRadius);

        // Dibujar Rango de Freno (ROJO)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }

    #endregion
}