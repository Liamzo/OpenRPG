using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ModSlotUI : MonoBehaviour, IPointerClickHandler
{
    public WeaponMod mod;

    public Image icon;
    public TextMeshProUGUI nameText;
    public GameObject effectsList;
    public List<GameObject> effectIcons;

    public Image background;


    public event System.Action<ModSlotUI, PointerEventData> OnClick = delegate { };


    public void AddMod(WeaponMod newMod) {
        mod = newMod;

        icon.sprite = mod.modIcon;
        icon.enabled = true;

        nameText.text = mod.modName;

        foreach (BaseStrategy strategy in mod.startingStrategies)
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

    public void ChangeBackgroundColor(Color colour) {
        background.color = colour;
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(this, eventData);
    }
}
