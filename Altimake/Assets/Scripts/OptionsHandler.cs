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

        optionObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            OnEditOptionClicked(part);
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
        int count = 0;
        foreach(Transform child in optionHolder.transform)
        {
            Debug.Log(++count);
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
    public TextMeshProUGUI addOptionsHeader;
    public Button addButton;
    public Button deleteButton;
    public Button saveButton;

    public void PrepareOptionsMenuForAdd()
    {
        optionNameInput.text = "";
        ClearLayerContainer();
        dropLayersHereText.gameObject.SetActive(true);
        onByDefaultToggle.isOn = true;
        addOptionsHeader.text = "Add Toggle Option";
        addButton.gameObject.SetActive(true);
        deleteButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(false);
    }

    public void PrepareOptionsMenuForEdit(Altimate.Part part)
    {
        optionNameInput.text = part.displayName;
        ClearLayerContainer();

        foreach(Altimate.Part.ImageData image in part.images)
        {
            LoadLayerObject(image);
        }

        dropLayersHereText.gameObject.SetActive(part.images.Count == 0);
        onByDefaultToggle.isOn = part.onByDefault;

        addOptionsHeader.text = "Edit Toggle Option";
        addButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(true);

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(delegate
        {
            OnDeleteToggleClicked(part);
        });

        saveButton.gameObject.SetActive(true);
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(delegate
        {
            OnSaveToggleClicked(part);
        });
    }

    public void OnLayerDeleteClicked(GameObject layerObject, Altimate.Part.ImageData image)
    {
        Destroy(layerObject);
    }

    public void LoadLayerObject(Altimate.Part.ImageData image)
    {
        GameObject layer = Instantiate(layerTemplate, layerContainer.transform);
        layer.GetComponentInChildren<TextMeshProUGUI>().text = image.source;

        layer.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            OnLayerDeleteClicked(layer, image);
        });

        //TODO: delete layer button assignment

        layer.SetActive(true);
    }

    public void OnAddOptionClicked()
    {
        PrepareOptionsMenuForAdd();
        addOptionMenu.SetActive(true);
    }

    public void OnEditOptionClicked(Altimate.Part part)
    {
        PrepareOptionsMenuForEdit(part);
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
            LoadLayerObject(imageData);
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

    public void OnSaveToggleClicked(Altimate.Part part)
    {
        //remove old option
        RemoveOption(part);

        part.displayName = optionNameInput.text;
        part.onByDefault = onByDefaultToggle.isOn;

        foreach(Altimate.Part.ImageData image in part.images)
        {
            if (!AltimateHelper.altimate.parts[0].images.Contains(image))
            {
                AltimateHelper.altimate.parts[0].images.Add(image);
            }
        }
        part.images.Clear();

        foreach (Transform child in layerContainer.transform)
        {
            string imageSource = child.GetComponentInChildren<TextMeshProUGUI>().text;

            foreach (Altimate.Part.ImageData image in AltimateHelper.images)
            {
                if (imageSource == image.source)
                {
                    //remove image from basePart
                    AltimateHelper.altimate.parts[0].images.Remove(image);
                    //add image to new part
                    part.images.Add(image);
                    break;
                }
            }

        }

        LoadToggleOption(part);

        addOptionMenu.SetActive(false);
    }

    public void OnDeleteToggleClicked(Altimate.Part part)
    {
        foreach(Altimate.Part.ImageData image in part.images)
        {
            if (!AltimateHelper.altimate.parts[0].images.Contains(image))
            {
                AltimateHelper.altimate.parts[0].images.Add(image);
            }
        }

        AltimateHelper.altimate.parts.Remove(part);
        RemoveOption(part);

        addOptionMenu.SetActive(false);
    }
}
