using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnotherFileBrowser.Windows;
using System.IO;

public static class FileHelper
{

    public static string PickFolderPath(string title)
    {
        var bp = new BrowserProperties();
        bp.filter = "txt files (*.txt)|*.txt|All Files (*.*)|*.*";
        bp.filterIndex = 0;
        bp.title = title;

        string result = "";

        new FileBrowser().OpenFolderBrowser(bp, path =>
        {
            result = path;
        });

        return result;
    }

    public static string PickFilePath(string title)
    {
        var bp = new BrowserProperties();
        bp.filter = "Altimate Project File (*.json)|*.json";
        bp.filterIndex = 0;
        bp.title = title;

        string result = "";

        new FileBrowser().OpenFileBrowser(bp, path =>
        {
            result = path;
        });

        return result;
    }

    public static string[] PickMultipleFilesPath(string title)
    {
        var bp = new BrowserProperties();
        //TODO: check which of these image types unity can handle
        bp.filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
        bp.filterIndex = 0;
        bp.title = title;

        string[] result = {};

        new FileBrowser().OpenMultiSelectFileBrowser(bp, path =>
        {
            result = path;
        });

        return result;
    }

    public static void SaveToPath(string path, string content)
    {
        //TODO: what if file exists?
        File.WriteAllText(path, content);
    }

    public static string ReadFromPath(string path)
    {
        //TODO: what if file doesn't exist?
        return File.ReadAllText(path);
    }
}
