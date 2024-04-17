using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo Attack", menuName = "Combos/New Combo Attack")]
public class ComboAttackSO : ScriptableObject {
    public ComboInputType comboInputType;
    public string chargeAnimName;
    public string attackAnimName;
    public string attackHeavyAnimName;

    public float swingDuration;
    public float endHoldDuration;

    public List<ComboAttackSO> comboChains;
}
