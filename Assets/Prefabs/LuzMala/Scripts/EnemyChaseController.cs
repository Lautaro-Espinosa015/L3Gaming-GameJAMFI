using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controla la persecuci�n de un objetivo dentro de un rango determinado.
/// Puede activarse o desactivarse mediante un flag p�blico.
/// Incluye eventos y herramientas de debug para uso en packages.
/// </summary>
[AddComponentMenu("AI/Enemy Chase Controller")]
public class EnemyChaseController : MonoBehaviour
{
    #region propiedades del script
    [Header("Persecuci�n")]
    [Tooltip("Objetivo a perseguir. Si est� vac�o busca por tag 'player'")]
    [SerializeField] private Transform target;       
    [SerializeField] private float detectionRadius = 10f;  // Radio de detecci�n
    [SerializeField] private float moveSpeed = 3f;         // Velocidad de persecuci�n
    [SerializeField] private bool canChase = false;        // Habilita o no la persecuci�n

    [Header("Rotaci�n suave")]
    [SerializeField] private float rotationSpeed = 5f;  // el gameobect debe girar hacia el target

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
        // Si no se asign� un target, busca autom�ticamente por tag
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    private void Update()
    {
        if (!canChase || target == null)
            return;

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
        }

        if (isTargetInRange)
        {
            ChaseTarget();
        }
    }
    #endregion
    #region Logica de m�todos
    /// <summary>
    /// Realiza un peque�o acercamiento del objeto hacia hacia el target
    /// </summary>
    private void ChaseTarget()
    {
        // Calcular direcci�n
        Vector3 direction = (target.position - transform.position).normalized;

        // Movimiento hacia el objetivo
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Rotaci�n suave hacia el objetivo
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, 
                               lookRotation, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Permite activar o desactivar la persecuci�n externamente.
    /// </summary>
    public void SetChaseState(bool state)
    {
        canChase = state;
    }

    /// <summary>
    /// Permite asignar un objetivo din�micamente.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebug) return;

        Gizmos.color = canChase ? activeColor : idleColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
    #endregion
}
