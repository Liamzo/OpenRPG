using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class InventoryHandler : MonoBehaviour, ISaveable
{
    public int slots;
    public StartingInventorySO startingInventory;

    public List<ItemHandler> inventory = new List<ItemHandler>();

    public int coins;

    public float carryWeightCurrent {
        get {
            float weight = 0;
            
            foreach (ItemHandler item in inventory) {
                weight += item.objectHandler.statsObject[ObjectStatNames.Weight].GetValue();
            }

            return weight;
        }
    }

    public float carryWeightMax {
        get {
            AttributeHandler attributeHandler;
            if (TryGetComponent<AttributeHandler>(out attributeHandler)) {
                return attributeHandler.stats[AttributeNames.Strength].GetValue() * 15f;
            }

            return Mathf.Infinity;
        }
    }

    protected virtual void Awake() {
        
    }

    public virtual bool Add (ItemHandler item) {
        if (inventory.Count < slots) {
            inventory.Add(item);
            return true;
        } else {
            Debug.Log("Not enough room in inventory");
            return false;
        }
    }

    public virtual void Remove (ItemHandler item) {
        inventory.Remove(item);
    }

    public string SaveComponent()
    {
        string json = $"inventory: {{ coins: {coins}, items: [";

        foreach (ItemHandler item in inventory) {
            json += item.objectHandler.SaveObject() + ",";
        }

        return json + "]}";
    }

    public void LoadComponent(JSONNode data)
    {
        coins = data["inventory"]["coins"];

        foreach (JSONNode node in data["inventory"]["items"]) {
            ObjectHandler item = GameManager.instance.SpawnPrefab(node["prefabId"]).GetComponent<ObjectHandler>();
            item.LoadObject(node);
            item.GetComponent<ItemHandler>().PickUp(GetComponent<ObjectHandler>());
        }
    }

    public void CreateBase() {
        if (startingInventory != null) {
            coins = startingInventory.coins;

            foreach (GameObject go in startingInventory.items) {
                ItemHandler item = Instantiate(go).GetComponent<ItemHandler>();
                item.GetComponent<ObjectHandler>().CreateBaseObject();
                item.PickUp(GetComponent<ObjectHandler>());
            }
        }
    }
}
