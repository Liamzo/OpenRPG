using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeHandler
{
    public Dictionary<AttributeNames, Stat> attributes = new Dictionary<AttributeNames, Stat>();

    public AttributeHandler(BaseAttributes baseAttributes)
    {
        foreach (AttributeValue sv in baseAttributes.stats) {
            attributes.Add(sv.attributeName, new Stat(sv.value));
        }
    }

    public float GetAttribute(AttributeNames attributeName) {
        return attributes[attributeName].GetValue();
    }
}

[System.Serializable]
public enum AttributeNames {
    Strength,
    Agility,
    Toughness,
    Willpower,
    Intelligence,
    Ego

}

[System.Serializable]
public struct AttributeValue {
    public AttributeNames attributeName;
    public float value;

    public AttributeValue(AttributeNames attributeName, float value) {
        this.attributeName = attributeName;
        this.value = value;
    }
}