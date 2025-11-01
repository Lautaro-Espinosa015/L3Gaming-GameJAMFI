using System.Collections; // Necesario para las Corutinas (el cooldown)
using UnityEngine;
using UnityEngine.UI; // Necesario para controlar las Imágenes

public class PlayerHealth : MonoBehaviour
{
    // --- Variables de Salud ---
    public int maxHealth = 5;
    private int currentHealth;

    // --- Variables de UI ---
    // Arrastraremos las 5 imágenes de vida aquí
    public Image[] lifeIcons;
    // Arrastraremos el panel "Game Over" aquí
    public GameObject gameOverPanel;

    // --- Variables de Cooldown (Invencibilidad) ---
    private bool canTakeDamage = true;
    public float invincibilityTime = 1.0f; // 1 segundo de invencibilidad después de ser golpeado

    void Start()
    {
        // 1. Empezamos con la vida máxima
        currentHealth = maxHealth;

        // 2. Nos aseguramos que la pantalla de Game Over esté apagada
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 3. Actualizamos la UI para mostrar 5 vidas
        UpdateHealthUI();
    }

    // Esta función se llama automáticamente cuando otro Collider entra en contacto con este
    // Asegúrate de que el Player (o el Enemigo) tenga un Rigidbody
    // Cerca de la línea 40
    void OnTriggerEnter(Collider other)
    {
        // Verificamos si chocamos con "Enemy" Y si podemos recibir daño
        if (other.gameObject.CompareTag("Enemy") && canTakeDamage)
        {
            // ¡La línea de abajo es nueva! Es para depurar
            Debug.Log("¡TRIGGER DETECTADO! El enemigo me tocó.");

            // Si chocamos con un enemigo, recibimos daño
            TakeDamage(1);
        }
    }

    // Función pública para recibir daño
    public void TakeDamage(int damage)
    {
        // Si ya estamos muertos, no hacemos nada
        if (currentHealth <= 0) return;

        // Restamos la vida
        currentHealth -= damage;

        // Nos volvemos invencibles temporalmente
        canTakeDamage = false;

        // Actualizamos la UI
        UpdateHealthUI();

        // Comprobamos si el jugador ha muerto
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Si no morimos, iniciamos el cooldown de invencibilidad
            StartCoroutine(InvincibilityCooldown());
        }
    }

    void UpdateHealthUI()
    {
        // Este bucle recorre todos nuestros iconos de vida
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            // Comparamos el índice del icono (i) con la vida actual
            if (i < currentHealth)
            {
                // Si el índice es MENOR que la vida, mostramos el icono
                lifeIcons[i].enabled = true;
            }
            else
            {
                // Si el índice es MAYOR o IGUAL, lo ocultamos
                lifeIcons[i].enabled = false;
            }
        }
    }

    void Die()
    {
        // 1. Mostramos la pantalla de Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 2. Pausamos el juego
        Time.timeScale = 0f;

        // 3. (Opcional) Desactivamos el script de movimiento del jugador
        // Si tu script de movimiento se llama "PlayerMovement", descomenta la siguiente línea:
        // GetComponent<PlayerMovement>().enabled = false;
    }

    // Esta es una Corutina que espera un tiempo
    IEnumerator InvincibilityCooldown()
    {
        // Espera por el tiempo de invencibilidad (ej. 1 segundo)
        yield return new WaitForSeconds(invincibilityTime);

        // Después de 1 segundo, el jugador puede volver a recibir daño
        canTakeDamage = true;
    }
}