using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayerHandler : MonoBehaviour
{
    public GameObject layerHolder;
    public GameObject layerTemplate;

    public static LayerHandler instance;

    private void Start()
    {
        instance = this;
    }

    public void Clear()
    {
        foreach (Transform child in layerHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnLayerValueEditEnd(Altimate.Part.ImageData image, TMP_InputField inputField)
    {
        //TODO: test if this line is still needed...
        image.layer = float.Parse(inputField.text);

        if(image.layer > AltimateHelper.maxLayer)
        {
            AltimateHelper.maxLayer = image.layer;
        }
        AltimateHelper.images[AltimateHelper.images.IndexOf(image)].layer = float.Parse(inputField.text);

        RefreshLayers();
        ImageHandler.instance.RefreshImageLayers();
    }

    public void RefreshLayers()
    {
        Clear();

        AltimateHelper.images.Sort();
        AltimateHelper.images.Reverse();

        foreach (Altimate.Part.ImageData imageData in AltimateHelper.images)
        {
            LoadLayer(imageData);
        }
    }

    public void OnNormalizeLayersClicked()
    {
        int layer = layerHolder.transform.childCount;
        AltimateHelper.maxLayer = layer;
        int index = 0;
        foreach(Transform child in layerHolder.transform)
        {
            child.gameObject.GetComponentInChildren<TMP_InputField>().text = layer + "";
            AltimateHelper.images[index].layer = layer;
            index++;
            layer--;
        }
    }

    public void LoadLayer(Altimate.Part.ImageData image)
    {
        GameObject layerObject = Instantiate(layerTemplate, layerHolder.transform);

        layerObject.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(image.source);
        TMP_InputField inputField = layerObject.GetComponentInChildren<TMP_InputField>();
        inputField.text = image.layer + "";

        inputField.onEndEdit.AddListener(delegate
        {
            OnLayerValueEditEnd(image, inputField);
        });

        layerObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            OnRemoveLayerClicked(layerObject, image);
        });

        DragDrop dragDrop = layerObject.GetComponent<DragDrop>();
        dragDrop.onDragEnd.AddListener((PointerEventData eventData) =>
        {
            OptionsHandler.instance.HandleLayerDrop(image, eventData);
        });

        layerObject.SetActive(true);
    }

    public void OnRemoveLayerClicked(GameObject layerObject, Altimate.Part.ImageData image)
    {
        Destroy(layerObject);
        AltimateHelper.RemoveImage(image);
    }
}
