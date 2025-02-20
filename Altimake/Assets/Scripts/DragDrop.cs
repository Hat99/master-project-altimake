using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;



/*****************************************************************/
/* implements drag / drop interfaces for drag drop functionality */
/*****************************************************************/

public class DragDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    #region fields

    //reference to the canvas to move the ui element
    public Canvas canvas;

    //canvas group allows setting object interactible
    public CanvasGroup canvasGroup;

    private RectTransform rectTransform;

    public UnityEvent<PointerEventData> onDragEnd;
    

    //wether to drag the object itself or make a copy and drag that
    public bool dragDuplicate;
    //the copy (if it's made)
    private GameObject duplicateObject;

    #endregion fields



    #region methods

    void Awake()
    {
        //ensures UnityEvent is not null
        if (onDragEnd == null) { onDragEnd = new UnityEvent<PointerEventData>(); }
    }

    //called once when the drag begins
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragDuplicate)
        {
            //create duplicate object to drag
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

        //make object uninteractible (so it can't be dragged onto itself)
        canvasGroup.blocksRaycasts = false;
    }

    //called each frame during a drag operation
    public void OnDrag(PointerEventData eventData)
    {
        //move dragged object
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    //called once when the drag ends
    public void OnEndDrag(PointerEventData eventData)
    {
        //make object interactible again
        canvasGroup.blocksRaycasts = true;

        //invoke onDragEnd event
        onDragEnd.Invoke(eventData);

        //destroy duplicate
        if(duplicateObject != null)
        {
            Destroy(duplicateObject);
        }
    }

    #endregion methods
}
