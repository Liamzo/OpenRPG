using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ResearchOptionUI : MonoBehaviour, IPointerClickHandler
{
    public ResearchOption researchOption { get; private set; }

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI progressText;


    public event System.Action<ResearchOptionUI, PointerEventData> OnClick = delegate { };


    public void AddResearchOpption(ResearchOption researchOption) {
        this.researchOption = researchOption;

        nameText.text = researchOption.researchName;
        progressText.text = researchOption.GetProgress();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }

    public void Select() {
        
    }
    public void Deselect() {
        
    }

    public bool TryComplete() {
        return researchOption.TryComplete();
    }
}
