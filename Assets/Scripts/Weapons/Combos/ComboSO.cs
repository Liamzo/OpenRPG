using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    public AttackDetails lightAttack;
    public AttackDetails heavyAttack;
    public float endHoldDuration;

    [SerializeReference] public List<ComboAttack> comboChains;
}

[System.Serializable]
public class AttackDetails {
    public string attackAnimName;
    public float swingDuration = 0f;
    public float knockBackModifier = 0f;
    public float selfKnockBackModifier = 0f;
    public float staggerModifier = 0f;
}

public enum ComboInputType {
    Click,
    Rapid
}