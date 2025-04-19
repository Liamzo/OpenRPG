using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ResearchOption)), CanEditMultipleObjects]
public class ResearchOptionEditor : Editor
{
    public string[] discoverOptions = new string[] {"Item", "Mod"};
    public int discoverIndex = 0;
    public string[] researchOptions = new string[] {"Uninstall"};
    public int researchIndex = 0;
    public string[] unlockOptions = new string[] {"Mod"};
    public int unlockIndex = 0;


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        ResearchOption researchOption = (ResearchOption) target;


        GUILayout.Label("Discovery");
        discoverIndex = EditorGUILayout.Popup(discoverIndex, discoverOptions);

        if(GUILayout.Button("Add Discover Requirement")) {
            switch (discoverIndex)
            {
                case 0:
                    researchOption.discoverRequirements.Add(new DiscoverItemRequirement());
                    break;
                case 1:
                    researchOption.discoverRequirements.Add(new DiscoverModRequirement());
                    break;
            }
        }

        
        GUILayout.Label("Research");
        researchIndex = EditorGUILayout.Popup(researchIndex, researchOptions);

        if(GUILayout.Button("Add Research Requirement")) {
            switch (researchIndex)
            {
                case 0:
                    researchOption.researchRequirements.Add(new ResearchRequirementUninstall());
                    break;
            }
        }


        GUILayout.Label("Assembly");
        if(GUILayout.Button("Add Assemble Requirement")) {
            researchOption.assembleRequirements.Add(new AssembleRequirement());
        }


        GUILayout.Label("Unlock");
        unlockIndex = EditorGUILayout.Popup(unlockIndex, unlockOptions);

        if(GUILayout.Button("Add Unlock Reward")) {
            switch (unlockIndex)
            {
                case 0:
                    researchOption.unlockRewards.Add(new ResearchUnlockMod());
                    break;
            }
        }
    }
}
