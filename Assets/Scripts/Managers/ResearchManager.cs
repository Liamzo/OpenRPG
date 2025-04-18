using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager Instance { get; private set; }

    public List<ResearchOption> researchOptions;

    public GameObject researchManagerUI;


    // Category Headers
    [Header("Category Headers")]
    public GameObject categoryHeaderPrefab;
    public GameObject categoryHeadersParent;
    List<ResearchCategoryUI> categoryHeaderUIs = new ();

    public ResearchCategory? selectedCategory = null;
    public ResearchCategoryUI selectedCategoryUI = null;



    // Research Options List
    [Header("Research Options List")]
    public GameObject researchOptionPrefab;
    public GameObject researchOptionsParent;
    List<ResearchOptionUI> researchOptionUIs = new ();

    [Header("Research Options Dropdown")]
    public GameObject researchTagPrefab;
    public GameObject researchTagsParent;
    List<ResearchOptionUI> researchTagUIs = new ();
    public TextMeshProUGUI filterTagText;


    // Select Research Option Details
    [Header("Selected Research Option Details")]
    public TextMeshProUGUI selectedResearchName;
    public TextMeshProUGUI selectedResearchDescription;
    public TextMeshProUGUI selectedResearchProgress;
    public TextMeshProUGUI selectedResearchButton;
    public ResearchOptionUI selectedResearchOption;

    void Awake()
    {
        Instance = this;

        List<ResearchOption> researchOptionsTemp = Resources.LoadAll<ResearchOption>("ResearchOptions/").ToList(); // Save this as a list of instances instead so we can track progress there
        researchOptionsTemp.ForEach(researchOption => researchOptions.Add(Instantiate(researchOption)));
        researchOptions.ForEach(researchOption => researchOption.Create());
        

        foreach (ResearchCategory researchCategory in System.Enum.GetValues(typeof(ResearchCategory))) {
            GameObject categoryGO = Instantiate(categoryHeaderPrefab, categoryHeadersParent.transform);

            ResearchCategoryUI categoryUI = categoryGO.GetComponent<ResearchCategoryUI>();
            categoryHeaderUIs.Add(categoryUI);

            categoryUI.AddResearchCategory(researchCategory);
            categoryUI.OnClick += OnPointerClickResearchCategory;

            categoryGO.SetActive(true);
        }

        selectedResearchName.text = "";
        selectedResearchDescription.text = "";
        selectedResearchProgress.text = "";
        selectedResearchButton.GetComponent<Button>().interactable = false;
    }

    void UpdateResearchManagerUI() {
        // Clear everything up
        if (researchManagerUI.activeSelf == false)
            return;

        foreach (ResearchOptionUI researchOptionUI in researchOptionUIs) {
            Destroy(researchOptionUI.gameObject); // Maybe add these to the Object Pool
        } 
        researchOptionUIs.Clear();

        ClearSelectedResearch();


        if (selectedCategory == null) return;


        // Generate new things
        foreach (ResearchOption researchOption in researchOptions.Where(x => x.researchCategories.Contains(selectedCategory.Value))) {
            GameObject researchGO = Instantiate(researchOptionPrefab, researchOptionsParent.transform);

            ResearchOptionUI researchUI = researchGO.GetComponent<ResearchOptionUI>();
            researchOptionUIs.Add(researchUI);

            researchUI.AddResearchOpption(researchOption);
            researchUI.OnClick += OnPointerClickResearchOption;

            researchGO.SetActive(true);
        }
    }


    public void OnPointerClickResearchCategory(ResearchCategoryUI researchCategoryUI, PointerEventData eventData) {
        ClearSelectedResearch();

        if (researchCategoryUI.researchCategory == selectedCategory) {
            // Selected the same Header
            selectedCategoryUI.UnselectHeader();
            selectedCategoryUI = null;
            selectedCategory = null;
            UpdateResearchManagerUI();
            return;
        }

        // Selected a different Header
        selectedCategoryUI?.UnselectHeader();

        selectedCategoryUI = researchCategoryUI;
        selectedCategory = researchCategoryUI.researchCategory;
        selectedCategoryUI.SelectHeader();

        UpdateResearchManagerUI();
    }


    public void OnPointerClickResearchOption(ResearchOptionUI researchOptionUI, PointerEventData eventData) {
        if (researchOptionUI == selectedResearchOption) {
            ClearSelectedResearch();
            return;
        }

        selectedResearchOption?.Deselect();

        selectedResearchOption = researchOptionUI;
        selectedResearchOption.Select();

        selectedResearchName.text = selectedResearchOption.researchOption.researchName;
        selectedResearchDescription.text = selectedResearchOption.researchOption.researchDescription;
        selectedResearchProgress.text = selectedResearchOption.researchOption.GetProgress();
        selectedResearchButton.GetComponent<Button>().interactable = true;
    }

    public void ClickedResearchButton() {
        selectedResearchOption.TryComplete();
    }

    public void OnPointerClickTagFilter() {
        Debug.Log("boop");
        //researchTagsParent.SetActive(true);
    }



    public void OpenResearchManager() {
        if (researchManagerUI.activeSelf == false) {
            researchManagerUI.SetActive(true);
        }

        UpdateResearchManagerUI();
    }
    public void CloseResearchManager() {
        if (researchManagerUI.activeSelf == true) {
            researchManagerUI.SetActive(false);
        }

        selectedCategoryUI?.UnselectHeader();
        selectedCategoryUI = null;
        selectedCategory = null;

        foreach (ResearchOptionUI researchOptionUI in researchOptionUIs) {
            Destroy(researchOptionUI.gameObject);
        } 
        researchOptionUIs.Clear();

        ClearSelectedResearch();
    }



    void ClearSelectedResearch() {
        selectedResearchOption?.Deselect();
        selectedResearchOption = null;

        selectedResearchName.text = "";
        selectedResearchDescription.text = "";
        selectedResearchProgress.text = "";
        selectedResearchButton.GetComponent<Button>().interactable = false;
    }
}


public enum ResearchCategory {
    Pistol,
    Sword,
    [Description("Consumable Items")] ConsumableItems
}