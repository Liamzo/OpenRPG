using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class BaseItemStats : ScriptableObject
{
    public Sprite icon;
    public List<BaseItemAction> itemActions;
    public BaseItemAction defaultAction;

    public List<EquipmentSlot> possibleSlots;

    public int baseValue;

    public List<ObjectStatBonus> objectStatBonuses;
    public List<CharacterStatBonus> characterStatBonuses;
    public List<AttributeBonus> attributeBonuses;
}

[System.Serializable]
public struct AttributeBonus {
    public AttributeNames attributeName;
    public int value;
}

[System.Serializable]
public struct ObjectStatBonus {
    public ObjectStatNames objectStatName;
    public int value;
}

[System.Serializable]
public struct CharacterStatBonus {
    public CharacterStatNames characterStatName;
    public int value;
}