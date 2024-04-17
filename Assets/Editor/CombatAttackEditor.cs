using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ComboSO)), CanEditMultipleObjects]
public class ComboAttackEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        ComboSO comboSO = (ComboSO) target;

        //EditorGUILayout.LabelField("i", comboSO.comboName.ToString());

        //string s = EditorGUILayout.TextField(label:"Object Name: ", "defulat");
        
        if(GUILayout.Button("Add Combo Attacks")) {
            Debug.Log("It's alive!");

            ComboAttack attack = comboSO.comboChain;

            while (true) {
                ComboAttack tempAttack = attack;

                if (attack.comboChains.Count > 0) {
                    foreach (ComboAttack comboAttack in attack.comboChains) {
                        comboAttack.comboChains.Add(new ComboAttack());
                    }
                } else {
                    attack.comboChains.Add(new ComboAttack());
                    break;
                }

                attack = attack.comboChains[0];
                tempAttack.comboChains.Add(new ComboAttack());
            }

        }
    }
}
