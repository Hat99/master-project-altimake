using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



/***********************************************/
/* handles use, display and editing of options */
/***********************************************/

public class OptionsHandler : MonoBehaviour
{
    #region fields

    //scroll container for loaded options
    public GameObject optionHolder;

    //templates for options 
    public GameObject optionToggleTemplate;
    public GameObject optionDropdownTemplate;

    //add / edit options menu references
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

    //allows calling from other classes
    public static OptionsHandler instance;
    private void Start()
    {
        instance = this;
    }

    #endregion fields



    #region option values changed

    public void OnToggleValueChanged(Altimate.Part part, Toggle toggle)
    {
        //when a toggle option is changed, all relevant images must be updated
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

    //sets active status of a list of images and updates rendering accordingly
    private void SetImagesActive(List<Altimate.Part.ImageData> images, bool val)
    {
        //set value for all given images
        foreach(Altimate.Part.ImageData image in images)
        {
            image.isActive = val;
        }

        //update rendering of given images
        ImageHandler.instance.UpdateImage(images);
    }

    #endregion option values changed



    #region option loading

    //clear the option scroll view
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

        //set option starting values
        optionObject.GetComponentInChildren<TextMeshProUGUI>().text = part.displayName;
        Toggle toggle = optionObject.GetComponentInChildren<Toggle>();
        toggle.isOn = part.onByDefault;

        //assign listeners to new object
        toggle.onValueChanged.AddListener(delegate
        {
            OnToggleValueChanged(part, toggle);
        });
        optionObject.GetComponentInChildren<Button>()?.onClick.AddListener(delegate
        {
            OnEditOptionClicked(part);
        });

        //set object visible
        optionObject.SetActive(true);
    }

    //not used, under construction!
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

    //removes an option from the generated list
    //note: options are identified by their name which only works for unique names
    //-> TODO: find better way of identification
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

    #endregion option loading



    #region add / edit options

    #region open option menu

    //open options menu in "add" configuration
    //-> "empty" settings
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

        addOptionMenu.SetActive(true);
    }

    //open options menu in "edit" configuration
    //-> settings loaded based on the edited option
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

        addOptionMenu.SetActive(true);
    }

    //empties layer container of option menu
    public void ClearLayerContainer()
    {
        foreach (Transform child in layerContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnAddOptionClicked()
    {
        PrepareOptionsMenuForAdd();
        
    }

    public void OnEditOptionClicked(Altimate.Part part)
    {
        PrepareOptionsMenuForEdit(part);
        
    }

    public void OnAddOptionMenuCloseClicked()
    {
        addOptionMenu.SetActive(false);
    }

    #endregion open option menu



    #region option layer management

    //called when a layer is dragged and dropped anywhere
    public void HandleLayerDrop(Altimate.Part.ImageData imageData, PointerEventData eventData)
    {
        //flag to identify the layer container as such ("Flag" script is on layer container object)
        Flag flag = eventData.pointerEnter?.GetComponentInChildren<Flag>();

        if (flag != null && flag.flag == "layer target")
        {
            dropLayersHereText.gameObject.SetActive(false);
            LoadLayerObject(imageData);
        }
    }

    //load a layer object (based on the simplified template for the option menu)
    public void LoadLayerObject(Altimate.Part.ImageData image)
    {
        GameObject layer = Instantiate(layerTemplate, layerContainer.transform);
        layer.GetComponentInChildren<TextMeshProUGUI>().text = image.source;

        layer.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            OnLayerDeleteClicked(layer, image);
        });

        layer.SetActive(true);
    }

    public void OnLayerDeleteClicked(GameObject layerObject, Altimate.Part.ImageData image)
    {
        Destroy(layerObject);
    }

    #endregion option layer management



    #region saving toggle options from menu inputs
    public void OnAddToggleClicked()
    {
        //create empty option object
        Altimate.Part part = new Altimate.Part();

        //toggle options are always optional
        part.optional = true;

        //get values from menu inputs
        part.displayName = optionNameInput.text;
        part.onByDefault = onByDefaultToggle.isOn;

        //go through layer container and move all relevant images to the new option
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

        //add new option to altimate object
        AltimateHelper.altimate.parts.Add(part);

        //load option representation in ui
        LoadToggleOption(part);

        addOptionMenu.SetActive(false);
    }

    public void OnSaveToggleClicked(Altimate.Part part)
    {
        //remove old option from ui (to update later)
        RemoveOption(part);

        //get values from menu inputs
        part.displayName = optionNameInput.text;
        part.onByDefault = onByDefaultToggle.isOn;

        //"empty" images of this option by moving them to the basePart
        //-> when they are added again later they will be based on the up to date layer list
        //(additions *and* removals will be saved)
        foreach(Altimate.Part.ImageData image in part.images)
        {
            if (!AltimateHelper.altimate.parts[0].images.Contains(image))
            {
                AltimateHelper.altimate.parts[0].images.Add(image);
            }
        }
        part.images.Clear();

        //add all relevant images from the layer container to the option
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

        //load updated option in ui
        LoadToggleOption(part);

        addOptionMenu.SetActive(false);
    }

    public void OnDeleteToggleClicked(Altimate.Part part)
    {
        //move all images to base part
        //(if they're no longer part of an option, they become part of the default image)
        foreach(Altimate.Part.ImageData image in part.images)
        {
            if (!AltimateHelper.altimate.parts[0].images.Contains(image))
            {
                AltimateHelper.altimate.parts[0].images.Add(image);
            }
        }

        //remove option from altimate and ui
        AltimateHelper.altimate.parts.Remove(part);
        RemoveOption(part);

        addOptionMenu.SetActive(false);
    }

    #endregion saving toggle options from menu inputs

    #endregion add / edit options
}
