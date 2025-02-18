using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileMenuHandler : MonoBehaviour
{
    public GameObject fileMenu;
    public Collider2D fileMenuCollider;

    public void OnFileClicked()
    {
        fileMenu.SetActive(true);
    }

    public void OnCreateNewClicked()
    {
        AltimateHelper.SaveAltimate();
        StartMenuHandler.instance.OnNewFileClicked();
        fileMenu.SetActive(false);
    }

    public void OnLoadExistingClicked()
    {
        AltimateHelper.SaveAltimate();
        StartMenuHandler.instance.OnLoadFileClicked();
        fileMenu.SetActive(false);
    }

    public bool mouseOver;
    private void Update()
    {
        //close the menu if a mouseclick is registered while it's open
        if (!mouseOver && Input.GetMouseButtonDown(0))
        {
            fileMenu.SetActive(false);
        }
    }

    public void OnMouseEnter()
    {
        mouseOver = true;
    }
    public void OnMouseExit()
    {
        mouseOver=false;
    }
}
