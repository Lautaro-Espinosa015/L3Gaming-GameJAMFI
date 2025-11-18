using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Componentes Opcionales")]
    private Animator animator;

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

    void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");

        // 1. Desactivar el GPS (NavMeshAgent) para que deje de intentar moverse
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        // 2. Desactivar el CEREBRO (Duende_AI_Leashed) para que deje de mandar órdenes
        var aiScript = GetComponent<Duende_AI_Leashed>();
        if (aiScript != null) aiScript.enabled = false;

        // 3. Desactivar el COLLIDER para que no se pueda golpear más
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4. (Opcional) Animación de muerte
        /* if (animator != null) {
             animator.SetTrigger("Die");
             Destroy(gameObject, 2f); // Esperar 2 segundos antes de borrar
             return;
        }
        */

        // 5. Destruir el objeto (con un pequeño retraso de seguridad)
        Destroy(gameObject, 0.1f);
    }
}