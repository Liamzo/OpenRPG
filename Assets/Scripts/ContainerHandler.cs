using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContainerHandler : MonoBehaviour
{
    public static ContainerHandler instance;

    public GameObject inventorySlotsParent;
    List<InventorySlotUI> itemSlots = new List<InventorySlotUI>();

    public InventoryHandler openedContainer = null;

    private void Awake() {
        instance = this;

        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OpenContainer(InventoryHandler container) {
        openedContainer = container;
        gameObject.SetActive(true);
        UpdateContainerUI();
    }

    public void CloseContainer() {
        openedContainer = null;
        gameObject.SetActive(false);

        foreach (InventorySlotUI slot in itemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        itemSlots.Clear();
    }

    public void UpdateContainerUI() {
        foreach (InventorySlotUI slot in itemSlots) {
            slot.ClearSlot();
            slot.gameObject.SetActive(false);
        }

        itemSlots.Clear();

        foreach(ItemHandler item in openedContainer.inventory) {
            GameObject slotGO = ObjectPoolManager.instance.GetPooledObject(PoolIdentifiers.ItemSlotUI);
            slotGO.transform.SetParent(inventorySlotsParent.transform, false);

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
            itemSlots.Add(slotUI);

            slotUI.AddItem(item);
            slotUI.OnClick += OnPointerClick;

            slotGO.SetActive(true);
        }
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
