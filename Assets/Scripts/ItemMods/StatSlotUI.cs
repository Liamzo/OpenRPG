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

    public WeaponStatNames statName;


    public void AddStat(WeaponStatNames statName, float value) {
        this.statName = statName;

        WeaponStatInfo statInfo = StatsManager.Instance.FindStatInfo(statName);

        icon.sprite = statInfo.icon;
        icon.enabled = true;

        nameText.text = statInfo.statName.ToString();

        valueText.text = value.ToString();
    }

    public void ClearSlot() {
        Destroy(gameObject);
    }

    public void ChangeValue(float newValue) {
        valueText.text = newValue.ToString();
    }

    public void ChangeValueColor(Color colour) {
        valueText.color = colour;
    }
}
