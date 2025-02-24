using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class CurrentModSlotUI : MonoBehaviour, IPointerClickHandler
{
    public WeaponMod mod;
    public WeaponModSlot slot;

    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI slotNameText;
    public GameObject effectsList;
    public List<GameObject> effectIcons;

    public Image background;
    public Color unselectedColor;
    public Color selectedColor;


    public event System.Action<CurrentModSlotUI, PointerEventData> OnClick = delegate { };


    public void AddMod(WeaponMod newMod, WeaponModSlot weaponModSlot) {
        mod = newMod;

        slotNameText.text = weaponModSlot.ToString();

        if (mod == null) {
            // Set icon TODO: Part of upgrade for adding details to Stats
            nameText.text = "";
            return;
        }

        icon.sprite = mod.modIcon;
        icon.enabled = true;

        nameText.text = mod.modName;

        slotNameText.text = mod.modSlot.ToString(); // TODO: Part of upgrade for adding details to Stats

        foreach (BaseStrategy strategy in mod.strategies)
        {
            GameObject strategyIcon = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.SmallIconUI);

            strategyIcon.GetComponent<Image>().sprite = strategy.strategyIcon;
            
            strategyIcon.transform.SetParent(effectsList.transform, false);
            strategyIcon.SetActive(true);
            effectIcons.Add(strategyIcon);
        }

        foreach (WeaponModBonus weaponModBonus in mod.weaponModBonuses)
        {
            GameObject bonusIcon = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.SmallIconUI);

            bonusIcon.GetComponent<Image>().sprite = weaponModBonus.icon;
            
            bonusIcon.transform.SetParent(effectsList.transform, false);
            bonusIcon.SetActive(true);
            effectIcons.Add(bonusIcon);
        }
    }

    public void ClearSlot() {
        foreach (GameObject modIcon in effectIcons)
        {
            modIcon.transform.SetParent(ObjectPoolManager.Instance.transform, false);
            modIcon.SetActive(false);
        }
        effectIcons.Clear();


        Destroy(gameObject);
    }


    public void Select() {
        background.color = selectedColor;
    }

    public void Unselect() {
        background.color = unselectedColor;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }
}
