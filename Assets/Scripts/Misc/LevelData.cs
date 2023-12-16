using System;
using System.Collections.Generic;
using UnityEngine;

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
            LevelManager.instance.currentLevel.items.Add(item.GetComponent<ObjectHandler>());
        }
    }
}

[Serializable]
public struct ItemStart {
    public GameObject prefab;
    public float x;
    public float y;
}
