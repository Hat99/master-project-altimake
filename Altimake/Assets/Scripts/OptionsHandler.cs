using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AnotherFileBrowser.Windows;
using UnityEngine.EventSystems;

public class OptionsHandler : MonoBehaviour
{
    public GameObject optionHolder;
    public GameObject optionToggleTemplate;

    public GameObject optionDropdownTemplate;

    public static OptionsHandler instance;

    private void Start()
    {
        instance = this;
    }

    public void OnToggleValueChanged(Altimate.Part part, Toggle toggle)
    {
        SetImagesActive(part.images, toggle.isOn);
    }



    public void OnDropDownValueChanged(Altimate.Part part, TMP_Dropdown dropdown)
    {
        for (int i = 0; i < part.subParts.Count; i++)
        {
            //set options that aren't selected to false and the selected option to true
            SetImagesActive(part.subParts[i].images, dropdown.value == i);
        }
    }

    private void SetImagesActive(List<Altimate.Part.ImageData> images, bool val)
    {
        //set values
        foreach(Altimate.Part.ImageData image in images)
        {
            image.isActive = val;
        }

        //update rendering
        ImageHandler.instance.UpdateImage(images);
    }

    public void Clear()
    {
        foreach (Transform child in optionHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    //Loads toggle option for part
    public void LoadToggleOption(Altimate.Part part)
    {
        //instantiate object
        GameObject optionObject = Instantiate(optionToggleTemplate, optionHolder.transform);
        optionObject.GetComponentInChildren<TextMeshProUGUI>().text = part.displayName;

        Toggle toggle = optionObject.GetComponent<Toggle>();
        toggle.isOn = part.onByDefault;

        toggle.onValueChanged.AddListener(delegate
        {
            OnToggleValueChanged(part, toggle);
        });

        optionObject.SetActive(true);
    }

    public void LoadDropDownOption(Altimate.Part part)
    {
        //TODO
        //instantiate object
        GameObject optionObject = GameObject.Instantiate(optionDropdownTemplate, optionHolder.transform);
        optionObject.GetComponentInChildren<TextMeshProUGUI>().text = part.displayName;

        TMP_Dropdown dropdown = optionObject.GetComponentInChildren<TMP_Dropdown>();
        //dropdown.options.Clear();
        //foreach (Altimate.Part.ImageData image in part.images)
        //{
        //    TMP_Dropdown.OptionData item = new TMP_Dropdown.OptionData();
        //    item.text = image.dropDownName;

        //    dropdown.options.Add(item);
        //}

        //optional dropdowns need an "off" option
        if (part.optional)
        {
            TMP_Dropdown.OptionData item = new TMP_Dropdown.OptionData();
            item.text = "Off";

            dropdown.options.Add(item);
        }

        dropdown.onValueChanged.AddListener(delegate
        {
            OnDropDownValueChanged(part, dropdown);
        });

        optionObject.SetActive(true);
    }

    public void RemoveOption(Altimate.Part part)
    {
        foreach(Transform child in optionHolder.transform)
        {
            if(child.gameObject.GetComponentInChildren<TextMeshProUGUI>().text == part.displayName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public GameObject addOptionMenu;
    public TMP_InputField optionNameInput;
    public TextMeshProUGUI dropLayersHereText;
    public GameObject layerContainer;
    public GameObject layerTemplate;
    public Toggle onByDefaultToggle;
    public void OnAddOptionClicked()
    {
        optionNameInput.text = "";
        ClearLayerContainer();
        dropLayersHereText.gameObject.SetActive(true);
        onByDefaultToggle.isOn = true;
        addOptionMenu.SetActive(true);
    }

    public void OnAddOptionMenuCloseClicked()
    {
        addOptionMenu.SetActive(false);
    }

    public void ClearLayerContainer()
    {
        foreach(Transform child in layerContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void HandleLayerDrop(Altimate.Part.ImageData imageData, PointerEventData eventData)
    {
        Flag flag = eventData.pointerEnter?.GetComponentInChildren<Flag>();

        if(flag != null && flag.flag == "layer target")
        {
            dropLayersHereText.gameObject.SetActive(false);
            GameObject layer = Instantiate(layerTemplate, layerContainer.transform);
            layer.GetComponentInChildren<TextMeshProUGUI>().text = imageData.source;
            layer.SetActive(true);
        }
    }

    public void OnAddToggleClicked()
    {
        Altimate.Part part = new Altimate.Part();
        part.optional = true;
        part.displayName = optionNameInput.text;
        part.onByDefault = onByDefaultToggle.isOn;

        foreach(Transform child in layerContainer.transform)
        {
            string imageSource = child.GetComponentInChildren<TextMeshProUGUI>().text;

            foreach(Altimate.Part.ImageData image in AltimateHelper.images)
            {
                if(imageSource == image.source)
                {
                    //remove image from basePart
                    AltimateHelper.altimate.parts[0].images.Remove(image);
                    //add image to new part
                    part.images.Add(image);
                    break;
                }
            }

        }
        
        AltimateHelper.altimate.parts.Add(part);
        LoadToggleOption(part);

        addOptionMenu.SetActive(false);
    }
}
