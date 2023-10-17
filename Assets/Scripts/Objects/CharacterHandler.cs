using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseBrain))]

public class CharacterHandler : ObjectHandler
{
    public BaseCharacterStats baseCharacterStats;

    public Dictionary<CharacterStatNames, Stat> statsCharacter = new Dictionary<CharacterStatNames, Stat>();
    
    public float currentStamina;
    public float staminaRecPerSec;

    protected override void Awake()
    {
        base.Awake();
        
        foreach (CharacterStatValue sv in baseCharacterStats.stats) {
            statsCharacter.Add(sv.statName, new Stat(sv.value));
        }

        currentStamina = statsCharacter[CharacterStatNames.Stamina].GetValue();
    }

    protected void Update() {
        if (currentStamina < statsCharacter[CharacterStatNames.Stamina].GetValue() && objectStatusHandler.CanRegainStamina()) {
            ChangeStamina(staminaRecPerSec * Time.deltaTime);
        }
    }

    public virtual void ChangeStamina(float changeAmount) {
        currentStamina = Mathf.Clamp(currentStamina + changeAmount, 0f, statsCharacter[CharacterStatNames.Stamina].GetValue());
    }
}

[System.Serializable]
public enum CharacterStatNames {
    Stamina,
    Sight,
    MovementSpeed,
    AttackSpeed

}

[System.Serializable]
public struct CharacterStatValue {
    public CharacterStatNames statName;
    public float value;
}