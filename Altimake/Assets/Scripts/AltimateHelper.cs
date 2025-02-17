using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


/*************************************/
/*helper for handling altimate object*/
/*************************************/
public static class AltimateHelper
{
    //global altimate object
    public static Altimate altimate;

    public static List<Altimate.Part.ImageData> images = new List<Altimate.Part.ImageData>();

    public static float maxLayer = 0f;

    //base path where the altimate files are to be stored
    public static string basePath;


    //creates an empty altimate and saves it in basePath
    public static void CreateNewAltimate()
    {
        altimate = new Altimate();

        //base part for each altimate, contains non-optional images
        Altimate.Part basePart = new Altimate.Part();
        basePart.onByDefault = true;
        basePart.optional = false;
        altimate.parts.Add(basePart);

        SaveAltimate();
    }
    

    //loads altimate object from basePath and stores in global altimate object
    //if no altimate file is found in designated path, stores empty altimate object instead
    public static void LoadAltimate(string fileName)
    {
        string json = FileHelper.ReadFromPath(basePath + Path.DirectorySeparatorChar + fileName);
        altimate = JsonUtility.FromJson<Altimate>(json);

        OptionsHandler optionsHandler = OptionsHandler.instance;
        LayerHandler layerHandler = LayerHandler.instance;
        ImageHandler imageHandler = ImageHandler.instance;

        images.Clear();

        //go through each part of the loaded altimate
        foreach(Altimate.Part part in altimate.parts)
        {
            //toggle option or base part
            if(part.subParts.Count == 0)
            {
                //toggle option
                if (part.optional)
                {
                    optionsHandler.LoadToggleOption(part);
                    foreach(Altimate.Part.ImageData image in part.images)
                    {
                        if (part.onByDefault)
                        {
                            image.isActive = true;
                        }
                        else
                        {
                            image.isActive = false;
                        }
                        images.Add(image);
                    }
                }
                //base part
                else
                {
                    //add all related images to the render list as active
                    foreach (Altimate.Part.ImageData image in part.images)
                    {
                        image.isActive = true;
                        images.Add(image);
                    }
                }
            }
            //alternating option (drop down)
            else
            {
                //TODO
            }
        }

        images.Sort();
        images.Reverse();

        foreach(Altimate.Part.ImageData image in images)
        {
            layerHandler.LoadLayer(image);
        }


        //initialize image loading (loads each image one by one to minimize main thread blocking)
        TimedThread.instance.InitializeImageLoading();
    }

    //Saves the global altimate object as a json file in basePath
    public static void SaveAltimate()
    {
        //TODO: include images list? (seems to work without it, need to test)
        string json = JsonUtility.ToJson(altimate, true);
        FileHelper.SaveToPath(basePath + Path.DirectorySeparatorChar + altimate.name + ".json", json);
    }

    public static void RenameAltimate(string name)
    {
        //remove old file
        File.Delete(basePath + Path.DirectorySeparatorChar + altimate.name + ".json");

        //update name
        altimate.name = name;

        //write new file
        SaveAltimate();
    }
}
