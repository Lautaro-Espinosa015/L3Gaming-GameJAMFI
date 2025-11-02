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

    public Nota[] notas; // Lista de notas
    private Nota notaActual = null;
    private bool isNotaVisible = false;

    void Update()
    {
        if (notaActual != null && Input.GetKeyDown(KeyCode.E))
        {
            if (!isNotaVisible)
            {
                StartCoroutine(MostrarTextoTemporal(notaActual));
            }

            isNotaVisible = !isNotaVisible;
            if (notaActual.panelUI != null)
            {
                notaActual.panelUI.SetActive(isNotaVisible);
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
