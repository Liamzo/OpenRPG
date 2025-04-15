using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class ResearchManager : MonoBehaviour
{
    public static ResearchManager Instance { get; private set; }

    public List<ResearchOption> researchOptions;

    public GameObject researchManagerUI;


    // Category Headers
    [Header("Category Headers")]
    public GameObject categoryHeaderPrefab;
    public GameObject categoryHeadersParent;
    public Color lowlightText;
    public Color highlightText;
    public Material lowlightMaterial;
    public Material highlightMaterial;
    List<ResearchCategoryUI> categoryHeaderUIs = new ();

    public ResearchCategory? selectedCategory = null;
    public ResearchCategoryUI selectedCategoryUI = null;



    // Research Options List
    [Header("Research Options List")]
    public GameObject researchOptionPrefab;
    public GameObject researchOptionsParent;
    List<ResearchOptionUI> researchOptionUIs = new ();



    // Select Research Option Details


    void Awake()
    {
        Instance = this;

        researchOptions = Resources.LoadAll<ResearchOption>("ResearchOptions/").ToList(); // Save this as a list of instances instead so we can track progress there

        foreach (ResearchCategory researchCategory in System.Enum.GetValues(typeof(ResearchCategory))) {
            GameObject categoryGO = Instantiate(categoryHeaderPrefab, categoryHeadersParent.transform);

            ResearchCategoryUI categoryUI = categoryGO.GetComponent<ResearchCategoryUI>();
            categoryHeaderUIs.Add(categoryUI);

            categoryUI.AddResearchCategory(researchCategory);
            categoryUI.OnClick += OnPointerClickResearchCategory;

            categoryGO.SetActive(true);
        }
    }

    void UpdateResearchManagerUI() {
        if (researchManagerUI.activeSelf == false)
            return;

        foreach (ResearchOptionUI researchOptionUI in researchOptionUIs) {
            Destroy(researchOptionUI.gameObject); // Maybe add these to the Object Pool
        } 
        researchOptionUIs.Clear();

        
        foreach (ResearchOption researchOption in researchOptions.Where(x => x.researchCategory == selectedCategory)) {
            GameObject researchGO = Instantiate(researchOptionPrefab, researchOptionsParent.transform);

            ResearchOptionUI researchUI = researchGO.GetComponent<ResearchOptionUI>();
            researchOptionUIs.Add(researchUI);

            researchUI.AddResearchOpption(researchOption);
            researchUI.OnClick += OnPointerClickResearchOption;

            researchGO.SetActive(true);
        }
    }


    public void OnPointerClickResearchCategory(ResearchCategoryUI researchCategoryUI, PointerEventData eventData) {
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
        Debug.Log(researchOptionUI.researchOption.researchName);
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
    }
}


public enum ResearchCategory {
    Pistol,
    Sword,
    ConsumableItems
}