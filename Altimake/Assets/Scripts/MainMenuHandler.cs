using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



/******************************************/
/* handles general tasks of the main menu */
/******************************************/

public class MainMenuHandler : MonoBehaviour
{
    #region fields

    //file viewer scroll view
    public GameObject fileViewContent;

    //template for representing file objects
    public GameObject fileViewItemTemplate;

    //input field and main Altimate title
    public TMP_InputField altimateTitle;

    //allows calling from other classes
    public static MainMenuHandler instance;
    private void Start()
    {
        instance = this;
    }

    #endregion fields



    #region menu clearing

    //resets the entire system to allow for loading a different file
    public void ClearAll()
    {
        ClearComponents();
        AltimateHelper.Clear();
        TimedThread.UseTime(false);
    }

    //resets the ui components to allow refreshing the same file
    public void ClearComponents()
    {
        ImageHandler.instance.Clear();
        OptionsHandler.instance.Clear();
        LayerHandler.instance.Clear();
    }

    //refresh the current altimate file
    public void ReloadAltimate()
    {
        ClearComponents();

        AltimateHelper.LoadAltimate(AltimateHelper.altimate.name + ".json");
    }

    #endregion menu clearing



    #region input handlers
    public void OnTitleChanged()
    {
        //if a new name was entered, rename file to match
        if (!string.IsNullOrEmpty(altimateTitle.text))
        {
            AltimateHelper.RenameAltimate(altimateTitle.text);
        }
        //else restore to previous name (file name can't be empty)
        else
        {
            altimateTitle.text = AltimateHelper.altimate.name;
        }
    }

    public void OnSaveClicked()
    {
        AltimateHelper.SaveAltimate();
    }

    public void OnImportFilesClicked()
    {
        //open file picker for multiple files
        string[] paths = FileHelper.PickMultipleFilesPath("Please select the image(s) you want to import:");

        if(paths == null)
        {
            return;
        }

        //copy each selected file into the basePath folder
        //TODO: stop self-imports (from basePath to basePath)
        foreach (string path in paths)
        {
            string fileName = Path.GetFileName(path);

            File.Copy(path, AltimateHelper.basePath + Path.DirectorySeparatorChar + fileName, true);
            Debug.Log(path);
        }

        RefreshFileView();
    }

    //adds an imported file to the altimate object
    public void OnAddImportedFileClicked(string fileName)
    {
        Altimate.Part.ImageData image = new Altimate.Part.ImageData();
        image.source = fileName;
        image.layer = ++AltimateHelper.maxLayer;
        image.isActive = true;

        AltimateHelper.altimate.parts[0].images.Add(image);
        AltimateHelper.images.Add(image);
        ImageHandler.instance.LoadImage(image);
        LayerHandler.instance.RefreshLayers();
    }

    //delets an imported file from the basePath folder
    public void OnDeleteImportedFileClicked(string fileName)
    {
        foreach (Altimate.Part.ImageData image in AltimateHelper.images)
        {
            if (image.source == fileName)
            {
                AltimateHelper.images.Remove(image);
                ImageHandler.instance.RemoveImage(image);
                foreach (Altimate.Part part in AltimateHelper.altimate.parts)
                {
                    part.images.Remove(image);
                    foreach (Altimate.Part subPart in part.subParts)
                    {
                        subPart.images.Remove(image);
                    }
                }
                break;
            }
        }
        File.Delete(AltimateHelper.basePath + Path.DirectorySeparatorChar + fileName);

        RefreshFileView();
        ReloadAltimate();
    }

    #endregion input handlers



    #region refresh file view

    public void RefreshFileView()
    {
        ClearFileView();

        string[] files = Directory.GetFiles(AltimateHelper.basePath);

        foreach (string file in files)
        {
            //exclude json files from file viewer
            if (!file.EndsWith(".json"))
            {
                string name = Path.GetFileName(file);
                GameObject item = Instantiate(fileViewItemTemplate, fileViewContent.transform);

                //assign listeners to both buttons in the template
                foreach(Button button in item.GetComponentsInChildren<Button>())
                {
                    //look for flags on the button to see which one it is
                    if(button.GetComponent<Flag>().flag == "add")
                    {
                        button.onClick.AddListener(delegate
                        {
                            OnAddImportedFileClicked(name);
                        });
                    }
                    else if(button.GetComponent<Flag>().flag == "delete")
                    {
                        button.onClick.AddListener(delegate
                        {
                            OnDeleteImportedFileClicked(name);
                        });
                    }
                }

                item.GetComponentInChildren<TextMeshProUGUI>().text = name;

                item.transform.localScale = Vector3.one;
                item.SetActive(true);
            }
        }
    }

    public void ClearFileView()
    {
        foreach (Transform child in fileViewContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    #endregion refresh file view
}
