using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseStats)), CanEditMultipleObjects]
public class StatBlockEditor : Editor
{
    public string[] options = new string[] {"Character", "Item", "Weapon"};
    public int index = 0;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        BaseStats baseStats = (BaseStats) target;

        if (baseStats.stats == null || baseStats.stats.Count == 0) {
            baseStats.stats = new List<ObjectStatValue>();

            foreach (ObjectStatNames objectStatNames in System.Enum.GetValues(typeof(ObjectStatNames)))
            {
                baseStats.stats.Add(new ObjectStatValue (objectStatNames, 0));
            }
        }

        index = EditorGUILayout.Popup(index, options);

        if(GUILayout.Button("Add Stat Block")) {
            switch (index)
            {
                case 0:
                
                    break;
                case 1:
                
                    break;
                case 2:
                
                    break;
            }
        }
    }
}
