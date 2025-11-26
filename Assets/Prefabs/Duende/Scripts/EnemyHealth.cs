using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 1;
    private int currentHealth;

    [Header("Componentes Opcionales")]
    private Animator animator;

    // --- VARIABLES DE DROP ---
    [Header("Drop al Morir")]
    public GameObject dropOnDeathPrefab; // El Prefab que queremos instanciar
    public float destroyDelay = 0.1f; // Retraso antes de destruir el enemigo

    // NUEVO: Tiempo de vida del objeto que aparece
    [Header("Configuración del Loot")]
    public float lootLifetime = 3f; // ¡3 segundos de vida para el drop!
    // -----------------------

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // Ya está muerto

        currentHealth -= damage;
        Debug.Log(gameObject.name + " recibió daño. Vida: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");

        // 1. Desactivar el GPS (NavMeshAgent)
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // 2. Desactivar el CEREBRO (Duende_AI_Leashed)
        var aiScript = GetComponent<Duende_AI_Leashed>();
        if (aiScript != null) aiScript.enabled = false;

        // 3. Desactivar el COLLIDER
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Programamos la instanciación después del retraso de destrucción del enemigo
        if (dropOnDeathPrefab != null)
        {
            Invoke("SpawnDrop", destroyDelay);
        }

        // Destruir el objeto enemigo después del retraso
        Destroy(gameObject, destroyDelay);
    }

    // --- FUNCIÓN DE SPAWN Y AUTODESTRUCCIÓN ---
    private void SpawnDrop()
    {
        // 1. Instanciar el objeto y capturar la referencia
        GameObject instantiatedDrop = Instantiate(dropOnDeathPrefab, transform.position, transform.rotation);

        // 2. Programar la autodestrucción del objeto recién creado (el drop)
        // Se destruirá después de 'lootLifetime' segundos
        Destroy(instantiatedDrop, lootLifetime);

        Debug.Log("¡Objeto drop instanciado después del retraso! Se autodestruirá en " + lootLifetime + " segundos.");
    }
    // ------------------------------------------
}
