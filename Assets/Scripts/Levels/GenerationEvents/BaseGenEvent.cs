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

    public static BaseStats PickRandomSpawn(List<SpawnChance> spawnChances) {
        float totalChance = 0f;
        spawnChances.ForEach(x => totalChance += x.chance);

        float randomChance = UnityEngine.Random.Range(0f, totalChance);

        float chanceCount = 0f;
        for (int i = 0; i < spawnChances.Count; i++)
        {
            chanceCount += spawnChances[i].chance;

            if (randomChance <= chanceCount) {
                return spawnChances[i].baseStats;
            }
        }

        Debug.LogError("Couldn't find a chance");
        return null;
    }
    public static GameObject PickRandomThingSpawn(List<ThingSpawnChance> spawnChances) {
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

    public static Vector2 PickRandomLocation(float minDistance, bool avoidSpawn = false) {
        Vector2 campPosition;
        bool validPosition = false;
        int attempts = 0;
        do
        {
            campPosition = new Vector2(
                UnityEngine.Random.Range(5f, 95f),
                UnityEngine.Random.Range(5f, 95f));
            
            validPosition = IsPositionValid(campPosition, minDistance, avoidSpawn);

            attempts++;
            if (attempts > 1000) // Prevent infinite loop in case of too many attempts
            {
                Debug.LogWarning("Unable to place all objects with given constraints.");
                return Vector2.zero;
            }

        } while (!validPosition);

        return campPosition;
    }

    protected static bool IsPositionValid(Vector2 position, float minDistance, bool avoidSpawn)
    {
        if (avoidSpawn) {
            if (Vector2.Distance(LevelManager.instance.currentLevel.levelData.spawnPosition, position) < 20f)
            {
                return false;
            }
        }

        foreach (GameObject existingPosition in LevelManager.instance.currentLevel.things)
        {
            if (Vector2.Distance(existingPosition.transform.position, position) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}


[Serializable]
public struct SpawnChance {
    public BaseStats baseStats;
    public float chance;
}

[Serializable]
public struct ThingSpawnChance {
    public GameObject prefab;
    public float chance;
}