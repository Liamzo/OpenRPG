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
        if (threatHandler.targetLastSeen != null)
            SetLookingDirection(threatHandler.targetLastSeen.Value + new Vector3(0,0.6f,0));
    }
}
