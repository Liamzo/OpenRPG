using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Orc : NonPlayerBrain
{
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        // Look at target
        if (threatHandler.targetLastSeen != null && character.objectStatusHandler.HasMovementControls())
            SetLookingDirection(threatHandler.targetLastSeen.Value);
    }
}
