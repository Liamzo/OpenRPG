using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Level
{
    public LevelData levelData;

    public List<ObjectHandler> items;
    List<JSONNode> itemsData;

    JSONNode savedLevelData;


    public Level(LevelData data) {
        levelData = data;
        items = new List<ObjectHandler>();
        itemsData = new List<JSONNode>();
    }

    public void SaveLevel() {
        itemsData.Clear();
        
        foreach(ObjectHandler handler in items) {
            JSONNode itemData = handler.SaveObject();
            itemsData.Add(itemData);
        }
    }

    public void LoadLevel() {
        items.Clear();

        foreach(JSONNode data in itemsData) {
            GameObject item = GameManager.instance.SpawnPrefab(data["prefabId"]);
            item.transform.position = new Vector3(data["x"], data["y"], 0f);
            items.Add(item.GetComponent<ObjectHandler>());
        }
    }
}
