using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Guardar posición inicial en coordenadas locales del Canvas
        startPosition = rectTransform.anchoredPosition;
        // Permitir que los raycasts pasen a los slots
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Mover la pieza según el delta del cursor
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reactivar raycasts para que la pieza pueda bloquear interacciones
        canvasGroup.blocksRaycasts = true;

        // Si no se suelta en un slot válido, volver a la posición inicial
        if (eventData.pointerEnter == null || !eventData.pointerEnter.CompareTag("PuzzleSlot"))
        {
            rectTransform.anchoredPosition = startPosition;
        }
        else
        {
            // Centrar la pieza dentro del slot
            rectTransform.SetParent(eventData.pointerEnter.transform);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}