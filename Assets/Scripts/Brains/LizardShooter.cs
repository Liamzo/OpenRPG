using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardShooter : NonPlayerBrain
{
    // Update is called once per frame
    protected override void Update()
    {
        // if (target == null) {
        //     Idle();
        //     return;
        // } else {
        //     // Sprite aim at target
        //     if (transform.position.x < target.transform.position.x) {
        //         character.spriteRenderer.flipX = false;
        //     } else {
        //         character.spriteRenderer.flipX = true;
        //     }
        // }

        // if (distToTarget > minChaseDistance) {
        //     agent.isStopped = false;
        //     ChaseTarget();
        // } else {
        //     StayAtRange();
        // }

        base.Update();
        
        // Look at target
        // if (threatHandler.targetLastSeen != null)
        //     GetComponent<EquipmentHandler>().SpotLook(threatHandler.targetLastSeen);
    }


    // Thoughts
    // public float minChaseDistance;
    // void ChaseTarget() {
    //     agent.SetPath(pathToTarget);
    // }

    // public float minRange;
    // public float maxRange;
    // public float maxShootingRange;
    // void StayAtRange() {
    //     // Direction from target to this
    //     Vector3 dir = (transform.position - threatHandler.targetLastSeen.transform.position).normalized;

    //     float dist = Vector2.Distance(threatHandler.targetLastSeen.transform.position, transform.position);
    //     if (dist >= minRange && dist <= maxRange) {
    //         // If sight, just wait/wander
    //         Vector3 targetDir = (threatHandler.targetLastSeen.transform.position - transform.position).normalized;
    //         RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, targetDir, maxShootingRange);
    //         if (hitPlayer.collider != null) {
    //             if (hitPlayer.collider.gameObject.Equals(threatHandler.targetLastSeen)) {
    //                 agent.isStopped = true;
    //                 if (attackTimer <= 0) {
    //                     AttackTarget();
    //                     attackTimer = attackCoolDown;
    //                 }
    //                 return;
    //             }
    //         }

    //         // Try to move into line of sight
    //         // agent.SetDestination(threatHandler.targetLastSeen.transform.position);
    //         // agent.isStopped = false;
    //         // return;
    //     }

    //     // Find ideal direction
    //     // Sweep in all directions in 20o
    //     // Find best direction to move, go that way

    //     Vector3 idealPos = dir * ((minRange + maxRange) / 2);
    //     idealPos += threatHandler.targetLastSeen.transform.position;

    //     Vector3 idealDir = (idealPos - transform.position).normalized;
        

    //     float score = 10f;

    //     //Debug.DrawLine(transform.position, transform.position + (idealDir * 5), Color.blue, 0.1f);
    //     RaycastHit2D hit = Physics2D.Raycast(transform.position, idealDir, 5f);
    //     if (hit.collider != null) {
    //         score += hit.distance * 2;
    //     } else {
    //         score += 10;
    //     }

    //     Vector3 bestDir = idealDir;
    //     float bestScore = score;

    //     for(int i = 20; i <= 180; i += 20) {
    //         // Used to check either side of the Character
    //         for (int j = -1; j <= 1; j += 2) {
    //             if (j == 1 && (i == 0 || i == 180)) {
    //                 continue;
    //             }

    //             Vector3 tryDir = Quaternion.AngleAxis(i * j, Vector3.forward) * idealDir;
    //             score = 10 * Vector3.Dot(idealDir, tryDir);

    //             //Debug.DrawLine(transform.position, transform.position + (tryDir * 5), Color.red, 0.1f);
    //             hit = Physics2D.Raycast(transform.position, tryDir, 5f);
    //             if (hit.collider != null) {
    //                 score += hit.distance * 2;
    //             } else {
    //                 score += 10;
    //             }

    //             if (score > bestScore) {
    //                 bestScore = score;
    //                 bestDir = tryDir;
    //             }
    //         }
    //     }

    //     //agent.SetDestination(bestDir + transform.position);
    //     agent.isStopped = true;
    //     gameObject.transform.position += bestDir * 2f * Time.deltaTime;
    //     Debug.DrawLine(transform.position, transform.position + (bestDir * 5), Color.yellow, 0.1f);
    // }

    // void AttackTarget() {
    //     float dist = Vector2.Distance(threatHandler.targetLastSeen.transform.position, transform.position);

    //     if (dist < maxShootingRange) {
    //         // Do Attack
    //         Vector3 dir = (threatHandler.targetLastSeen.transform.position - transform.position).normalized;
    //         GetComponent<LizardSpit>().Attack(dir);

    //         attackTimer = attackCoolDown;
    //     }
    // }

    // void Idle() {
    
    // }
}
