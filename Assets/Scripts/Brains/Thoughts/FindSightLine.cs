using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindSightLine : BaseThought
{
    [Header("Distance")]
    public float idealDistance;

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
            value += 80f;
        }

        return value;
    }

    public override void Execute()
    {
        Vector3 currentDir = (transform.position - brain.threatHandler.Target.transform.position).normalized;

        for(int i = 20; i <= 180; i += 20) { // Consider adjusting angle jumps
            // Used to check either side of the Character
            for (int j = -1; j <= 1; j += 2) {
                if (j == 1 && (i == 0 || i == 180)) {
                    continue;
                }

                Vector3 tryDir = Quaternion.AngleAxis(i * j, Vector3.forward) * currentDir;

                Vector3 tryPosition = tryDir * (idealDistance + 1.0f);
                tryPosition += brain.threatHandler.Target.transform.position;

                Debug.DrawLine(brain.threatHandler.Target.transform.position, tryPosition, Color.red, 0.1f);
                
                LineOfSightInfo lineOfSightInfo = brain.threatHandler.CheckLineOfSightFromPosition(brain.threatHandler.Target, tryPosition);

                if (lineOfSightInfo.TargetInLineOfSight) {
                    Vector3 movingDir = (tryPosition - transform.position).normalized;
                    brain.movement += movingDir * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.75f;
                    return;
                }
            }
        }


        // Couldn't find a good position, so move towards the target
        brain.movement += brain.GetDirectionFromPath() * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.75f;;
    }
}
