using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CircleTarget : BaseThought
{
    public float circleDistance;
    public int circleDirection = 1;

    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        if (brain.threatHandler.target == null) {
            return 0f;
        }

        float value = 0f;

        if (brain.distToTarget < circleDistance * 2) {
            value += 75f;
        }

        if (brain.attackTimer <= 0f) {
            // Can attack so slightly less this
            value -= 20f;
        }

        return value;
    }

    public override void Execute()
    {
        // Vectical Target
        Vector3 dir = (transform.position - brain.threatHandler.target.transform.position).normalized;
        Vector3 vertPos = dir * circleDistance;
        Vector3 horzPos = Quaternion.AngleAxis(20 * circleDirection, Vector3.forward) * vertPos;


        Vector3 vertTarget = brain.threatHandler.target.transform.position + vertPos;
        Vector3 horzTarget = brain.threatHandler.target.transform.position + horzPos;

        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(transform.position, horzTarget, NavMesh.AllAreas, path) && path.corners.Length > 1) {
            brain.movement += brain.GetDirectionFromPath(path) * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.7f; // 70% move speed
        } else {
            // Bug where Orc gets stuck on wall, thinks there's a path when none exists so keeps moving into wall. Path length returns 2, need to verify points
            circleDirection = circleDirection * -1;
            horzPos = Quaternion.AngleAxis(20 * circleDirection, Vector3.forward) * vertPos;

            horzTarget = brain.threatHandler.target.transform.position + horzPos;
            if (NavMesh.CalculatePath(transform.position, horzTarget, NavMesh.AllAreas, path)) {
                brain.movement += brain.GetDirectionFromPath(path) * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.7f; // 70% move speed
            }
        }
    }
}
