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

    public Vector2 spawnPosition;

    [Header("First Generation Settings")]
    public List<ItemStart> items;
    public List<CharacterStart> characters;
    public List<CharacterStart> things;

    public virtual void GenerateLevel() {
        foreach (ItemStart itemStart in items) {
            ObjectHandler item = Instantiate(itemStart.prefab).GetComponent<ObjectHandler>();
            item.transform.position = new Vector3(itemStart.x, itemStart.y, 0f);
            LevelManager.instance.currentLevel.items.Add(item);
            item.CreateBaseObject();
        }

        foreach (CharacterStart characterStart in characters) {
            ObjectHandler character = Instantiate(characterStart.prefab).GetComponent<ObjectHandler>();
            character.transform.position = new Vector3(characterStart.x, characterStart.y, 0f);
            LevelManager.instance.currentLevel.characters.Add(character);
            character.CreateBaseObject();
        }

        GameObject levelObject = GameObject.FindWithTag("Level");
        foreach (ObjectHandler prop in levelObject.GetComponentsInChildren<ObjectHandler>()) {
            LevelManager.instance.currentLevel.props.Add(prop);
            prop.CreateBaseObject();
        }

    }
}

[Serializable]
public struct ItemStart {
    public GameObject prefab;
    public float x;
    public float y;
}

[Serializable]
public struct CharacterStart {
    public GameObject prefab;
    public float x;
    public float y;
}
