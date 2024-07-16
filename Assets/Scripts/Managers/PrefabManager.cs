using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    public List<BaseStats> allBaseStats;
    public List<PrefabInfo> allThings;


    private void Awake() {
        Instance = this;

        allBaseStats = Resources.LoadAll<BaseStats>("ObjectStats/").ToList();

        for (int i = 0; i < allThings.Count; i++) {
            allThings[i].prefabId = allThings[i].prefab.GetComponent<Thing>().prefabId;
        }
    }

    public BaseStats FindBaseStatsById(string prefabId) {
        foreach (BaseStats baseStats in allBaseStats)
        {
            if (baseStats.prefabId == prefabId) {
                return baseStats;
            }
        }

        Debug.LogWarning("No prefab found with that ID: " + prefabId);
        return null;
    }


    public (BaseStats, GameObject) SpawnPrefab(string prefabId) {
        foreach (BaseStats baseStats in allBaseStats)
        {
            if (baseStats.prefabId == prefabId) {
                return (baseStats, Instantiate(baseStats.prefab));
            }
        }

        Debug.LogWarning("No prefab found with that ID: " + prefabId);
        return (null, null);
    }

    public GameObject SpawnThing(string prefabId) {
        foreach (PrefabInfo thing in allThings) {
            if (thing.prefabId == prefabId) {
                return Instantiate(thing.prefab);
            }
        }

        Debug.LogWarning("No prefab found with that ID: " + prefabId);
        return null;
    }
}

[Serializable]
public class PrefabInfo {
    public string prefabId;
    public GameObject prefab;
}