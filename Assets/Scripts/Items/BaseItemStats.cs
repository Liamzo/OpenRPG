using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[System.Serializable]
public class BaseItemStats : StatBlock
{
    public Sprite icon;
    public List<BaseItemAction> itemActions;
    public BaseItemAction defaultAction;

    public List<EquipmentSlot> possibleSlots;
    public SpriteParts spritePart;
    public SpriteLibraryAsset spriteLibrary;

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