using UnityEngine;

public class NotaTrigger : MonoBehaviour
{
    public InteraccionNotasManager manager;
    public int notaIndex; // índice en el array del Manager

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.ActivarNota(notaIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.DesactivarNota(notaIndex);
        }
    }
}

