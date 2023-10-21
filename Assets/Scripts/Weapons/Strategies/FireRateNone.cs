using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateNone : BaseStrategy, IFireRate
{
    public bool CanFire()
    {
        return true;
    }

    public void FiredShot()
    {
        
    }
}
