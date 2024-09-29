using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI valueText;

    public Color baseColour;
    public Color positiveColour;
    public Color negativeColour;

    public void AddStat(WeaponStatNames statName, float value) {
        WeaponStatInfo statInfo = StatsManager.Instance.FindStatInfo(statName);

        icon.sprite = statInfo.icon;
        icon.enabled = true;

        nameText.text = statInfo.statName.ToString();

        valueText.text = value.ToString();
    }

    public void ClearSlot() {
        Destroy(gameObject);
    }
}
