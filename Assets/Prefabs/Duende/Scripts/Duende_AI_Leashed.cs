using UnityEngine;
using UnityEngine.AI; // ¡Importante para el NavMesh!

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Duende_AI_Leashed : MonoBehaviour
{
    // --- Componentes ---
    private NavMeshAgent agent; // El "GPS"
    private Animator anim;      // El "Titiritero"
    private Transform player;

    // --- Variables de "Correa" (Leash) ---
    [Header("Configuración de IA")]
    public float detectionRange = 15f;
    public float leashRange = 25f;

    // --- Variables de Estado ---
    private Vector3 startPosition;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPosition = transform.position;

        // ¡Ya no necesitamos poner el agent.isStopped = true aquí!
        // Dejamos que el Update lo controle todo.
    }

    void Update()
    {
        if (player == null) return;

        // Calcular distancias
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceToHome = Vector3.Distance(transform.position, startPosition);

        // --- LÓGICA DE DECISIÓN ---

        // 1. ¿Debo empezar a perseguir?
        if (!isChasing && distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }

        // 2. ¿Debo dejar de perseguir?
        if (isChasing && (distanceToPlayer > detectionRange || distanceToHome > leashRange))
        {
            isChasing = false;
        }

        // --- LÓGICA DE ACCIÓN (MOVIMIENTO) ---

        if (isChasing)
        {
            // --- ESTADO: PERSEGUIR ---
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // --- ESTADO: NO PERSEGUIR (Volviendo a casa o quieto) ---
            if (distanceToHome > 1.0f) // ¿Estoy lejos de casa?
            {
                // Sí, estoy lejos -> Vuelve a casa
                agent.isStopped = false;
                agent.SetDestination(startPosition);
            }
            else
            {
                // No, estoy en casa -> Quédate quieto
                agent.isStopped = true;
                transform.position = startPosition; // (Opcional) "Snap" a la posición exacta
            }
        }

        // --- LÓGICA DE ANIMACIÓN (¡LA PARTE NUEVA!) ---
        // Esta es la única línea que controla TODAS las animaciones.
        // Si el "GPS" (agent) NO está parado, entonces "isMoving" es true.
        anim.SetBool("isMoving", !agent.isStopped);
    }

    // (El OnDrawGizmosSelected sigue igual, no lo borres)
    private void OnDrawGizmosSelected()
    {
        if (startPosition == Vector3.zero)
            startPosition = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPosition, 1.0f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(startPosition, leashRange);
    }
}
