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

    public List<GameObject> things;
    public List<JSONNode> thingsSaved;

    public List<ExclusionZone> exclusionZones;


    public Level(LevelData data) {
        levelData = data;

        items = new List<ObjectHandler>();
        itemsData = new List<JSONNode>();

        characters = new List<ObjectHandler>();
        charactersData = new List<JSONNode>();

        props = new List<ObjectHandler>();
        propsData = new List<JSONNode>();

        things = new List<GameObject>();
        thingsSaved = new List<JSONNode>();

        exclusionZones = new ();
    }

    public void SaveLevel() {
        itemsData.Clear();
        charactersData.Clear();
        propsData.Clear();
        thingsSaved.Clear();
        
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
        
        foreach (GameObject thing in things)
        {
            JSONNode propData = JSONNode.Parse(thing.GetComponent<Thing>().SaveObject());
            thingsSaved.Add(propData);
        }
    }

    public void LoadLevel() {
        items.Clear();
        characters.Clear();
        props.Clear();
        things.Clear();

        foreach(JSONNode data in itemsData) {
            (BaseStats baseStats, GameObject go) = PrefabManager.Instance.SpawnPrefab(data["prefabId"]);
            ObjectHandler item = go.GetComponent<ObjectHandler>();
            items.Add(item);
            item.LoadObject(baseStats, data);
        }

        foreach(JSONNode data in charactersData) {
            (BaseStats baseStats, GameObject go) = PrefabManager.Instance.SpawnPrefab(data["prefabId"]);
            ObjectHandler character = go.GetComponent<ObjectHandler>();
            characters.Add(character);
            character.LoadObject(baseStats, data);
        }

        GameObject levelObject = GameObject.FindWithTag("Level");
        foreach (ObjectHandler prop in levelObject.GetComponentsInChildren<ObjectHandler>()) {
            GameObject.Destroy(prop.gameObject);
        }

        foreach(JSONNode data in propsData) {
            (BaseStats baseStats, GameObject go) = PrefabManager.Instance.SpawnPrefab(data["prefabId"]);
            ObjectHandler prop = go.GetComponent<ObjectHandler>();
            props.Add(prop);
            prop.LoadObject(baseStats, data);
        }

        foreach (JSONNode thingSaved in thingsSaved)
        {
            GameObject thing = PrefabManager.Instance.SpawnThing(thingSaved["prefabId"]);
            things.Add(thing);
            thing.GetComponent<Thing>().LoadObject(thingSaved);
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
