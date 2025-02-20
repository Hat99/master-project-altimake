using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



/*****************************************/
/* handles display and editing of layers */
/*****************************************/

public class LayerHandler : MonoBehaviour
{
    #region fields

    //scroll view container for layer objects in ui
    public GameObject layerHolder;

    //template for ui layer representations
    public GameObject layerTemplate;

    //allows calling from other classes
    public static LayerHandler instance;
    private void Start()
    {
        instance = this;
    }

    #endregion fields



    #region handle inputs

    public void OnLayerValueEditEnd(Altimate.Part.ImageData image, TMP_InputField inputField)
    {
        image.layer = float.Parse(inputField.text);

        //update max layer attribute
        if (image.layer > AltimateHelper.maxLayer)
        {
            AltimateHelper.maxLayer = image.layer;
        }

        //update layer info in main image list
        AltimateHelper.images[AltimateHelper.images.IndexOf(image)].layer = image.layer;

        RefreshLayers();
        ImageHandler.instance.RefreshImageLayers();
    }

    //assigns each layer an int value based on the current order
    public void OnNormalizeLayersClicked()
    {
        //start at highest layer since transform children are ordered top to bottom
        //(and drawing programs tend to sort layers in front above layers in back)
        int layer = layerHolder.transform.childCount;

        //update max layer
        AltimateHelper.maxLayer = layer;

        //go through images from low to high and layers from high to low
        int index = 0;
        foreach (Transform child in layerHolder.transform)
        {
            child.gameObject.GetComponentInChildren<TMP_InputField>().text = layer + "";
            AltimateHelper.images[index].layer = layer;
            index++;
            layer--;
        }
    }

    public void OnRemoveLayerClicked(GameObject layerObject, Altimate.Part.ImageData image)
    {
        Destroy(layerObject);
        AltimateHelper.RemoveImage(image);
    }

    #endregion handle inputs



    #region load layers

    public void Clear()
    {
        foreach (Transform child in layerHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void RefreshLayers()
    {
        Clear();

        //sort images from high to low
        AltimateHelper.images.Sort();
        AltimateHelper.images.Reverse();

        foreach (Altimate.Part.ImageData imageData in AltimateHelper.images)
        {
            LoadLayer(imageData);
        }
    }

    public void LoadLayer(Altimate.Part.ImageData image)
    {
        GameObject layerObject = Instantiate(layerTemplate, layerHolder.transform);

        //assign values based on image data
        layerObject.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(image.source);
        TMP_InputField inputField = layerObject.GetComponentInChildren<TMP_InputField>();
        inputField.text = image.layer + "";

        //add listeners
        inputField.onEndEdit.AddListener(delegate
        {
            OnLayerValueEditEnd(image, inputField);
        });
        layerObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            OnRemoveLayerClicked(layerObject, image);
        });
        layerObject.GetComponent<DragDrop>().onDragEnd.AddListener((PointerEventData eventData) =>
        {
            OptionsHandler.instance.HandleLayerDrop(image, eventData);
        });

        layerObject.SetActive(true);
    }

    #endregion load layers
}
