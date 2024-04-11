using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // This will make the UI element non-blocking for other raycasts, allowing click-through
        canvasGroup.blocksRaycasts = false;
        // Bring to the foreground
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;

        // Get the corners of the canvas
        Vector3[] canvasCorners = new Vector3[4];
        canvas.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);

        // Calculate the size of the canvas
        float canvasWidth = Vector3.Distance(canvasCorners[0], canvasCorners[3]);
        float canvasHeight = Vector3.Distance(canvasCorners[0], canvasCorners[1]);

        // Adjust the clamping based on the canvas size and the size of the UI element
        float clampedX = Mathf.Clamp(newPosition.x, -canvasWidth / 2 + rectTransform.sizeDelta.x / 2, canvasWidth / 2 - rectTransform.sizeDelta.x / 2);
        float clampedY = Mathf.Clamp(newPosition.y, -canvasHeight / 2 + rectTransform.sizeDelta.y / 2, canvasHeight / 2 - rectTransform.sizeDelta.y / 2);

        rectTransform.anchoredPosition = new Vector2(clampedX, clampedY);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset to true to block raycasts again, making the element interactive
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Bring to the foreground on click
        transform.SetAsLastSibling();
    }
}
