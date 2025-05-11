using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageType
{
    public void DealDamage(ObjectHandler target, HitOutcome hitOutcome, float charge, BasicBullet projectile);
}

public enum HitOutcome {
    Hit,
    Block,
    Dodge,
    Parry
}