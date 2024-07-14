using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    public List<PrefabInfo> allItems;
    public List<PrefabInfo> allCharacters;
    public List<PrefabInfo> allProps;
    public List<ThingInfo> allThings;


    private void Awake() {
        Instance = this;

        for (int i = 0; i < allItems.Count; i++) {
            allItems[i].prefabId = allItems[i].prefab.prefab.GetComponent<ObjectHandler>().prefabId;
        }

        for (int i = 0; i < allCharacters.Count; i++) {
            allCharacters[i].prefabId = allCharacters[i].prefab.prefab.GetComponent<ObjectHandler>().prefabId;
        }

        for (int i = 0; i < allProps.Count; i++) {
            allProps[i].prefabId = allProps[i].prefab.prefab.GetComponent<ObjectHandler>().prefabId;
        }

        for (int i = 0; i < allThings.Count; i++) {
            allThings[i].prefabId = allThings[i].prefab.GetComponent<Thing>().prefabId;
        }
    }


    public (BaseStats, GameObject) SpawnPrefab(string prefabId) {
        foreach (PrefabInfo info in allItems) {
            if (info.prefabId == prefabId) {
                return (info.prefab, Instantiate(info.prefab.prefab));
            }
        }

        foreach (PrefabInfo info in allCharacters) {
            if (info.prefabId == prefabId) {
                return (info.prefab, Instantiate(info.prefab.prefab));
            }
        }

        foreach (PrefabInfo info in allProps) {
            if (info.prefabId == prefabId) {
                return (info.prefab, Instantiate(info.prefab.prefab));
            }
        }

        Debug.LogWarning("No prefab found with that ID");
        return (null, null);
    }

    public GameObject SpawnThing(string prefabId) {
        foreach (ThingInfo thing in allThings) {
            if (thing.prefabId == prefabId) {
                return Instantiate(thing.prefab);
            }
        }

        Debug.LogWarning("No prefab found with that ID");
        return null;
    }
}

[Serializable]
public class PrefabInfo {
    public string prefabId;
    public BaseStats prefab;
}

[Serializable]
public class ThingInfo {
    public string prefabId;
    public GameObject prefab;
}