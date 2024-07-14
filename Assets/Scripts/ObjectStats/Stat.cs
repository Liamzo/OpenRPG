using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {
    [SerializeField] public float BaseValue {get; private set;}
    [SerializeField] public AttributeValue? BaseAttribute {get; private set;}
    [SerializeField] public CharacterHandler Character {get; private set;}

    [SerializeField] private List<float> flatModifiers = new List<float>();

    [SerializeField] private List<float> percentModifiers = new List<float>();


    // Events
    public event System.Action OnChange = delegate { };

    public Stat(float baseValue, AttributeValue? baseAttribute = null, CharacterHandler character = null) {
        BaseValue = baseValue;
        BaseAttribute = baseAttribute;
        Character = character;
    }

    public void AddAttribute (AttributeValue attribute, CharacterHandler character) {
        BaseAttribute = attribute;
        Character = character;
    }

    public float GetValue() {
        float finalValue = BaseValue;

        if (BaseAttribute.HasValue) {
            finalValue += Character.Attributes.GetAttribute(BaseAttribute.Value.attributeName) * BaseAttribute.Value.value;
        }

        flatModifiers.ForEach(x => finalValue += x);

        float multiplier = 100f;

        percentModifiers.ForEach(x => multiplier += x);

        multiplier = Mathf.Clamp(multiplier, 0f, Mathf.Infinity);

        return finalValue * (multiplier / 100f);
    }

    public void SetBaseValue(float value) {
        BaseValue = value;
    }
    public void ChangeBaseValue(float value) {
        BaseValue += value;
    }

    public void AddModifier (Modifier modifier) {
        if (modifier.value != 0) {
            if (modifier.type == ModifierTypes.Flat) {
                flatModifiers.Add(modifier.value);
            } else if (modifier.type == ModifierTypes.Multiplier) {
                percentModifiers.Add(modifier.value);
            }

            OnChange();
        }
    }

    public void RemoveModifier (Modifier modifier) {
        if (modifier.value != 0) {
            if (modifier.type == ModifierTypes.Flat) {
                flatModifiers.Remove(modifier.value);
            } else if (modifier.type == ModifierTypes.Multiplier) {
                percentModifiers.Remove(modifier.value);
            }

            OnChange();
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




[System.Serializable]
public enum ObjectStatNames {
    Health,
    ArmourValue,
    Weight
}

[System.Serializable]
public struct ObjectStatValue {
    public ObjectStatNames statName;
    public float value;
}


[System.Serializable]
public enum CharacterStatNames {
    Stamina,
    Sight,
    MovementSpeed,
    SprintMultiplier,
    StaminaRegen

}

[System.Serializable]
public struct CharacterStatValue {
    public CharacterStatNames statName;
    public float value;
}