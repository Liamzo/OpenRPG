using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {
    [SerializeField] public float baseValue {get; private set;}
    [SerializeField] public AttributeValue baseAttribute {get; private set;}

    [SerializeField] private List<float> flatModifiers = new List<float>();

    [SerializeField] private List<float> percentModifiers = new List<float>();

    public Stat(float baseValue) {
        this.baseValue = baseValue;
    }

    public float GetValue() {
        float finalValue = baseValue;

        flatModifiers.ForEach(x => finalValue += x);

        float multiplier = 100f;

        percentModifiers.ForEach(x => multiplier += x);

        multiplier = Mathf.Clamp(multiplier, 0f, Mathf.Infinity);

        return finalValue * (multiplier / 100f);
    }

    public void SetBaseValue(float value) {
        baseValue = value;
    }
    public void ChangeBaseValue(float value) {
        baseValue += value;
    }

    public void AddModifier (Modifier modifier) {
        if (modifier.value != 0) {
            if (modifier.type == ModifierTypes.Flat) {
                flatModifiers.Add(modifier.value);
            } else if (modifier.type == ModifierTypes.Multiplier) {
                percentModifiers.Add(modifier.value);
            }
        }
    }

    public void RemoveModifier (Modifier modifier) {
        if (modifier.value != 0) {
            if (modifier.type == ModifierTypes.Flat) {
                flatModifiers.Remove(modifier.value);
            } else if (modifier.type == ModifierTypes.Multiplier) {
                percentModifiers.Remove(modifier.value);
            }
        }
    }
}


[System.Serializable]
public enum ModifierTypes {
    Flat,
    Multiplier
}
[System.Serializable]
public struct Modifier {
    public ModifierTypes type;
    public float value;

    public Modifier(ModifierTypes type, float value) {
        this.type = type;
        this.value = value;
    }
}