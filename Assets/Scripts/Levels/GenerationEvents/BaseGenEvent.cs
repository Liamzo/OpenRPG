using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseGenEvent
{
    public virtual void Generate(LevelData level) {

    }

    protected bool IsPositionValid(Vector2 position, float minDistance)
    {
        foreach (GameObject existingPosition in LevelManager.instance.currentLevel.things)
        {
            if (Vector2.Distance(existingPosition.transform.position, position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    public static GameObject PickRandomSpawn(List<SpawnChance> spawnChances) {
        float totalChance = 0f;
        spawnChances.ForEach(x => totalChance += x.chance);

        float randomChance = UnityEngine.Random.Range(0f, totalChance);

        float chanceCount = 0f;
        for (int i = 0; i < spawnChances.Count; i++)
        {
            chanceCount += spawnChances[i].chance;

            if (randomChance <= chanceCount) {
                return spawnChances[i].prefab;
            }
        }

        Debug.LogError("Couldn't find a chance");
        return null;
    }
}


[Serializable]
public struct SpawnChance {
    public GameObject prefab;
    public float chance;
}