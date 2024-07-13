using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData)), CanEditMultipleObjects]
public class LevelDataEditor : Editor
{
    public string[] options = new string[] {"Road", "Trees", "BanditCamp", "Shack", "Enemies"};
    public int index = 0;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        LevelData levelData = (LevelData) target;

        index = EditorGUILayout.Popup(index, options);
        
        if(GUILayout.Button("Add Generation Event")) {
            switch (index)
            {
                case 0:
                    levelData.generationEvents.Add(new RoadGenEvent());
                    break;
                case 1:
                    levelData.generationEvents.Add(new TreesGenEvent());
                    break;
                case 2:
                    levelData.generationEvents.Add(new BanditCampGenEvent());
                    break;
                case 3:
                    levelData.generationEvents.Add(new ShackSpawnEvent());
                    break;
                case 4:
                    levelData.generationEvents.Add(new EnemiesGenEvent());
                    break;
            }
        }
    }

    void GenerateEvent() {

    }
}
