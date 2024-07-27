using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EquipmentHandlerUI : EquipmentHandler
{
    public GameObject bulletUIPrefab;
    List<GameObject> bulletUIs = new List<GameObject>();
    public RectTransform bulletUIHolder;

    public WeaponHandler currentRangedWeapon;
    IAmmo currentRangedAmmo;
    IReload currentRangedReload;

    float rangedWeaponPrevAmmo;
    Color usedAmmoColor = new Color(1,1,1,0.5f);
    public Slider reloadBar;
    public RectTransform reloadBarTransform;

    protected override void Awake() {
        base.Awake();

        onEquipmentChanged += UpdateEquipmentUI;

        bulletUIHolder = GameObject.Find("BulletHolder").GetComponent<RectTransform>();

        reloadBar = GameObject.Find("AmmoReload").GetComponent<Slider>();
        reloadBarTransform = GameObject.Find("AmmoReload").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentRangedWeapon == null || reloadBarTransform.gameObject.activeSelf == false) { return; }
        
        UpdatedReloadSlider();
        

        if (rangedWeaponPrevAmmo != currentRangedAmmo.GetCurrentAmmo()) {
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

            currentRangedWeapon = newItem.GetComponent<WeaponHandler>();
            currentRangedAmmo = currentRangedWeapon.GetAllModStrategies().OfType<IAmmo>().FirstOrDefault();
            currentRangedReload = currentRangedWeapon.GetAllModStrategies().OfType<IReload>().FirstOrDefault();

            if (currentRangedAmmo == null) {
                reloadBarTransform.gameObject.SetActive(false);
                return;
            } else {
                reloadBarTransform.gameObject.SetActive(true);
            }

            for (int i = 0; i < currentRangedWeapon.statsWeapon[WeaponStatNames.ClipSize].GetValue(); i++) {
                GameObject go = Instantiate(bulletUIPrefab, bulletUIHolder);

                bulletUIs.Add(go);
            }

            UpdatedReloadSlider();

            rangedWeaponPrevAmmo = currentRangedAmmo.GetCurrentAmmo();
        }
    }

    void UpdatedReloadSlider() {
        reloadBarTransform.sizeDelta = new Vector2(bulletUIHolder.sizeDelta.x, reloadBarTransform.sizeDelta.y);

        if (currentRangedReload.ReloadPercentage() == 1f) {
            reloadBar.value = 0f;
        } else {
            reloadBar.value = currentRangedReload.ReloadPercentage();
        }
    }

    void UpdateAmmoCount() {
        if (rangedWeaponPrevAmmo < currentRangedAmmo.GetCurrentAmmo()) {
            for (int i = Mathf.Max((int)rangedWeaponPrevAmmo - 1, 0); i < currentRangedAmmo.GetCurrentAmmo(); i++) {
                bulletUIs[i].GetComponent<Image>().color = Color.white;
            }
        } else {
            for (int i = (int)rangedWeaponPrevAmmo - 1; i >= currentRangedAmmo.GetCurrentAmmo(); i--) {
                bulletUIs[i].GetComponent<Image>().color = usedAmmoColor;
            }
        }

        rangedWeaponPrevAmmo = currentRangedAmmo.GetCurrentAmmo();
    }
}
