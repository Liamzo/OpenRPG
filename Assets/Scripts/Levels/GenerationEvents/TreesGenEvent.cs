using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreesGenEvent : BaseGenEvent
{
    public List<ThingSpawnChance> treePrefabs;
    public float treeCoverage;

    
    public override void Generate(LevelData level)
    {
        for (int i = 0; i < treeCoverage; i++)
        {
            Vector2 newPosition = PickRandomLocation(3f);
            GameObject treePrefab = BaseGenEvent.PickRandomThingSpawn(treePrefabs);
            GameObject go = GameObject.Instantiate(treePrefab);

            go.transform.position = newPosition;
            
            LevelManager.instance.currentLevel.things.Add(go);
        }
    }
}
