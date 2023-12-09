using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseBrain))]

public class CharacterHandler : ObjectHandler
{
    [SerializeField] private BaseCharacterStats baseCharacterStats;

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

        OnTakeDamage += OnDamageFlash;
    }

    protected void Update() {
        if (currentStamina < statsCharacter[CharacterStatNames.Stamina].GetValue() && objectStatusHandler.CanRegainStamina()) {
            ChangeStamina(staminaRecPerSec * Time.deltaTime);
        }
    }

    public virtual void ChangeStamina(float changeAmount) {
        currentStamina = Mathf.Clamp(currentStamina + changeAmount, 0f, statsCharacter[CharacterStatNames.Stamina].GetValue());
    }

    public float GetStatValue(CharacterStatNames statName) {
        return statsCharacter[statName].GetValue();
    }

    void OnDamageFlash(float damage, WeaponHandler weapon, CharacterHandler damageDealer) {
        StartCoroutine("DoFlash");
    }
    IEnumerator DoFlash() {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = Color.white;
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