using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(LevelData)), CanEditMultipleObjects]
public class LevelDataEditor : Editor
{
    public string[] options = new string[] {"Road", "Trees", "BanditCamp", "Shack", "Enemies"};
    public int index = 0;


    private SerializedProperty genEventsProp;

    private Type[] actionTypes;
    private string[] actionTypeNames;


    private void OnEnable() {
        genEventsProp = serializedObject.FindProperty("generationEvents");

        var baseType = typeof(BaseGenEvent);
        actionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
            .ToArray();

        actionTypeNames = actionTypes.Select(t => t.Name).ToArray();
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        // LevelData levelData = (LevelData) target;

        // index = EditorGUILayout.Popup(index, options);
        
        // if(GUILayout.Button("Add Generation Event")) {
        //     switch (index)
        //     {
        //         case 0:
        //             levelData.generationEvents.Add(new RoadGenEvent());
        //             break;
        //         case 1:
        //             levelData.generationEvents.Add(new TreesGenEvent());
        //             break;
        //         case 2:
        //             levelData.generationEvents.Add(new BanditCampGenEvent());
        //             break;
        //         case 3:
        //             levelData.generationEvents.Add(new ShackSpawnEvent());
        //             break;
        //         case 4:
        //             levelData.generationEvents.Add(new EnemiesGenEvent());
        //             break;
        //     }
        // }

        serializedObject.Update();

        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        for (int i = 0; i < genEventsProp.arraySize; i++) {
            var element = genEventsProp.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical("box");

            int currentIndex = GetTypeIndex(element.managedReferenceFullTypename);
            int newIndex = EditorGUILayout.Popup("Type", currentIndex, actionTypeNames);

            if (newIndex != currentIndex) {
                element.managedReferenceValue = Activator.CreateInstance(actionTypes[newIndex]);
            }

            if (element.managedReferenceValue != null) {
                EditorGUILayout.PropertyField(element, GUIContent.none, true);
            }

            if (GUILayout.Button("Remove")) {
                genEventsProp.DeleteArrayElementAtIndex(i);
                continue;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Action")) {
            genEventsProp.arraySize++;
            var newElement = genEventsProp.GetArrayElementAtIndex(genEventsProp.arraySize - 1);
            newElement.managedReferenceValue = Activator.CreateInstance(actionTypes[0]);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private int GetTypeIndex(string fullTypename) {
        for (int i = 0; i < actionTypes.Length; i++) {
            if (fullTypename.Contains(actionTypes[i].FullName))
                return i;
        }
        return 0;
    }

    void GenerateEvent() {

    }
}
