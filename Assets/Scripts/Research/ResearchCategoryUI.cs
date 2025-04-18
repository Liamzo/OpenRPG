using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ResearchCategoryUI : MonoBehaviour, IPointerClickHandler
{
    public ResearchCategory researchCategory;

    TextMeshProUGUI text;


    public event System.Action<ResearchCategoryUI, PointerEventData> OnClick = delegate { };


    public void AddResearchCategory(ResearchCategory category) {
        researchCategory = category;

        text = GetComponent<TextMeshProUGUI>();

        text.text = category.GetPrettyName();
    }


    public void SelectHeader() {
        text.color = GameMenuManager.Instance.highlightText;

        text.fontMaterial = GameMenuManager.Instance.highlightMaterial;
    }
    public void UnselectHeader() {
        text.color = GameMenuManager.Instance.lowlightText;

        text.fontMaterial = GameMenuManager.Instance.lowlightMaterial;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }
}
