using UnityEngine;
using UnityEngine.AI; // ¡Importante para el NavMesh!

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Duende_AI_Leashed : MonoBehaviour
{
    // --- Componentes ---
    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;

    // --- Variables de Configuración ---
    [Header("Configuración de IA")]
    public float detectionRange = 15f; // Rango para empezar a perseguir
    public float leashRange = 25f;     // Rango máximo desde casa

    // --- Variables de Estado ---
    private Vector3 startPosition; // Su "casa"
    private bool isChasingLogic = false; // Variable interna para la lógica de decisión

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Buscar al jugador por Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        startPosition = transform.position;

        // Estado inicial: Quieto
        agent.isStopped = true;
    }

    void Update()
    {
        // 1. SEGURIDAD CRÍTICA:
        // Si el jugador no existe, o el agente murió/se apagó, no hacer nada.
        // Esto evita los errores rojos en la consola.
        if (player == null || agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        // --- LÓGICA DE DECISIÓN (CEREBRO) ---

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceToHome = Vector3.Distance(transform.position, startPosition);

        // Decidir si debe perseguir o volver
        if (!isChasingLogic && distanceToPlayer <= detectionRange)
        {
            isChasingLogic = true; // Empezar a perseguir
        }
        else if (isChasingLogic && (distanceToPlayer > detectionRange || distanceToHome > leashRange))
        {
            isChasingLogic = false; // Rendirse y volver a casa
        }

        // --- LÓGICA DE MOVIMIENTO (GPS) ---

        if (isChasingLogic)
        {
            // ESTADO: PERSEGUIR JUGADOR
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // ESTADO: VOLVER A CASA
            if (distanceToHome > 1.0f) // Si está lejos de casa (margen de 1 metro)
            {
                agent.isStopped = false;
                agent.SetDestination(startPosition);
            }
            else // Si ya llegó a casa
            {
                agent.isStopped = true;
            }
        }

        // --- LÓGICA DE ANIMACIÓN (CORRECCIÓN "MOONWALKING") ---

        // Usamos la velocidad física real y un umbral de 0.5f para evitar que "corra quieto".
        // También usamos el nombre 'isChasing' que configuramos en tu Animator.

        if (agent.velocity.sqrMagnitude > 0.5f && !agent.isStopped)
        {
            anim.SetBool("isChasing", true); // Activa RUN
        }
        else
        {
            anim.SetBool("isChasing", false); // Activa IDLE
        }
    }

    // Dibujar los rangos en el editor
    private void OnDrawGizmosSelected()
    {
        if (startPosition == Vector3.zero) startPosition = transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, 1.0f); // Casa

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Rango detección (Corregido)

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPosition, leashRange); // Límite correa
    }
}