using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    public Image background;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI weightText;

    public ItemHandler item {get; private set;}


    public event System.Action<InventorySlotUI, PointerEventData> OnClick = delegate { };

    public void AddItem (ItemHandler newItem) {
        item = newItem;

        icon.sprite = item.baseItemStats.icon;
        icon.enabled = true;

        nameText.SetText(newItem.objectHandler.objectName);
        nameText.enabled = true;

        valueText.SetText(item.value.ToString());
        valueText.enabled = true;

        weightText.SetText(item.objectHandler.statsObject[ObjectStatNames.Weight].GetValue().ToString());
        weightText.enabled = true;
    }

    public void ClearSlot () {
        item = null;

        icon.sprite = null;
        icon.enabled = false;

        nameText.SetText("");
        nameText.enabled = false;

        valueText.SetText("");
        valueText.enabled = false;

        weightText.SetText("");
        weightText.enabled = false;

        OnClick = delegate { };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }
}
