using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;



/***************************************************/
/* toggles tooltip when mouse is over a gameobject */
/***************************************************/

public class Tooltip : MonoBehaviour
{
    #region fields

    public string tooltipMessage;

    #endregion fields



    #region methods

    private void Start()
    {
        //create event triggers on the gameobject to detect mouse over
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry mouseEnter = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerEnter,
        };
        mouseEnter.callback.AddListener(MouseEnter);
        trigger.triggers.Add(mouseEnter);

        EventTrigger.Entry mouseExit = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerExit,
        };
        mouseExit.callback.AddListener(MouseExit);
        trigger.triggers.Add(mouseExit);
    }

    public void MouseEnter(BaseEventData data)
    {
        TooltipBase.instance.showTip = true;
        TooltipBase.instance.gameObject.SetActive(true);
        TextMeshProUGUI tmp = TooltipBase.instance.tmp;
        tmp.text = tooltipMessage;
        Vector2 backgroundSize = new Vector2(tmp.preferredWidth + 10, tmp.preferredHeight + 10);
        TooltipBase.instance.rectTransform.sizeDelta = backgroundSize;
    }

    public void MouseExit(BaseEventData data)
    {
        TooltipBase.instance.showTip = false;
        TooltipBase.instance.gameObject.SetActive(false);
    }

    #endregion methods
}
