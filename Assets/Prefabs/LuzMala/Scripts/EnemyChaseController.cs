using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controla la persecución de un objetivo dentro de un rango determinado.
/// Puede activarse o desactivarse mediante un flag público.
/// Incluye eventos y herramientas de debug para uso en packages.
/// </summary>
[AddComponentMenu("AI/Enemy Chase Controller")]
public class EnemyChaseController : MonoBehaviour
{
    #region propiedades del script

    [Header("Persecución")]
    [SerializeField] private Transform target;
    [SerializeField] private float baseDetectionRadius = 10f; // rango de persecución
    [SerializeField] private float baseMoveSpeed = 3f;  // Velocidad de persecución
    [SerializeField] private float stoppingDistance = 2f;// Distancia para frenar
    [SerializeField] private bool canChase = false;  // Habilita o no la persecución


    [Header("Escalado por dagas")]
    public CollectorController collector;
    public float detectionIncreasePerDaga = 1f;
    public float speedIncreasePerDaga = 0.5f;
    private int dagas;
    [Header("Post-Processing")]
    public Volume postProcessVolume;
    public ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;


         

    [Header("Rotación suave")]
    [SerializeField] private float rotationSpeed = 5f;   // el gameobect debe girar hacia el target

    [Header("Eventos")]
    public UnityEvent OnTargetEnterRange;
    public UnityEvent OnTargetExitRange;

    [Header("Debug visual")]
    [SerializeField] private bool showDebug = true;
    [SerializeField] private Color activeColor = Color.cyan;
    [SerializeField] private Color idleColor = Color.gray;

    private bool isTargetInRange = false;

    // Nueva propiedad solo de lectura
    public bool IsTargetInRange => isTargetInRange;

    #endregion

    #region ciclo de vida del sccript
    private void Start()
    {
        // Si no se asignó un target, busca automáticamente por tag
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out chromaticAberration);
            postProcessVolume.profile.TryGet(out lensDistortion);
        }

    }

    private void Update()
    {
        if (!canChase || target == null)
            return;
        
        dagas = collector != null ? collector.dagasRecolectadas.Count : 0;

        float detectionRadius = baseDetectionRadius + (dagas * detectionIncreasePerDaga);
        float moveSpeed = baseMoveSpeed + (dagas * speedIncreasePerDaga);


        float distance = Vector3.Distance(transform.position, target.position);
        bool inRange = distance <= detectionRadius;

        if (inRange && !isTargetInRange)
        {
            // Evento: objetivo entra en rango
            isTargetInRange = true;
            OnTargetEnterRange?.Invoke();
        }
        else if (!inRange && isTargetInRange)
        {
            // Evento: objetivo sale de rango
            isTargetInRange = false;
            OnTargetExitRange?.Invoke();
            ResetVisualEffects();
        }

        if (isTargetInRange)
        {
            // --- LÍNEA MODIFICADA ---
            // Le pasamos la distancia al método ChaseTarget

            ChaseTarget(distance, moveSpeed);
            ApplyVisualEffects(dagas);

        }
    }
    #endregion
    #region Logica de métodos
    /// <summary>
    /// Realiza un pequeño acercamiento del objeto hacia hacia el target
    /// </summary>
    // --- LÍNEA MODIFICADA ---
    private void ChaseTarget(float distanceToTarget, float moveSpeed) // Ahora recibe la distancia
    {
        // Calcular dirección
        Vector3 direction = (target.position - transform.position).normalized;

        // --- BLOQUE MODIFICADO ---
        // Solo nos movemos si estamos más lejos que la distancia de parada
        if (distanceToTarget > stoppingDistance)
        {
            // Movimiento hacia el objetivo
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        // --- FIN DEL BLOQUE MODIFICADO ---


        // Rotación suave hacia el objetivo (la rotación siempre ocurre)
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                    lookRotation, rotationSpeed * Time.deltaTime);
    }


    private void ApplyVisualEffects(int dagas)
    {
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = Mathf.Clamp(dagas * 0.2f, 0f, 1f);

        if (lensDistortion != null)
            lensDistortion.intensity.value = Mathf.Clamp(-dagas * 0.1f, -0.5f, 0f);
    }


    private void ResetVisualEffects()
    {
        if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;
        if (lensDistortion != null) lensDistortion.intensity.value = 0f;
    }



    /// <summary>
    /// Permite activar o desactivar la persecución externamente.
    /// </summary>
    public void SetChaseState(bool state)
    {
        canChase = state;
    }

    /// <summary>
    /// Permite asignar un objetivo dinámicamente.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;

        Gizmos.color = canChase ? activeColor : idleColor;
        Gizmos.DrawWireSphere(transform.position, baseDetectionRadius);

        // --- AÑADIDO: Dibujar el círculo de "freno" ---
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        // ---

        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
    #endregion
}