using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModManager : MonoBehaviour
{
    public static ModManager Instance { get; private set; }

    public List<WeaponMod> allWeaponMods;

    public GameObject modManagerUI;

    public GameObject statSlotPrefab;
    public GameObject statSlotsParent;
    List<StatSlotUI> statSlots = new List<StatSlotUI>();
    public TextMeshProUGUI selectedWeaponName;
    public Image selectedWeaponSprite;

    public GameObject modItemSlotPrefab;
    public GameObject modItemSlotsParent;
    List<ModItemSlotUI> modItemSlots = new List<ModItemSlotUI>();
    ModItemSlotUI selectedModItem;

    public GameObject currentModSlotPrefab;
    public GameObject currentModSlotsParent;
    List<CurrentModSlotUI> currentModSlots = new List<CurrentModSlotUI>();
    CurrentModSlotUI selectedCurrentMod;

    public GameObject modSlotPrefab;
    public GameObject modSlotsParent;
    List<ModSlotUI> modSlots = new List<ModSlotUI>();
    ModSlotUI selectedMod;
    WeaponHandler previousWeapon;
    WeaponMod previousCurrentMod;



    private void Awake() {
        Instance = this;

        allWeaponMods = Resources.LoadAll<WeaponMod>("ItemMods/").ToList();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            OnModManager();
        }
    }


    public List<WeaponMod> FindModsBySlot(WeaponModSlot weaponModSlot) 
    {
        List<WeaponMod> weaponMods = allWeaponMods.Where(x => x.modSlot == weaponModSlot).ToList();

        return weaponMods;
    }

    public WeaponMod FindModById(string modId) {
        foreach (WeaponMod mod in allWeaponMods)
        {
            if (mod.modId == modId) {
                return mod;
            }
        }

        Debug.LogWarning("No mod found with that ID: " + modId);
        return null;
    }



    void UpdateModManagerUI() 
    {
        if (modManagerUI.activeSelf == false)
            return;


        foreach (ModItemSlotUI slot in modItemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        modItemSlots.Clear();

        // First do equipped, then do inventory. Show all moddable items
        WeaponHandler weapon = Player.instance.equipmentHandler.GetEquipment(EquipmentSlot.RightHand)?.GetComponent<WeaponHandler>();
        if (weapon != null) 
            CreateModItemSlot(weapon);

        weapon = Player.instance.equipmentHandler.GetEquipment(EquipmentSlot.RightRangedWeapon)?.GetComponent<WeaponHandler>();
        if (weapon != null) 
            CreateModItemSlot(weapon);

        
        foreach(ItemHandler item in Player.instance.GetComponent<InventoryHandler>().inventory) {
            if (item.TryGetComponent<WeaponHandler>(out weapon)) {
                CreateModItemSlot(weapon);
            }
        }


        if (previousWeapon != null) {
            ModItemSlotUI modItemSlotUI = modItemSlots.Find(x => x.weapon == previousWeapon);
            modItemSlotUI?.Select();
            selectedModItem = modItemSlotUI;
            previousWeapon = null;
        }

        UpdateSelectedWeapon();
        UpdateCurrentModsUI();
        UpdateModUI();
    }

    void UpdateSelectedWeapon() 
    {
        if (modManagerUI.activeSelf == false)
            return;


        foreach (StatSlotUI slot in statSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        statSlots.Clear();
        selectedWeaponName.text = "";
        selectedWeaponSprite.enabled = false;

        if (selectedModItem == null) {
            return;
        }


        selectedWeaponName.text = selectedModItem.weapon.item.objectHandler.baseStats.objectName;
        selectedWeaponSprite.sprite = selectedModItem.weapon.item.objectHandler.baseStats.sprite;
        selectedWeaponSprite.enabled = true;

        foreach ((WeaponStatNames statName, Stat stat) in selectedModItem.weapon.statsWeapon) {
            GameObject slotGO = Instantiate(statSlotPrefab, statSlotsParent.transform);

            StatSlotUI slotUI = slotGO.GetComponent<StatSlotUI>();
            statSlots.Add(slotUI);

            slotUI.AddStat(statName, stat.GetValue());

            slotGO.SetActive(true);
        }
    }

    void UpdateCurrentModsUI() 
    {
        if (modManagerUI.activeSelf == false)
            return;


        foreach (CurrentModSlotUI slot in currentModSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        currentModSlots.Clear();

        if (selectedModItem == null) {
            selectedCurrentMod = null;
            return;
        }

        foreach (KeyValuePair<WeaponModSlot, WeaponMod> mod in selectedModItem.weapon.mods)
        {
            GameObject slotGO = Instantiate(currentModSlotPrefab, currentModSlotsParent.transform);

            CurrentModSlotUI slotUI = slotGO.GetComponent<CurrentModSlotUI>();
            currentModSlots.Add(slotUI);

            slotUI.AddMod(mod.Value, mod.Key);

            if (mod.Value != null)
                slotUI.OnClick += OnPointerClickCurrentMod;

            slotGO.SetActive(true);
        }


        if (previousCurrentMod != null) {
            CurrentModSlotUI currentModSlotUI = currentModSlots.Find(x => x.mod?.modId == previousCurrentMod.modId);
            currentModSlotUI?.Select();
            selectedCurrentMod = currentModSlotUI;
            previousCurrentMod = null;
        } else {
            selectedCurrentMod = null;
        }


        UpdateModUI();
    }

    void UpdateModUI() {
        if (modManagerUI.activeSelf == false)
            return;

        selectedMod?.Unselect();
        selectedMod = null;
        foreach (ModSlotUI slot in modSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        modSlots.Clear();

        if (selectedCurrentMod == null){
            selectedMod = null;
            return;
        }

        foreach (WeaponMod mod in FindModsBySlot(selectedCurrentMod.mod.modSlot))
        {
            GameObject slotGO = Instantiate(modSlotPrefab, modSlotsParent.transform);

            ModSlotUI slotUI = slotGO.GetComponent<ModSlotUI>();
            modSlots.Add(slotUI);

            slotUI.AddMod(mod);

            slotUI.OnClick += OnPointerClickMod;

            slotGO.SetActive(true);

            if (selectedCurrentMod.mod.modId == mod.modId) {
                selectedMod?.Unselect();
                slotUI.Select();
                selectedMod = slotUI;
            }

        }
    }

    void CreateModItemSlot(WeaponHandler weapon) {
        GameObject slotGO = Instantiate(modItemSlotPrefab, modItemSlotsParent.transform);

        ModItemSlotUI slotUI = slotGO.GetComponent<ModItemSlotUI>();
        modItemSlots.Add(slotUI);

        slotUI.AddWeapon(weapon);
        slotUI.OnClick += OnPointerClickModItem;

        slotGO.SetActive(true);
    }

    public void OnPointerClickModItem(ModItemSlotUI slot, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (selectedModItem != slot) {
                selectedModItem?.Unselect();
                slot.Select();
                selectedModItem = slot;
            } else {
                selectedModItem?.Unselect();
                selectedModItem = null;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            
        }
        
        UpdateSelectedWeapon();
        UpdateCurrentModsUI();
    }

    public void OnPointerClickCurrentMod(CurrentModSlotUI slot, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (selectedCurrentMod != slot) {
                selectedCurrentMod?.Unselect();
                slot.Select();
                selectedCurrentMod = slot;
            } else {
                selectedCurrentMod?.Unselect();
                selectedCurrentMod = null;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            
        }

        UpdateModUI();
    }

    public void OnPointerClickMod(ModSlotUI slot, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            if (selectedMod != slot) {
                selectedMod?.Unselect();
                slot.Select();
                selectedMod = slot;
            } else {
                selectedModItem.weapon.ChangeMod(slot.mod);
                previousWeapon = selectedModItem.weapon;
                previousCurrentMod = slot.mod;
                UpdateModManagerUI();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            
        }
    }


    public void OnModManager() {
        if (modManagerUI.activeSelf == true) {
            CloseModManager();
        } else {
            OpenModManager();
        }
    }

    public void OpenModManager() {
        if (modManagerUI.activeSelf == false) {
            modManagerUI.SetActive(true);
        }

        UpdateModManagerUI();
    }
    public void CloseModManager() {
        if (modManagerUI.activeSelf == true) {
            modManagerUI.SetActive(false);
        }


        foreach (ModItemSlotUI slot in modItemSlots) {
            slot.ClearSlot();
        }

        modItemSlots.Clear();

        selectedModItem = null;


        foreach (CurrentModSlotUI slot in currentModSlots) {
            slot.ClearSlot();
        }

        currentModSlots.Clear();

        selectedCurrentMod = null;


        foreach (ModSlotUI slot in modSlots) {
            slot.ClearSlot();
        }

        modSlots.Clear();

        selectedMod = null;

        previousWeapon = null;
        previousCurrentMod = null;
    }
}
