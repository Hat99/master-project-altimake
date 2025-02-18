using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject fileViewContent;
    public GameObject fileViewItemTemplate;

    public TMP_InputField altimateTitle;

    public static MainMenuHandler instance;

    private void Start()
    {
        instance = this;
    }

    //resets the entire system to allow for loading a different file
    public void ClearAll()
    {
        ClearComponents();
        AltimateHelper.Clear();
        TimedThread.UseTime(false);
    }

    public void ClearComponents()
    {
        ImageHandler.instance.Clear();
        OptionsHandler.instance.Clear();
        LayerHandler.instance.Clear();
    }
    public void ReloadAltimate()
    {
        ClearComponents();

        AltimateHelper.LoadAltimate(AltimateHelper.altimate.name + ".json");
    }



    public void OnSaveClicked()
    {
        AltimateHelper.SaveAltimate();
    }

    public void OnImportFilesClicked()
    {
        string[] paths = FileHelper.PickMultipleFilesPath("Please select the image(s) you want to import:");

        if(paths == null)
        {
            return;
        }

        foreach (string path in paths)
        {
            string fileName = Path.GetFileName(path);

            File.Copy(path, AltimateHelper.basePath + Path.DirectorySeparatorChar + fileName, true);
            Debug.Log(path);
        }

        RefreshFileView();
    }

    public void RefreshFileView()
    {
        string[] files = Directory.GetFiles(AltimateHelper.basePath);

        ClearFileView();

        foreach (string file in files)
        {
            //exclude json files from file viewer
            if (!file.EndsWith(".json"))
            {
                string name = Path.GetFileName(file);
                GameObject item = Instantiate(fileViewItemTemplate, fileViewContent.transform);

                
                foreach(Button button in item.GetComponentsInChildren<Button>())
                {
                    if(button.gameObject.name == "add")
                    {
                        button.onClick.AddListener(delegate
                        {
                            OnAddImportedFileClicked(name);
                        });
                    }
                    else if(button.gameObject.name == "delete")
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

    public void OnDeleteImportedFileClicked(string fileName)
    {
        foreach(Altimate.Part.ImageData image in AltimateHelper.images)
        {
            if(image.source == fileName)
            {
                AltimateHelper.images.Remove(image);
                ImageHandler.instance.RemoveImage(image);
                foreach(Altimate.Part part in AltimateHelper.altimate.parts)
                {
                    part.images.Remove(image);
                    foreach(Altimate.Part subPart in part.subParts)
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
}
