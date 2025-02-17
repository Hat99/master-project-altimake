using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//under construction!!!
public class Tooltip : MonoBehaviour
{
    public string tooltipMessage;

    private TextMeshProUGUI tooltipTMP;

    private bool showTip;

    private void Start()
    {
        tooltipTMP.text = tooltipMessage;
        tooltipTMP.transform.parent = transform;
    }
    private void Update()
    {
        if (showTip)
        {
            
        }
    }

    public void OnMouseEnter()
    {
        showTip = true;
        tooltipTMP.gameObject.SetActive(true);
    }

    public void OnMouseExit()
    {
        showTip = false;
        tooltipTMP.gameObject.SetActive(false);
    }
}
