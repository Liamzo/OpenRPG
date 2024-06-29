using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class Level
{
    public LevelData levelData;

    public List<ObjectHandler> items;
    List<JSONNode> itemsData;

    public List<ObjectHandler> characters;
    List<JSONNode> charactersData;

    public List<ObjectHandler> props;
    List<JSONNode> propsData;


    public Level(LevelData data) {
        levelData = data;
        items = new List<ObjectHandler>();
        itemsData = new List<JSONNode>();

        characters = new List<ObjectHandler>();
        charactersData = new List<JSONNode>();

        props = new List<ObjectHandler>();
        propsData = new List<JSONNode>();
    }

    public void SaveLevel() {
        itemsData.Clear();
        charactersData.Clear();
        propsData.Clear();
        
        foreach(ObjectHandler handler in items) {
            JSONNode itemData = JSONNode.Parse(handler.SaveObject());
            itemsData.Add(itemData);
        }
        
        foreach(ObjectHandler handler in characters) {
            JSONNode charData = JSONNode.Parse(handler.SaveObject());
            charactersData.Add(charData);
        }
        
        foreach(ObjectHandler handler in props) {
            JSONNode propData = JSONNode.Parse(handler.SaveObject());
            propsData.Add(propData);
        }
    }

    public void LoadLevel() {
        items.Clear();
        characters.Clear();
        props.Clear();

        foreach(JSONNode data in itemsData) {
            ObjectHandler item = GameManager.instance.SpawnPrefab(data["prefabId"]).GetComponent<ObjectHandler>();
            items.Add(item);
            item.LoadObject(data);
        }

        foreach(JSONNode data in charactersData) {
            ObjectHandler character = GameManager.instance.SpawnPrefab(data["prefabId"]).GetComponent<ObjectHandler>();
            characters.Add(character);
            character.LoadObject(data);
        }

        GameObject levelObject = GameObject.FindWithTag("Level");
        foreach (ObjectHandler prop in levelObject.GetComponentsInChildren<ObjectHandler>()) {
            GameObject.Destroy(prop.gameObject);
        }

        foreach(JSONNode data in propsData) {
            ObjectHandler prop = GameManager.instance.SpawnPrefab(data["prefabId"]).GetComponent<ObjectHandler>();
            props.Add(prop);
            prop.LoadObject(data);
        }
    }

    public ObjectHandler FindCharacter(int objectId) {
        foreach (ObjectHandler character in characters) {
            if (character.objectHandlerId == objectId) {
                return character;
            }
        }

        return null;
    }
}
