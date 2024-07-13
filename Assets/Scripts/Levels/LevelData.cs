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
    public List<ItemStart> items;
    public List<CharacterStart> characters;
    public List<CharacterStart> props;
    public List<CharacterStart> things;
    [SerializeReference] public List<BaseGenEvent> generationEvents = new List<BaseGenEvent>();

    public virtual void GenerateLevel() {
        foreach (ItemStart itemStart in items) {
            ObjectHandler item = Instantiate(itemStart.prefab).GetComponent<ObjectHandler>();
            item.transform.position = itemStart.position;
            LevelManager.instance.currentLevel.items.Add(item);
            item.CreateBaseObject();
        }

        foreach (CharacterStart characterStart in characters) {
            ObjectHandler character = Instantiate(characterStart.prefab).GetComponent<ObjectHandler>();
            character.transform.position = characterStart.position;
            LevelManager.instance.currentLevel.characters.Add(character);
            character.CreateBaseObject();
        }
        
        foreach (CharacterStart characterStart in props) {
            ObjectHandler prop = Instantiate(characterStart.prefab).GetComponent<ObjectHandler>();
            prop.transform.position = characterStart.position;
            LevelManager.instance.currentLevel.props.Add(prop);
            prop.CreateBaseObject();
        }

        foreach (CharacterStart thing in things) {
            GameObject go = Instantiate(thing.prefab);
            go.transform.position = thing.position;
            LevelManager.instance.currentLevel.things.Add(go);
        }

        GameObject levelObject = GameObject.FindWithTag("Level");
        // Only works for now because everything in Town with an ObjectHandler is a Prop. Would break if any Items or Characters were present
        foreach (ObjectHandler prop in levelObject.GetComponentsInChildren<ObjectHandler>()) {
            LevelManager.instance.currentLevel.props.Add(prop);
            prop.CreateBaseObject();
        }

        foreach (BaseGenEvent generationEvent in generationEvents)
        {
            generationEvent.Generate(this);
        }

    }
}

[Serializable]
public struct ItemStart {
    public GameObject prefab;
    public Vector2 position;
}

[Serializable]
public struct CharacterStart {
    public GameObject prefab;
    public Vector2 position;
}
