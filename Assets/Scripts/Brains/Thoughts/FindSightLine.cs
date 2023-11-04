using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindSightLine : BaseThought
{
    protected override void Start() {
        base.Start();
    }

    public override float Evaluate()
    {
        float value = 0f;

        if (brain.threatHandler.targetLastSeen == null) {
            return 0f;
        }

        if (brain.threatHandler.target == null && brain.attackTimer <= 0f) {
            value += 60f;
        }

        return value;
    }

    public override void Execute()
    {
        brain.movement += brain.GetDirectionFromPath();
    }
}
