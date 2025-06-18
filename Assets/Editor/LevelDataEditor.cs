using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEditorInternal;

[CustomEditor(typeof(LevelData)), CanEditMultipleObjects]
public class LevelDataEditor : Editor
{
    private SerializedProperty genEventsProp;
    private ReorderableList reorderableList;

    private Type[] actionTypes;
    private string[] actionTypeNames;


    private void OnEnable()
    {
        genEventsProp = serializedObject.FindProperty("generationEvents");

        var baseType = typeof(BaseGenEvent);
        actionTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
            .ToArray();

        actionTypeNames = actionTypes.Select(t => t.Name).ToArray();


        // --- setup the reorderable list
        reorderableList = new ReorderableList(serializedObject, genEventsProp,
            draggable: true, displayHeader: true,
            displayAddButton: true, displayRemoveButton: true);

        reorderableList.drawHeaderCallback = (Rect r) =>
        {
            EditorGUI.LabelField(r, "Generation Events");
        };

        reorderableList.elementHeightCallback = (int index) =>
        {
            var elem = genEventsProp.GetArrayElementAtIndex(index);
            float baseLine = EditorGUIUtility.singleLineHeight * 2;
            if (elem.managedReferenceValue != null)
                baseLine += EditorGUI.GetPropertyHeight(elem, true) + 4;
            return baseLine;
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var elem = genEventsProp.GetArrayElementAtIndex(index);
            rect.y += 2;

            // Header row: Type dropdown + Name field
            var typeRect = new Rect(rect.x + 4, rect.y, rect.width * 0.4f - 4, EditorGUIUtility.singleLineHeight);
            var nameRect = new Rect(rect.x + rect.width * 0.4f + 8, rect.y, rect.width * 0.6f - 12, EditorGUIUtility.singleLineHeight);


            // --- type dropdown
            int current = GetTypeIndex(elem.managedReferenceFullTypename);
            int sel = EditorGUI.Popup(typeRect, current, actionTypeNames);

            if (sel != current)
            {
                elem.managedReferenceValue = Activator.CreateInstance(actionTypes[sel]);
            }

            // --- draw the fields of the instance
            if (elem.managedReferenceValue != null)
            {
                var actionObj = elem.managedReferenceValue;
                var nameProp = elem.FindPropertyRelative("eventName");
                if (nameProp != null) {
                    nameProp.stringValue = EditorGUI.TextField(nameRect, nameProp.stringValue);
                }


                var fieldRect = new Rect(rect.x + 4, rect.y + EditorGUIUtility.singleLineHeight + 4,
                                 rect.width - 8,
                                 EditorGUI.GetPropertyHeight(elem, true));
                EditorGUI.PropertyField(fieldRect, elem, true);
            }
        };
        
        reorderableList.onAddCallback = (list) => {
            genEventsProp.arraySize++;
            var newElement = genEventsProp.GetArrayElementAtIndex(genEventsProp.arraySize - 1);
            newElement.managedReferenceValue = Activator.CreateInstance(actionTypes[0]);
            serializedObject.ApplyModifiedProperties(); // Ensures it's visible right away
        };
    }
    

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();

        // serializedObject.Update();


        // EditorGUILayout.LabelField("Generation Events", EditorStyles.boldLabel);

        // for (int i = 0; i < genEventsProp.arraySize; i++) {
        //     var element = genEventsProp.GetArrayElementAtIndex(i);

        //     EditorGUILayout.BeginVertical("box");

        //     int currentIndex = GetTypeIndex(element.managedReferenceFullTypename);
        //     int newIndex = EditorGUILayout.Popup("Type", currentIndex, actionTypeNames);

        //     if (newIndex != currentIndex) {
        //         element.managedReferenceValue = Activator.CreateInstance(actionTypes[newIndex]);
        //     }

        //     if (element.managedReferenceValue != null) {
        //         EditorGUILayout.PropertyField(element, GUIContent.none, true);
        //     }

        //     if (GUILayout.Button("Remove")) {
        //         genEventsProp.DeleteArrayElementAtIndex(i);
        //         continue;
        //     }

        //     EditorGUILayout.EndVertical();
        // }

        // if (GUILayout.Button("Add Generation Event")) {
        //     genEventsProp.arraySize++;
        //     var newElement = genEventsProp.GetArrayElementAtIndex(genEventsProp.arraySize - 1);
        //     newElement.managedReferenceValue = Activator.CreateInstance(actionTypes[0]);
        // }

        // serializedObject.ApplyModifiedProperties();

        serializedObject.Update();

        // draw everything *except* the 'actions' list
        DrawPropertiesExcluding(serializedObject, "generationEvents");

        // now draw our reorderable polymorphic list
        reorderableList.DoLayoutList();

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
