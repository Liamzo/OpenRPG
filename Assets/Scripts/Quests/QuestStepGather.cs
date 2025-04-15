using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


[System.Serializable]
public class QuestStepGather : QuestStep
{
    public string target;
    public int neededAmount;
    public int currentAmount;

    public QuestStepGather(JSONNode json, Quest parent) : base(json, parent) {
        target = json["target"];
        neededAmount = (int)json["amount"];
        currentAmount = 0;
    }

    public override void Begin()
    {
        base.Begin();
        
        Player.Instance.GetComponent<InventoryHandler>().OnItemAdded += CheckInventory;
        Player.Instance.GetComponent<InventoryHandler>().OnItemRemoved += CheckInventory;

        CheckInventory(null);
    }

    public override void Complete()
    {
        base.Complete();

        Player.Instance.GetComponent<InventoryHandler>().OnItemAdded -= CheckInventory;
        Player.Instance.GetComponent<InventoryHandler>().OnItemRemoved -= CheckInventory;
    }

    public override (string, string) GetText()
    {
        return (name, $"{currentAmount}/{neededAmount}");
    }

    void CheckInventory(ItemHandler item) {
        currentAmount = 0;

        foreach (ItemHandler inventoryItem in Player.Instance.GetComponent<InventoryHandler>().inventory) {
            if (inventoryItem.objectHandler.prefabId == target)
                currentAmount++;
        }

        if (currentAmount >= neededAmount) {
            Complete();
        }
    }
}
