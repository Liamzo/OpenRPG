using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ResearchOption)), CanEditMultipleObjects]
public class ResearchOptionEditor : Editor
{
    public string[] options = new string[] {"Uninstall"};
    public int index = 0;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        ResearchOption researchOption = (ResearchOption) target;

        //EditorGUILayout.LabelField("i", comboSO.comboName.ToString());

        //string s = EditorGUILayout.TextField(label:"Object Name: ", "defulat");
        
        index = EditorGUILayout.Popup(index, options);

        if(GUILayout.Button("Add Research Requirement")) {
            switch (index)
            {
                case 0:
                    researchOption.researchRequirements.Add(new ResearchRequirementUninstall());
                    break;
            }
        }

        if(GUILayout.Button("Add Assemble Requirement")) {
            researchOption.assembleRequirements.Add(new AssembleRequirement());
        }
    }
}
