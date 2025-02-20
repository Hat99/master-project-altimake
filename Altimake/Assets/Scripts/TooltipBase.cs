using TMPro;
using UnityEngine;



/********************************************************/
/* template tooltip, follows mouse position when active */
/********************************************************/

public class TooltipBase : MonoBehaviour
{
    #region fields

    public TextMeshProUGUI tmp;

    public bool showTip;

    public RectTransform rectTransform;
    public RectTransform parentRectTransform;


    public static TooltipBase instance;
    private void Start()
    {
        instance = this;
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    #endregion fields



    #region methods

    private void Update()
    {
        if (showTip)
        {
            //place tooltip on mouse position
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRectTransform, Input.mousePosition, null, out position);

            transform.localPosition = position;

            //prevent tooltip going off screen
            Vector2 anchoredPosition = rectTransform.anchoredPosition;

            if(anchoredPosition.x + rectTransform.rect.width > parentRectTransform.rect.width)
            {
                anchoredPosition.x = parentRectTransform.rect.width - rectTransform.rect.width;
            }
            rectTransform.anchoredPosition = anchoredPosition;
        }
    }

    #endregion methods
}
