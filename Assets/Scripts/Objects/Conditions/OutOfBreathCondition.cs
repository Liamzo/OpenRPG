using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBreathCondition : BaseCondition
{
    CharacterHandler owner;

    float durationStop;

    float durationPause;
    float timerPause = 0f;

    float pauseStrength;

    float lastPlayedAudio = 0f;


    public OutOfBreathCondition (CharacterHandler owner, float durationStop = 0f, float durationPause = 3f, float pauseStrength = 75f) {
        this.owner = owner;
        this.durationStop = durationStop;
        this.durationPause = durationPause;
        this.pauseStrength = pauseStrength;
    }


    public override void Start()
    {
        if (durationStop > 0f) {
            owner.objectStatusHandler.BlockRegainStamina(durationStop);
        }

        if (durationPause > 0f) {
            owner.statsCharacter[CharacterStatNames.StaminaRegen].AddModifier(new Modifier(ModifierTypes.Multiplier, -pauseStrength));
        }

        AudioManager.instance.PlayClipRandom(AudioID.HeavyBreathing, owner.audioSource);
    }

    public override void Tick()
    {
        lastPlayedAudio += Time.deltaTime;

        if (durationPause > 0f) {
            timerPause += Time.deltaTime;

            if (timerPause >= durationPause) {
                End();
            }
        }
    }

    public override void End()
    {
        if (durationPause > 0f) {
            owner.statsCharacter[CharacterStatNames.StaminaRegen].RemoveModifier(new Modifier(ModifierTypes.Multiplier, -pauseStrength));
        }
        owner.conditionHandler.RemoveCondition(this);
    }

    public override void SameAdded(BaseCondition condition)
    {
        OutOfBreathCondition otherCondition = (OutOfBreathCondition) condition;
        // Reset and use the new one's values

        if (durationPause > 0f) {
            owner.statsCharacter[CharacterStatNames.StaminaRegen].RemoveModifier(new Modifier(ModifierTypes.Multiplier, -pauseStrength));
        }

        durationStop = otherCondition.durationStop;
        durationPause = otherCondition.durationPause;
        pauseStrength = otherCondition.pauseStrength;
        timerPause = 0f;

        if (durationStop > 0f) {
            owner.objectStatusHandler.BlockRegainStamina(durationStop);
        }

        if (durationPause > 0f) {
            owner.statsCharacter[CharacterStatNames.StaminaRegen].AddModifier(new Modifier(ModifierTypes.Multiplier, -pauseStrength));
        }

        if (lastPlayedAudio >= 5f) {
            AudioManager.instance.PlayClipRandom(AudioID.HeavyBreathing, owner.audioSource);
            lastPlayedAudio = 0f;
        }
    }
}
