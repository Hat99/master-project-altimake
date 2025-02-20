using UnityEngine;



/************************************************************/
/* handles new file / load file functionality for main menu */
/************************************************************/

public class FileMenuHandler : MonoBehaviour
{
    #region fields

    public GameObject fileMenu;

    public bool mouseOver;

    #endregion fields



    #region methods

    private void Update()
    {
        //close the menu if a mouseclick is registered elsewhere while it's open
        if (!mouseOver && Input.GetMouseButtonDown(0))
        {
            fileMenu.SetActive(false);
        }
    }

    //called externally when mouse enters the menu
    public void OnMouseEnter()
    {
        mouseOver = true;
    }

    //called externally when mouse exits the menu
    public void OnMouseExit()
    {
        mouseOver = false;
    }

    public void OnFileClicked()
    {
        fileMenu.SetActive(true);
    }

    public void OnCreateNewClicked()
    {
        //save the file
        AltimateHelper.SaveAltimate();

        //proceed to start up
        StartMenuHandler.instance.OnNewFileClicked();
        fileMenu.SetActive(false);
    }

    public void OnLoadExistingClicked()
    {
        //save the file
        AltimateHelper.SaveAltimate();

        //proceed to start up
        StartMenuHandler.instance.OnLoadFileClicked();
        fileMenu.SetActive(false);
    }

    #endregion methods
}
