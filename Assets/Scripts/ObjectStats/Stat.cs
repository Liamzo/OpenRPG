using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {
    [SerializeField] public float baseValue {get; private set;}

    private List<float> modifiers = new List<float>();

    public Stat(float baseValue) {
        this.baseValue = baseValue;
    }

    public float GetValue() {
        //
        float finalValue = baseValue;

        modifiers.ForEach(x => finalValue += x);

        return finalValue;
    }

    public void SetBaseValue(float value) {
        baseValue = value;
    }
    public void ChangeBaseValue(float value) {
        baseValue += value;
    }

    public void AddModifier (float modifier) {
        if (modifier != 0) {
            modifiers.Add(modifier);
        }
    }

    public void RemoveModifier (float modifier) {
        if (modifier != 0) {
            modifiers.Remove(modifier);
        }
    }
}