using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FireRate None", menuName = "Strategies/FireRate None")]
public class FireRateNone : BaseStrategy, IFireRate
{
    public bool CanFire()
    {
        return true;
    }

    public void DidAttack()
    {
        
    }
}
