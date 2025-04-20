using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionBlock : BaseReaction
{
    public override float Evaluate()
    {
        float value = 0f;

        value += 100f;

        return value;
    }

    public override void Execute()
    {
        Debug.Log("block");

        brain.thoughtLocked = null;
    }
}
