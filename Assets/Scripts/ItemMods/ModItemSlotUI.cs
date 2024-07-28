using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class ModItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    public WeaponHandler weapon {get; private set;}
    public Image icon;
    public TextMeshProUGUI nameText;
    public GameObject modItemList;
    public List<GameObject> modIcons;

    public event System.Action<ModItemSlotUI, PointerEventData> OnClick = delegate { };


    public void AddWeapon(WeaponHandler newWeapon) 
    {
        weapon = newWeapon;

        icon.sprite = weapon.item.baseItemStats.icon;
        icon.enabled = true;

        nameText.SetText(weapon.item.objectHandler.objectName);
        nameText.enabled = true;

        foreach (KeyValuePair<WeaponModSlot, WeaponMod> mod in weapon.mods)
        {
            GameObject modIcon = ObjectPoolManager.Instance.GetPooledObject(PoolIdentifiers.SmallIconUI);

            if (mod.Value != null) {
                modIcon.GetComponent<Image>().sprite = mod.Value.modIcon;

                modIcon.transform.SetParent(modItemList.transform, false);
                modIcon.SetActive(true);
                modIcons.Add(modIcon);
            } else {
                // TODO after adding way to map Enums to Icons, descriptions, etc
                // Put icon based on modslot to show empty slot
            }
        }
    }

    public void ClearSlot () {
        weapon = null;

        icon.sprite = null;
        icon.enabled = false;

        nameText.SetText("");
        nameText.enabled = false;

        foreach (GameObject modIcon in modIcons)
        {
            modIcon.transform.SetParent(ObjectPoolManager.Instance.transform, false);
            modIcon.SetActive(false);
        }
        modIcons.Clear();

        OnClick = delegate { };

        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }
}
