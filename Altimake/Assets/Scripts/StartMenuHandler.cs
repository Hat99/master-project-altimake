using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class StartMenuHandler : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject mainMenu;

    public TMP_InputField altimateNameText;

    public void OnNewFileClicked()
    {
        //select save location for autosaves
        string path = FileHelper.PickFolderPath("Please select a folder for your new project:");
        
        if(string.IsNullOrEmpty(path))
        {
            //TODO: pop up "Please choose a folder"
            return;
        }
        Debug.Log(path);

        AltimateHelper.basePath = path;
        AltimateHelper.CreateNewAltimate();

        InitiateStartUp();
    }

    public void OnLoadFileClicked()
    {
        string path = FileHelper.PickFilePath("Please select your project file:");

        if (string.IsNullOrEmpty(path))
        {
            //TODO: pop up "Please choose a folder"
            return;
        }

        //isolate file name from basePath
        string [] split = path.Split(Path.DirectorySeparatorChar);
        string fileName = split[split.Length - 1];
        path = path.Replace(Path.DirectorySeparatorChar + fileName, "");

        AltimateHelper.basePath = path;
        AltimateHelper.LoadAltimate(fileName);

        InitiateStartUp();
    }

    //common startup procedure
    private void InitiateStartUp()
    {
        //switch to main menu page
        startMenu.SetActive(false);
        mainMenu.SetActive(true);

        //display the name of the currently loaded project
        Debug.Log(AltimateHelper.altimate.name);
        altimateNameText.text = AltimateHelper.altimate.name;

        //"start up" the timed thread
        TimedThread.UseTime(true);
    }
}
