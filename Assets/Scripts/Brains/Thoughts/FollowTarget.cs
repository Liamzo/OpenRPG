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

        if (brain.threatHandler.targetLastSeen == null || !brain.character.objectStatusHandler.HasMovementControls()) {
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
        if (Vector3.Distance(transform.position, brain.threatHandler.targetLastSeen.Value) < 0.5f) {
            // Close enough to last seen point, so just wait
            return;
        }    

        brain.movement += brain.GetDirectionFromPath() * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.9f; // 90% speed
    }
}
