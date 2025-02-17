using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField]
    private Canvas canvas;
    private RectTransform rectTransform;

    public UnityEvent<PointerEventData> onDragEnd;
    public CanvasGroup canvasGroup;

    public bool dragDuplicate;
    private GameObject duplicateObject;

    void Awake()
    {
        if (onDragEnd == null) { onDragEnd = new UnityEvent<PointerEventData>(); }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragDuplicate)
        {
            duplicateObject = Instantiate(gameObject, canvas.transform);
            duplicateObject.transform.localScale = Vector3.one;
            duplicateObject.transform.position = Input.mousePosition;
            rectTransform = duplicateObject.GetComponent<RectTransform>();
            canvasGroup = duplicateObject.GetComponent<CanvasGroup>();
        }
        else
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        onDragEnd.Invoke(eventData);
        if(duplicateObject != null)
        {
            Destroy(duplicateObject);
        }
    }
}
