using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[RequireComponent(typeof(ObjectHandler))]

public class ItemHandler : MonoBehaviour//, ISaveable
{
    public ObjectHandler objectHandler {get; private set;}
    public ObjectHandler owner;
    public Collider2D itemHitbox;
    public BaseItemStats baseItemStats;
    public List<ItemAction> itemActions;
    public ItemAction defaultItemAction;
    public int value {
        get {
            return baseItemStats.baseValue;
        }
        private set {
            this.value = value;
        }
    }


    // Events
    public event System.Action OnUnequip = delegate { };
    public event System.Action OnEquip = delegate { };

    private void Awake() {
        objectHandler = GetComponent<ObjectHandler>();

        // Create List of Actions
        itemActions = new List<ItemAction>();

        foreach (BaseItemAction action in baseItemStats.itemActions) {
            if (action is EquipItemAction) {
                foreach (EquipmentSlot slot in baseItemStats.possibleSlots) {
                    Dictionary<ItemActionDataType, object> data = new Dictionary<ItemActionDataType, object>();
                    data.Add(ItemActionDataType.EquipSlot, slot);
                    itemActions.Add(new ItemAction(this, action, data));
                }
                continue;
            }
            itemActions.Add(new ItemAction(this, action));
        }

        defaultItemAction = new ItemAction(this, baseItemStats.defaultAction);
    }

    public bool PickUp(ObjectHandler owner) {
        if (owner.GetComponent<InventoryHandler>().Add(this)) {
            itemHitbox.enabled = false;
            itemHitbox.transform.gameObject.SetActive(false);
            gameObject.SetActive(false);
            this.owner = owner;
            transform.SetParent(owner.transform);
            LevelManager.instance.currentLevel.items.Remove(objectHandler);
            return true;
        }
        return false;
    }

    public void Drop() {
        owner.GetComponent<InventoryHandler>().Remove(this);
        transform.position = owner.transform.position;
        itemHitbox.enabled = true;
        itemHitbox.gameObject.SetActive(true);
        objectHandler.spriteRenderer.enabled = true;
        objectHandler.spriteRenderer.sortingOrder = 0;
        gameObject.SetActive(true);
        owner = null;
        transform.SetParent(null);
        LevelManager.instance.currentLevel.items.Add(objectHandler);
    }

    public void Use() {
        defaultItemAction.Action();
    }

    public void Equip() {
        OnEquip();
    }
    public void Unequip() {
        OnUnequip();
    }

    // public string SaveComponent()
    // {
    //     return $"item: {{ owner: {owner} }}";
    // }

    // public void LoadComponent(JSONNode data)
    // {
    //     owner = data["item"]["owner"];
    // }
}
