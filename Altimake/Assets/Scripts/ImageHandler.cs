using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEditor;
using Unity.VisualScripting;

public class ImageHandler : MonoBehaviour
{
    /****************************/
    /*helper for handling images*/
    /****************************/

    public GameObject imageHolder;
    public GameObject imageTemplate;

    public static ImageHandler instance;

    private void Start()
    {
        instance = this;
    }

    public void Clear()
    {
        foreach (Transform child in imageHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    //loads the image for a singular imageData
    public void LoadImage(Altimate.Part.ImageData imageData)
    {
        //load data into texture from disc
        byte[] bytes = File.ReadAllBytes(AltimateHelper.basePath + Path.DirectorySeparatorChar + imageData.source);
        Texture2D loadTexture = new Texture2D(1, 1); //mock size 1x1
        loadTexture.LoadImage(bytes);

        //instantiate image object and assign texture
        GameObject image = Instantiate(imageTemplate, imageHolder.transform);
        image.name = imageData.source;
        image.GetComponent<RawImage>().texture = loadTexture;
        image.GetComponent<AspectRatioFitter>().aspectRatio = (float)loadTexture.width / (float)loadTexture.height;
        image.SetActive(imageData.isActive);

        TimedThread.instance.imageLoaded = true;
    }

    public void UpdateImage(List<Altimate.Part.ImageData> images)
    {
        int i = images.Count;
        foreach (Transform child in imageHolder.transform)
        {
            foreach (Altimate.Part.ImageData image in images)
                if (child.gameObject.name == image.source)
                {
                    child.gameObject.SetActive(image.isActive);
                    i--;
                    if (i == 0)
                    {
                        return;
                    }
                }
        }
    }

    //refreshes the layer order of all images to reflect changes made
    public void RefreshImageLayers()
    {
        int index = AltimateHelper.images.Count - 1;
        foreach(Altimate.Part.ImageData imageData in AltimateHelper.images)
        {
            foreach(Transform child in imageHolder.transform)
            {
                if(child.gameObject.name == imageData.source)
                {
                    child.SetSiblingIndex(index);
                    index--;
                    break;
                }
            }
        }
    }

    public void SetImageZoom(float zoom)
    {
        imageHolder.transform.localScale = new Vector3(zoom, zoom, zoom);
    }

    public Slider zoomSlider;
    public void ResetImageZoomAndPos()
    {
        zoomSlider.value = 1;
        imageHolder.transform.localScale = Vector3.one;
        imageHolder.transform.localPosition = new Vector3(0,60,0);
    }

    public float zoomSpeed;
    private bool inZoomSpace;
    private void Update()
    {
        if (inZoomSpace)
        {
            zoomSlider.value += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        }
    }

    public void OnMouseEnterZoomSpace()
    {
        inZoomSpace = true;
    }
    public void OnMouseExitZoomSpace()
    {
        inZoomSpace = false;
    }
}
