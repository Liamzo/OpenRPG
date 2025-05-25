using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShackSpawnEvent : BaseGenEvent
{
    public List<ThingSpawnChance> shackPrefabs;
    public int minAmount;
    public int maxAmount;

    public override void Generate(LevelData level)
    {
        int amount = Random.Range(minAmount, maxAmount+1);

        for (int i = 0; i < amount; i++)
        {
            Vector2 newPosition = PickRandomLocation(5f);
            GameObject shackPrefab = BaseGenEvent.PickRandomThingSpawn(shackPrefabs);
            GameObject go = GameObject.Instantiate(shackPrefab);

            go.transform.position = newPosition;
            
            LevelManager.instance.currentLevel.things.Add(go);

            go.GetComponentsInChildren<Building>().ToList().ForEach(x => x.Generate());

            LevelManager.instance.currentLevel.exclusionZones.Add(new CircleExclusionZone(newPosition, 5f));
        }
    }
}
