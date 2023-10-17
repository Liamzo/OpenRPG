using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeHandler : MonoBehaviour
{
    public BaseAttributes baseAttributes;

    public Dictionary<AttributeNames, Stat> stats = new Dictionary<AttributeNames, Stat>();

    void Awake()
    {
        foreach (AttributeValue sv in baseAttributes.stats) {
            stats.Add(sv.statName, new Stat(sv.value));
        }
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
    public AttributeNames statName;
    public float value;
}