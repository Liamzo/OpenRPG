using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModManager : MonoBehaviour
{
    public static ModManager Instance { get; private set; }

    public List<WeaponMod> allWeaponMods;

    public GameObject modManagerUI;

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

        UpdateCurrentModsUI();
        UpdateModUI();
        
    }

    void UpdateCurrentModsUI() 
    {
        if (modManagerUI.activeSelf == false)
            return;

        selectedCurrentMod?.Unselect();
        selectedCurrentMod = null;
        foreach (CurrentModSlotUI slot in currentModSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        currentModSlots.Clear();

        if (selectedModItem == null)
            return;

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

        if (selectedCurrentMod == null)
            return;

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

            UpdateCurrentModsUI();
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            
        }
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
    }
}
