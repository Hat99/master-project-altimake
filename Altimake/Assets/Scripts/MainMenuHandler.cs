using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TMPro;
using UnityEngine;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject fileViewContent;
    public GameObject fileViewItemTemplate;

    public TMP_InputField altimateTitle;


    public void OnSaveClicked()
    {
        AltimateHelper.SaveAltimate();
    }

    public void OnImportFilesClicked()
    {
        string[] paths = FileHelper.PickMultipleFilesPath("Please select the image(s) you want to import:");

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

                item.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate
                {
                    OnAddImportedFileClicked(name);
                });

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

    public void OnDeleteImportedFileClicked()
    {
        //TODO: delete file and all references?
    }
}
