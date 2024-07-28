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



    private void Awake() {
        Instance = this;

        modManagerUI = GameObject.Find("Canvas").transform.Find("ModdingUI").gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) {
            OnModManager();
        }
    }



    void UpdateModManagerUI() {
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
        
    }

    void CreateModItemSlot(WeaponHandler weapon) {
        GameObject slotGO = Instantiate(modItemSlotPrefab, modItemSlotsParent.transform);

        ModItemSlotUI slotUI = slotGO.GetComponent<ModItemSlotUI>();
        modItemSlots.Add(slotUI);

        slotUI.AddWeapon(weapon);
        slotUI.OnClick += OnPointerClick;

        slotGO.SetActive(true);
    }

    public void OnPointerClick(ModItemSlotUI slot, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            
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
