using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Configuración")]
    public Animator animator;
    public Transform attackPoint;
    public float attackRange = 1.0f;
    public LayerMask enemyLayers;
    public int attackDamage = 1;

    void Update()
    {
        // Detectar Tecla F o Clic Izquierdo
        if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0))
        {
            RealizarPatada();
        }
    }

    void RealizarPatada()
    {
        // Activar animación
        if (animator != null) animator.SetTrigger("Kick");

        // NOTA: Si usas Eventos de Animación, borra la línea de abajo (HitEvent).
        // Si NO usas Eventos, déjala para que el daño sea instantáneo.
    }

    // Esta función hace el daño
    public void HitEvent()
    {
        // Crear esfera invisible de detección
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Golpeaste a: " + enemy.name);

            // Buscar el script de vida del enemigo
            EnemyHealth saludEnemigo = enemy.GetComponent<EnemyHealth>();

            if (saludEnemigo != null)
            {
                saludEnemigo.TakeDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}