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

        // Intento inicial de buscar al jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        startPosition = transform.position;

        // Estado inicial: Quieto
        agent.isStopped = true;
    }

    void Update()
    {
        // 1. SEGURIDAD CRÍTICA: Si el agente murió o se apagó, no hacer nada
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        // 2. BUSCAR JUGADOR (La corrección para el Spawner)
        // Si la variable 'player' está vacía, la buscamos de nuevo.
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform; // ¡Lo encontramos!
            }
            else
            {
                return; // Todavía no existe, volvemos a intentar en el próximo frame
            }
        }

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
            if (distanceToHome > 1.0f) // Si está lejos de casa
            {
                agent.isStopped = false;
                agent.SetDestination(startPosition);
            }
            else // Si ya llegó a casa
            {
                agent.isStopped = true;
            }
        }

        // --- LÓGICA DE ANIMACIÓN ---

        // Usamos la velocidad física real y un umbral de 0.5f para evitar que "corra quieto".
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
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Rango detección

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPosition, leashRange); // Límite correa
    }
}