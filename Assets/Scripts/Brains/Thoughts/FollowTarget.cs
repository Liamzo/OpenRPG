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

        if (brain.threatHandler.Target == null || !brain.character.objectStatusHandler.HasMovementControls()) {
            return 0f;
        }


        if (brain.threatHandler.LineOfSightToTarget.TargetInLineOfSight == false) {
            value += 70f;
        }
        else if (brain.distToTarget > minChaseDistance) {
            value += 50f;
        }

        return value;
    }

    public override void Execute()
    {
        brain.SetLookingDirection(brain.threatHandler.TargetLastSeen.Value);
        
        if (Vector3.Distance(transform.position, brain.threatHandler.TargetLastSeen.Value) < 0.5f) {
            // Close enough to last seen point, so just wait
            return;
        }

        Vector3 bestDir = brain.FindPossibleDirectionFromIdeal(brain.GetDirectionFromPath());

        brain.movement += bestDir * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.9f; // 90% speed
    }
}
