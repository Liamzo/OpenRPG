using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[RequireComponent(typeof(ObjectHandler))]

public class ItemHandler : MonoBehaviour, ISaveable
{
    public ObjectHandler objectHandler { get; private set; }
    public int tier { get; private set; }
    public ObjectHandler owner;
    public Collider2D itemHitbox;
    public BaseItemStats baseItemStats { get; private set; }
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

    public bool equipped { get; private set; }

    // Events
    public event System.Action OnUnequip = delegate { };
    public event System.Action OnEquip = delegate { };

    private void Awake() {
        
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
        equipped = true;
        OnEquip();

        foreach (ObjectStatBonus objectStatBonus in baseItemStats.objectStatBonuses)
        {
            owner.statsObject[objectStatBonus.objectStatName].AddModifier(new Modifier(ModifierTypes.Flat, objectStatBonus.value));
        }

        if (owner is CharacterHandler) {
            CharacterHandler character = (CharacterHandler) owner;

            foreach (CharacterStatBonus characterStatBonus in baseItemStats.characterStatBonuses)
            {
                character.statsCharacter[characterStatBonus.characterStatName].AddModifier(new Modifier(ModifierTypes.Flat, characterStatBonus.value));
            }

            foreach (AttributeBonus attributeBonus in baseItemStats.attributeBonuses)
            {
                character.Attributes.attributes[attributeBonus.attributeName].AddModifier(new Modifier(ModifierTypes.Flat, attributeBonus.value));
            }
        }
    }
    public void Unequip() {
        equipped = false;
        OnUnequip();

        foreach (ObjectStatBonus objectStatBonus in baseItemStats.objectStatBonuses)
        {
            owner.statsObject[objectStatBonus.objectStatName].RemoveModifier(new Modifier(ModifierTypes.Flat, objectStatBonus.value));
        }

        if (owner is CharacterHandler) {
            CharacterHandler character = (CharacterHandler)owner;

            foreach (CharacterStatBonus characterStatBonus in baseItemStats.characterStatBonuses)
            {
                character.statsCharacter[characterStatBonus.characterStatName].RemoveModifier(new Modifier(ModifierTypes.Flat, characterStatBonus.value));
            }

            foreach (AttributeBonus attributeBonus in baseItemStats.attributeBonuses)
            {
                character.Attributes.attributes[attributeBonus.attributeName].RemoveModifier(new Modifier(ModifierTypes.Flat, attributeBonus.value));
            }
        }
    }

    public void CreateBase()
    {
        objectHandler = GetComponent<ObjectHandler>();

        // Create List of Actions
        itemActions = new List<ItemAction>();

        baseItemStats = objectHandler.baseStats.GetStatBlock<BaseItemStats>();

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

    public string SaveComponent()
    {
        return "";
    }

    public void LoadComponent(JSONNode data)
    {
        CreateBase();
    }
}
