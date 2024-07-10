using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootTableManager : MonoBehaviour
{
    public static LootTableManager Instance { get; private set; }

    public List<LootTier> itemTierList;

    private void Awake() {
        Instance = this;
    }

    public List<GameObject> GetRandomLoot(int minAmount, int maxAmount, int tierMod = 0, bool exclusive = false) {
        List<GameObject> loot = new List<GameObject>();
        List<SpawnChance> possibleSpawns = new List<SpawnChance>();

        int maxTier = LevelManager.instance.currentLevel.levelData.levelTier + tierMod;


        for (int i = 0; i <= maxTier && i < itemTierList.Count; i++)
        {
            possibleSpawns.AddRange(itemTierList[i].itemList);
        }

        int amount = Random.Range(minAmount, maxAmount);

        for (int i = 0; i < amount; i++)
        {
            GameObject go = BaseGenEvent.PickRandomSpawn(possibleSpawns);
            loot.Add(go);
        }

        return loot;
    }
}

[System.Serializable]
public class LootTier {
    public List<SpawnChance> itemList;
}
