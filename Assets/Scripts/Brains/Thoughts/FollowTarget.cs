using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : BaseThought
{
    public float minChaseDistance;

    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        float value = 0f;

        if (brain.threatHandler.targetLastSeen == null) {
            return 0f;
        }

        if (brain.threatHandler.target != null && brain.distToTarget <= minChaseDistance) {
            return 0f;
        } else if (brain.threatHandler.target != null && brain.distToTarget > minChaseDistance) {
            value += 50f;
        } else if (brain.threatHandler.target == null) {
            value += 70f;
        }

        return value;
    }

    public override void Execute()
    {
        brain.movement += brain.GetDirectionFromPath() * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.9f; // 90% speed
    }
}
