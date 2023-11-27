using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageType
{
    public void DealDamage(ObjectHandler target, float charge);
}
