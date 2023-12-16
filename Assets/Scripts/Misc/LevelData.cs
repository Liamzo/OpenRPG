using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels/New Level")]
public class LevelData : ScriptableObject
{
    public string sceneName;
    public string levelName;
    public string x;
    public string y;
    public Sprite icon;

    [Header("First Generation Settings")]
    public List<ItemStart> items;

    public void GenerateLevel() {
        foreach (ItemStart itemStart in items) {
            GameObject item = Instantiate(itemStart.prefab);
            item.transform.position = new Vector3(itemStart.x, itemStart.y, 0f);
        }
    }
}

[Serializable]
public struct ItemStart {
    public GameObject prefab;
    public float x;
    public float y;
}
