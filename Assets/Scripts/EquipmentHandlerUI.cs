using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentHandlerUI : EquipmentHandler
{
    public GameObject bulletUIPrefab;
    List<GameObject> bulletUIs = new List<GameObject>();
    public RectTransform bulletUIHolder;
    public RangedWeapon currentRangedWeapon;
    float rangedWeaponPrevAmmo;
    Color usedAmmoColor = new Color(1,1,1,0.5f);
    public Slider reloadBar;
    public RectTransform reloadBarTransform;

    protected private void Awake() {
        onEquipmentChanged += UpdateEquipmentUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentRangedWeapon != null) {
            UpdatedReloadSlider();
        }

        if (rangedWeaponPrevAmmo != currentRangedWeapon.ammo.GetCurrentAmmo()) {
            UpdateAmmoCount();
        }
    }

    void UpdateEquipmentUI(ItemHandler newItem, ItemHandler oldItem, EquipmentSlot equipmentSlot) {
        if (equipmentSlot == EquipmentSlot.RightRangedWeapon) {
            foreach (GameObject bulletUI in bulletUIs) {
                Destroy(bulletUI);
            }

            bulletUIs.Clear();

            if (newItem == null) {
                reloadBar.value = 0f;
                return;
            }

            currentRangedWeapon = newItem.GetComponent<RangedWeapon>();

            for (int i = 0; i < currentRangedWeapon.statsWeapon[WeaponStatNames.ClipSize].GetValue(); i++) {
                GameObject go = Instantiate(bulletUIPrefab, bulletUIHolder);

                bulletUIs.Add(go);
            }

            UpdatedReloadSlider();

            rangedWeaponPrevAmmo = currentRangedWeapon.ammo.GetCurrentAmmo();
        }
    }

    void UpdatedReloadSlider() {
        reloadBarTransform.sizeDelta = new Vector2(bulletUIHolder.sizeDelta.x, reloadBarTransform.sizeDelta.y);

        if (currentRangedWeapon.reload.ReloadPercentage() == 1f) {
            reloadBar.value = 0f;
        } else {
            reloadBar.value = currentRangedWeapon.reload.ReloadPercentage();
        }
    }

    void UpdateAmmoCount() {
        if (rangedWeaponPrevAmmo < currentRangedWeapon.ammo.GetCurrentAmmo()) {
            for (int i = Mathf.Max((int)rangedWeaponPrevAmmo - 1, 0); i < currentRangedWeapon.ammo.GetCurrentAmmo(); i++) {
                bulletUIs[i].GetComponent<Image>().color = Color.white;
            }
        } else {
            for (int i = (int)rangedWeaponPrevAmmo - 1; i >= currentRangedWeapon.ammo.GetCurrentAmmo(); i--) {
                bulletUIs[i].GetComponent<Image>().color = usedAmmoColor;
            }
        }

        rangedWeaponPrevAmmo = currentRangedWeapon.ammo.GetCurrentAmmo();
    }
}
