using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;



/**********************************************/
/* handles images and rendering functionality */
/**********************************************/

public class ImageHandler : MonoBehaviour
{
    #region fields

    public GameObject imageHolder;
    public GameObject imageTemplate;

    public Slider zoomSlider;

    //how fast mouse wheel zoom zooms in and out (factor)
    public float zoomSpeed;

    private bool inZoomSpace;

    //allows calling from other classes
    public static ImageHandler instance;
    private void Start()
    {
        instance = this;
    }

    #endregion fields



    #region rendering

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

        //signal the timed thread to start loading the next image if there is one
        TimedThread.instance.imageLoaded = true;
    }

    //toggle image visibility for given image data list
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
                    //sibling index equals order in object hierarchy and thus "layer" order
                    child.SetSiblingIndex(index);
                    index--;
                    break;
                }
            }
        }
    }

    public void RemoveImage(Altimate.Part.ImageData image)
    {
        foreach(Transform child in imageHolder.transform)
        {
            if (child.gameObject.name == image.source)
            {
                Destroy(child.gameObject);
            }
        }
    }

    #endregion rendering



    #region image zoom

    public void SetImageZoom(float zoom)
    {
        imageHolder.transform.localScale = new Vector3(zoom, zoom, zoom);
    }
    
    public void ResetImageZoomAndPos()
    {
        zoomSlider.value = 1;
        imageHolder.transform.localScale = Vector3.one;
        imageHolder.transform.localPosition = new Vector3(0,60,0);
    }
    
    private void Update()
    {
        if (inZoomSpace)
        {
            zoomSlider.value += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        }
    }

    //only allow for zoom if the mouse is in the designated area
    public void OnMouseEnterZoomSpace()
    {
        inZoomSpace = true;
    }
    public void OnMouseExitZoomSpace()
    {
        inZoomSpace = false;
    }

    #endregion image zoom
}
