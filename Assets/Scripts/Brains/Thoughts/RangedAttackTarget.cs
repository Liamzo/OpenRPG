using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTarget : BaseThought
{
    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        float value = 0f;

        if (brain.threatHandler.target == null) {
            return 0f;
        }

        if (brain.attackTimer <= 0f) {
            value += 100f;
        }

        // Need to add check for In Range. Probably in FindSightLine too

        return value;
    }

    public override void Execute()
    {
        //float dist = Vector2.Distance(threatHandler.targetLastSeen.transform.position, transform.position);

        // Do Attack
        //Vector3 dir = (brain.threatHandler.target.transform.position - transform.position).normalized;
        brain.GetComponent<LizardSpit>().AttackHold();

        brain.attackTimer = brain.attackCoolDown;
    }
}
