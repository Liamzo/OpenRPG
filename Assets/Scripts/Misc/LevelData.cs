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
    public List<CharacterStart> characters;

    public virtual void GenerateLevel() {
        foreach (ItemStart itemStart in items) {
            ObjectHandler item = Instantiate(itemStart.prefab).GetComponent<ObjectHandler>();
            item.transform.position = new Vector3(itemStart.x, itemStart.y, 0f);
            item.CreateBaseObject();
            LevelManager.instance.currentLevel.items.Add(item.GetComponent<ObjectHandler>());
        }

        foreach (CharacterStart characterStart in characters) {
            ObjectHandler character = Instantiate(characterStart.prefab).GetComponent<ObjectHandler>();
            character.transform.position = new Vector3(characterStart.x, characterStart.y, 0f);
            character.CreateBaseObject();
            LevelManager.instance.currentLevel.characters.Add(character.GetComponent<ObjectHandler>());
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
