using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class QuestEntryUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI nameText;

    public Quest quest {get; private set;}

    [SerializeField] private Color normalColour;
    [SerializeField] private Color highlightedColour;


    public event System.Action<QuestEntryUI, PointerEventData> OnClick = delegate { };

    public void AddItem (Quest newQuest) {
        quest = newQuest;

        icon.enabled = true;

        nameText.SetText(newQuest.name);
        nameText.enabled = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (background.color == normalColour)
            background.color = highlightedColour;
    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (background.color == highlightedColour)
            background.color = normalColour;
    }
}
