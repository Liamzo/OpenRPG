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
        if (brain.threatHandler.targetLastSeen == null ) {
            return 0f;
        }

        if (brain.threatHandler.target == null) {
            return 0f;
        }
        
        return 65f;
    }

    public override void Execute()
    {
        // If within min and max distance then hold position
        // Else, try and move towards to middle on min and max distance
        if (brain.distToTarget >= minRange && brain.distToTarget <= maxRange) {
            // We know we already have line of sight, so just wait
            return;
        }


        // Find ideal direction
        // Sweep in all directions in 20o
        // Find best direction to move, go that way
        Vector3 dir = (transform.position - brain.threatHandler.target.transform.position).normalized;
        Vector3 idealPos = dir * ((minRange + maxRange) / 2);
        idealPos += brain.threatHandler.target.transform.position;

        Vector3 idealDir = (idealPos - transform.position).normalized;
        

        float score = 10f;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, idealDir, 5f);
        if (hit.collider != null) {
            score += hit.distance * 2;
        } else {
            score += 10;
        }

        Vector3 bestDir = idealDir;
        float bestScore = score;

        for(int i = 20; i <= 180; i += 20) {
            // Used to check either side of the Character
            for (int j = -1; j <= 1; j += 2) {
                if (j == 1 && (i == 0 || i == 180)) {
                    continue;
                }

                Vector3 tryDir = Quaternion.AngleAxis(i * j, Vector3.forward) * idealDir;
                score = 10 * Vector3.Dot(idealDir, tryDir);

                hit = Physics2D.Raycast(transform.position, tryDir, 5f);
                if (hit.collider != null) {
                    score += hit.distance * 2;
                } else {
                    score += 10;
                }

                if (score > bestScore) {
                    bestScore = score;
                    bestDir = tryDir;
                }
            }
        }

        brain.movement += bestDir.normalized * brain.character.statsCharacter[CharacterStatNames.MovementSpeed].GetValue() * 0.75f;
    }
}
