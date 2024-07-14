using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class RandomLootGeneration : MonoBehaviour, ISaveable
{
    public bool generateLoot = true;
    public int minAmount;
    public int maxAmount;
    public int tierBoost;

    public void CreateBase()
    {
        List<BaseStats> loot = LootTableManager.Instance.GetRandomLoot(minAmount, maxAmount, tierBoost);

        foreach (BaseStats baseStats in loot) {
            ItemHandler item = Instantiate(baseStats.prefab).GetComponent<ItemHandler>();
            item.GetComponent<ObjectHandler>().CreateBaseObject(baseStats);
            item.PickUp(GetComponent<ObjectHandler>());
        }
}

    public void LoadComponent(JSONNode data)
    {
        
    }

    public string SaveComponent()
    {
        return "";
    }
}
