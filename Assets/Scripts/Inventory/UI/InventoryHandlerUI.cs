using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryHandlerUI : InventoryHandler
{
    public static InventoryHandlerUI instance;

    public GameObject inventoryUI;
    public GameObject inventorySlotsParent;
    List<InventorySlotUI> itemSlots = new List<InventorySlotUI>();
    EquipmentSlotUI[] equipSlots;

    protected override void Awake()
    {
        base.Awake();

        instance = this;
        GetComponent<EquipmentHandler>().onEquipmentChanged += UpdateEquipmentUI;
        equipSlots = inventoryUI.GetComponentsInChildren<EquipmentSlotUI>();
    }

    private void Update() {
        if (InputManager.GetInstance().GetInventoryPressed()) {
            OnInventory();
        }
    }

    public override bool Add (ItemHandler item) {
        bool added = base.Add(item);

        if (added == true) {
            UpdateInventoryUI();
        }

        return added;
    }

    public override void Remove(ItemHandler item) {
        base.Remove(item);

        UpdateInventoryUI();
    }

    void UpdateInventoryUI() {
        if (inventoryUI.activeSelf == false)
            return;

        foreach (InventorySlotUI slot in itemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        itemSlots.Clear();

        foreach(ItemHandler item in inventory) {
            GameObject slotGO = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.ItemSlotUI);
            slotGO.transform.SetParent(inventorySlotsParent.transform, false);

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            itemSlots.Add(slotUI);

            slotUI.AddItem(item);
            slotUI.OnClick += OnPointerClick;

            slotGO.SetActive(true);
        }
    }

    void UpdateEquipmentUI (ItemHandler newItem, ItemHandler oldItem, EquipmentSlot equipmentSlot) {
        if (newItem == null) {
            equipSlots[(int)equipmentSlot].ClearSlot();
        } else {
            equipSlots[(int)equipmentSlot].AddItem(newItem);
        }
    }

    public void OnInventory() {
        if (inventoryUI.activeSelf == true) {
            CloseInventory();
        } else {
            OpenInventory();
        }
    }

    public void OpenInventory() {
        if (inventoryUI.activeSelf == false) {
            inventoryUI.SetActive(true);
        }

        UpdateInventoryUI();
    }
    public void CloseInventory() {
        if (inventoryUI.activeSelf == true) {
            inventoryUI.SetActive(false);
        }

        foreach (InventorySlotUI slot in itemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        itemSlots.Clear();
    }



    public void OnPointerClick(InventorySlotUI slot, PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            UseItem(slot.item);
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            InventoryExtraOptionsMenuUI.GetInstance().Move(slot.transform.position);

            InventoryExtraOptionsMenuUI.GetInstance().ClearOptions();

            foreach (ItemAction action in slot.item.itemActions) {
                InventoryExtraOptionsMenuUI.GetInstance().AddOption(action);
            }

            // Add contextual actions, like transfer

            InventoryExtraOptionsMenuUI.GetInstance().gameObject.SetActive(true);
        }
    }

    public void UseItem (ItemHandler item) {
        // Use this across player inventory and chests
        // Make it contextual
        // E.g. Check if Container is open, if yes, transfer items between containers/inventories
        if (ContainerHandler.instance.openedContainer != null) {
            TransferItem(item);
            ContainerHandler.instance.UpdateContainerUI();
        } else {
            item.Use();
        }
    }

    void TransferItem(ItemHandler item) {
        InventoryHandlerUI player;
        if (item.owner.TryGetComponent<InventoryHandlerUI>(out player)) {
            // Transfer to open contianer
            if (item.PickUp(ContainerHandler.instance.openedContainer.GetComponent<ObjectHandler>()) == true)
                player.Remove(item);
        } else {
            // Transfer to player
            if (item.PickUp(InventoryHandlerUI.instance.GetComponent<ObjectHandler>()) == true)
                ContainerHandler.instance.openedContainer.Remove(item);
        }
    }
}
