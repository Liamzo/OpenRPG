using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels/New Level")]
public class LevelData : ScriptableObject
{
    public string sceneName;
    public string levelName;
    public Vector2 levelPosition;
    public Sprite icon;

    public int levelTier;
    public bool persistent;

    public Vector2 spawnPosition;


    [Header("Audio")]
    public List<AudioID> ambientAudioList;
    public List<AudioID> musicAudioList;


    [Header("First Generation Settings")]
    public List<ObjectStart> items;
    public List<ObjectStart> characters;
    public List<ObjectStart> props;
    public List<ThingStart> things;
    [SerializeReference] public List<BaseGenEvent> generationEvents = new List<BaseGenEvent>();

    public virtual void GenerateLevel() {
        foreach (ObjectStart objectStart in items) {
            ObjectHandler item = Instantiate(objectStart.baseStats.prefab).GetComponent<ObjectHandler>();
            item.transform.position = objectStart.position;
            LevelManager.instance.currentLevel.items.Add(item);
            item.CreateBaseObject(objectStart.baseStats);
        }

        foreach (ObjectStart objectStart in characters) {
            ObjectHandler character = Instantiate(objectStart.baseStats.prefab).GetComponent<ObjectHandler>();
            character.transform.position = objectStart.position;
            LevelManager.instance.currentLevel.characters.Add(character);
            character.CreateBaseObject(objectStart.baseStats);
        }
        
        foreach (ObjectStart objectStart in props) {
            ObjectHandler prop = Instantiate(objectStart.baseStats.prefab).GetComponent<ObjectHandler>();
            prop.transform.position = objectStart.position;
            LevelManager.instance.currentLevel.props.Add(prop);
            prop.CreateBaseObject(objectStart.baseStats);
        }

        foreach (ThingStart thingStart in things) {
            GameObject go = Instantiate(thingStart.prefab);
            go.transform.position = thingStart.position;
            LevelManager.instance.currentLevel.things.Add(go);
        }

        GameObject levelObject = GameObject.FindWithTag("Level");
        // Only works for now because everything in Town with an ObjectHandler is a Prop. Would break if any Items or Characters were present
        // Anything already in scene will need to have Base Stats assigned manually somehow
        foreach (ObjectHandler prop in levelObject.GetComponentsInChildren<ObjectHandler>()) {
            LevelManager.instance.currentLevel.props.Add(prop);
            prop.CreateBaseObject(prop.baseStats);
        }

        foreach (BaseGenEvent generationEvent in generationEvents)
        {
            generationEvent.Generate(this);
        }

    }
}

[Serializable]
public struct ObjectStart {
    public BaseStats baseStats;
    public Vector2 position;
}

[Serializable]
public struct ThingStart {
    public GameObject prefab;
    public Vector2 position;
}
