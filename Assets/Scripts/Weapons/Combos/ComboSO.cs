using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo", menuName = "Combos/New Combo")]
public class ComboSO : ScriptableObject
{
    public string comboName;
    public bool looping;

    public ComboAttack comboChain;
}

[System.Serializable]
public class ComboAttack {
    public ComboInputType comboInputType;
    public string chargeAnimName;
    public string attackAnimName;
    public string attackHeavyAnimName;

    public float swingDuration;
    public float endHoldDuration;

    [SerializeReference] public List<ComboAttack> comboChains;
}

public enum ComboInputType {
    Click,
    Rapid
}