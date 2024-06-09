using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAtRange : BaseThought
{
    public float minRange;
    public float maxRange;

    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        if (brain.threatHandler.Target == null || brain.threatHandler.LineOfSightToTarget.TargetInLineOfSight == false) {
            return 0f;
        }


        if (brain.distToTarget >= minRange && brain.distToTarget <= maxRange) {
            // We know we already have line of sight, so just wait
            return 55f;
        } else {
            return 65f;
        }
    }

    public override void Execute()
    {
        brain.SetLookingDirection(brain.threatHandler.TargetLastSeen.Value);
        
        // If within min and max distance then hold position
        // Else, try and move towards to middle on min and max distance
        if (brain.distToTarget >= minRange && brain.distToTarget <= maxRange) {
            // We know we already have line of sight, so just wait
            return;
        }

        
        Vector3 dir = (transform.position - brain.threatHandler.Target.transform.position).normalized;
        Vector3 idealPos = dir * ((minRange + maxRange) / 2);
        idealPos += brain.threatHandler.Target.transform.position;

        Vector3 idealDir = (idealPos - transform.position).normalized;
        

        Vector3 bestDir = brain.FindPossibleDirectionFromIdeal(idealDir);

        brain.movement += bestDir * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.75f;
    }
}
