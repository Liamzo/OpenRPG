using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITrigger {
    public float AttackHoldCost();
    public float AttackHold();
    public float AttackReleaseCost();
    public float AttackRelease();
    public void AttackCancel();
}