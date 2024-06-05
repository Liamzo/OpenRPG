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


    public OutOfBreathCondition (CharacterHandler owner, float durationStop = 0f, float durationPause = 3f, float pauseStrength = 0.5f) {
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
            owner.statsCharacter[CharacterStatNames.StaminaRegen].AddModifier(-pauseStrength);
        }
    }

    public override void Tick()
    {
        if (durationPause > 0f) {
            timerPause += Time.deltaTime;

            if (timerPause >= durationPause) {
                End();
            }
        }
    }

    public override void End()
    {
        owner.statsCharacter[CharacterStatNames.StaminaRegen].RemoveModifier(-pauseStrength);
        owner.conditionHandler.RemoveCondition(this);
    }
}
