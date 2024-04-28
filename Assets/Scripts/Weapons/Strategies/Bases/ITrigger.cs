using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrigger {
    public float AttackHoldCost();
    public bool CanAttackHold();
    public void AttackHold();
    public float AttackReleaseCost();
    public bool CanAttackRelease();
    public void AttackRelease();
    public void AttackCancel();
}