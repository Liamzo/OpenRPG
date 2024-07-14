using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public List<Transform> lootContainerSpawnLocations;
    public List<SpawnChance> lootContainerPrefabs;


    public void Generate() {
        // Loot containers
        float totalChance = 0f;
        lootContainerPrefabs.ForEach(x => totalChance += x.chance);
        foreach (Transform transform in lootContainerSpawnLocations)
        {
            BaseStats lootContainerPrefab = BaseGenEvent.PickRandomSpawn(lootContainerPrefabs);
            ObjectHandler prop = Instantiate(lootContainerPrefab.prefab).GetComponent<ObjectHandler>();
            prop.transform.position = transform.position;
            LevelManager.instance.currentLevel.props.Add(prop);
            prop.CreateBaseObject(lootContainerPrefab);
        }
    }
}
