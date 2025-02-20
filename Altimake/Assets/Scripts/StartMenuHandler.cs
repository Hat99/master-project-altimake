using System.IO;
using TMPro;
using UnityEngine;



/*************************************************/
/* handles start up functionality (start screen) */
/*************************************************/

public class StartMenuHandler : MonoBehaviour
{
    #region fields

    //object references to the menu screens
    public GameObject startMenu;
    public GameObject mainMenu;

    //main screen title reference
    public TMP_InputField altimateNameText;

    //allows calling from other classes
    public static StartMenuHandler instance;
    private void Start()
    {
        instance = this;
    }

    #endregion fields



    #region methods

    public void OnNewFileClicked()
    {
        //select save location
        string path = FileHelper.PickFolderPath("Please select a folder for your new project:");
        
        if(string.IsNullOrEmpty(path))
        {
            //no path chosen -> no action required
            //TODO: could provide feedback for user "Please select a path"
            return;
        }

        //TODO: could check if provided path already contains a "New Altimate.json"
        //-> warn user before overriding old file

        //clear anything currently present in the program (needed if something was already loaded)
        MainMenuHandler.instance.ClearAll();

        //set basePath to be used for file operations
        AltimateHelper.basePath = path;

        AltimateHelper.CreateNewAltimate();

        //finish start up sequence with shared start up steps
        InitiateStartUp();
    }

    public void OnLoadFileClicked()
    {
        //select file location
        string path = FileHelper.PickFilePath("Please select your project file:");

        if (string.IsNullOrEmpty(path))
        {
            //TODO: pop up "Please choose a project file"
            return;
        }

        //TODO: check if selected file is actually an altimate file 
        //(any .json file could be selected)

        //clear anything currently present in the program
        MainMenuHandler.instance.ClearAll();

        //isolate file name from basePath
        string fileName = Path.GetFileName(path);
        path = path.Replace(Path.DirectorySeparatorChar + fileName, "");

        //set basePath to be used for file operations
        AltimateHelper.basePath = path;

        AltimateHelper.LoadAltimate(fileName);

        //finish start up sequence with shared start up steps
        InitiateStartUp();
    }

    //shared startup procedure
    private void InitiateStartUp()
    {
        //switch to main menu page
        startMenu.SetActive(false);
        mainMenu.SetActive(true);

        //display the name of the currently loaded project
        altimateNameText.text = AltimateHelper.altimate.name;

        //"start up" the timed thread
        TimedThread.UseTime(true);
    }

    #endregion methods
}
