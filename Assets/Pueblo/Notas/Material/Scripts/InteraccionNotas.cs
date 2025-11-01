using UnityEngine;
using UnityEngine.UI; 

public class InteraccionNotas : MonoBehaviour
{
    public GameObject imagePanel;
    private bool isPlayerNear = false; 
    private bool isImageVisible = false;

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) { 
            isImageVisible = !isImageVisible;
            imagePanel.SetActive(isImageVisible);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }
}
