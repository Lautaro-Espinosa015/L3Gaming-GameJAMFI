


using UnityEngine;
using TMPro;
using System.Collections;

public class InteraccionNotasManager : MonoBehaviour
{
    [System.Serializable]
    public class Nota
    {
        public GameObject triggerObject;   // Objeto con collider para detectar interacción
        public GameObject panelUI;         // Panel UI opcional
        public TextMeshPro texto3D;        // Texto 3D que aparece brevemente
        public string mensaje;             // Texto que se mostrará en el panel
    }

    [Header("Sonido Ambiental")]
    public AudioSource ambientAudioSource;    // Asigna el AudioSource en el inspector
    public AudioClip[] ambientClips;          // Clips de sonidos ambientales
    public float fadeDuration = 1.5f;         // Duración del fade-in/out
    public float soundDelay = 0.5f;           // Retraso antes de reproducir el sonido

    [Header("Notas")]
    public Nota[] notas; // Lista de notas
    private Nota notaActual = null;
    private bool isNotaVisible = false;

    void Update()
    {
        if (notaActual != null && Input.GetKeyDown(KeyCode.E))
        {
            // Invertimos el estado primero
            isNotaVisible = !isNotaVisible;

            if (isNotaVisible)
            {
                // Nota abierta → mostrar texto y activar panel
                StartCoroutine(MostrarTextoTemporal(notaActual));
                if (notaActual.panelUI != null)
                    notaActual.panelUI.SetActive(true);

               
                CancelInvoke("PlayAmbientSound"); 
                Invoke("PlayAmbientSound", soundDelay);

                // Fade-in del volumen
                StartCoroutine(FadeVolume(ambientAudioSource, 1f, fadeDuration));
            }
            else
            {
                // Nota cerrada → ocultar panel
                if (notaActual.panelUI != null)
                    notaActual.panelUI.SetActive(false);

                // Cancelar cualquier Invoke pendiente
                CancelInvoke("PlayAmbientSound");

                // Fade-out del volumen
                StartCoroutine(FadeVolume(ambientAudioSource, 0f, fadeDuration));
            }
        }
    }

    private IEnumerator MostrarTextoTemporal(Nota nota)
    {
        if (nota.texto3D != null)
        {
            nota.texto3D.text = nota.mensaje; // Asigna el texto
            nota.texto3D.gameObject.SetActive(true);
            yield return new WaitForSeconds(25f);
            nota.texto3D.gameObject.SetActive(false);
        }
    }

    private void PlayAmbientSound()
    {
        if (ambientAudioSource != null && ambientClips.Length > 0)
        {
            AudioClip clip = ambientClips[Random.Range(0, ambientClips.Length)];
            ambientAudioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator FadeVolume(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void ActivarNota(int index)
    {
        notaActual = notas[index];
    }

    public void DesactivarNota(int index)
    {
        if (notaActual == notas[index])
        {
            if (notaActual.panelUI != null)
                notaActual.panelUI.SetActive(false);

            isNotaVisible = false;
            notaActual = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var nota in notas)
        {
            if (other.gameObject == nota.triggerObject)
            {
                notaActual = nota;
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (notaActual != null && other.gameObject == notaActual.triggerObject)
        {
            if (notaActual.panelUI != null)
                notaActual.panelUI.SetActive(false);

            isNotaVisible = false;
            notaActual = null;
        }
    }
}



