using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModManager : MonoBehaviour
{
    public ModManager Instance { get; private set; }

    public GameObject modManagerUI;

    public GameObject modItemSlotPrefab;
    public GameObject modItemSlotsParent;
    List<ModItemSlotUI> modItemSlots = new List<ModItemSlotUI>();
    ModItemSlotUI selectedModItem;

    public GameObject currentModSlotPrefab;
    public GameObject currentModSlotsParent;
    List<CurrentModSlotUI> currentModSlots = new List<CurrentModSlotUI>();
    CurrentModSlotUI selectedCurrentMod;



    private void Awake() {
        Instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            OnModManager();
        }
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
    }
}
