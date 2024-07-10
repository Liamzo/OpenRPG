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
        List<GameObject> loot = LootTableManager.Instance.GetRandomLoot(minAmount, maxAmount, tierBoost);

        foreach (GameObject go in loot) {
            ItemHandler item = Instantiate(go).GetComponent<ItemHandler>();
            item.GetComponent<ObjectHandler>().CreateBaseObject();
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
