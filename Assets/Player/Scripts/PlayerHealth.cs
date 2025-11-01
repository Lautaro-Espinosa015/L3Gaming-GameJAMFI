using System.Collections; // Necesario para las Corutinas (el cooldown)
using UnityEngine;
using UnityEngine.UI; // Necesario para controlar las Im�genes

public class PlayerHealth : MonoBehaviour
{
    // --- Variables de Salud ---
    public int maxHealth = 5;
    private int currentHealth;

    // --- Variables de UI ---
    // Arrastraremos las 5 im�genes de vida aqu�
    public Image[] lifeIcons;
    // Arrastraremos el panel "Game Over" aqu�
    public GameObject gameOverPanel;

    // --- Variables de Cooldown (Invencibilidad) ---
    private bool canTakeDamage = true;
    public float invincibilityTime = 1.0f; // 1 segundo de invencibilidad despu�s de ser golpeado

    void Start()
    {
        // 1. Empezamos con la vida m�xima
        currentHealth = maxHealth;

        // 2. Nos aseguramos que la pantalla de Game Over est� apagada
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 3. Actualizamos la UI para mostrar 5 vidas
        UpdateHealthUI();
    }

    // Esta funci�n se llama autom�ticamente cuando otro Collider entra en contacto con este
    // Aseg�rate de que el Player (o el Enemigo) tenga un Rigidbody
    // Cerca de la l�nea 40
    void OnTriggerEnter(Collider other)
    {
        // Verificamos si chocamos con "Enemy" Y si podemos recibir da�o
        if (other.gameObject.CompareTag("Enemy") && canTakeDamage)
        {
            // �La l�nea de abajo es nueva! Es para depurar
            Debug.Log("�TRIGGER DETECTADO! El enemigo me toc�.");

            // Si chocamos con un enemigo, recibimos da�o
            TakeDamage(1);
        }
    }

    // Funci�n p�blica para recibir da�o
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
            // Comparamos el �ndice del icono (i) con la vida actual
            if (i < currentHealth)
            {
                // Si el �ndice es MENOR que la vida, mostramos el icono
                lifeIcons[i].enabled = true;
            }
            else
            {
                // Si el �ndice es MAYOR o IGUAL, lo ocultamos
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
        // Si tu script de movimiento se llama "PlayerMovement", descomenta la siguiente l�nea:
        // GetComponent<PlayerMovement>().enabled = false;
    }

    // Esta es una Corutina que espera un tiempo
    IEnumerator InvincibilityCooldown()
    {
        // Espera por el tiempo de invencibilidad (ej. 1 segundo)
        yield return new WaitForSeconds(invincibilityTime);

        // Despu�s de 1 segundo, el jugador puede volver a recibir da�o
        canTakeDamage = true;
    }
}