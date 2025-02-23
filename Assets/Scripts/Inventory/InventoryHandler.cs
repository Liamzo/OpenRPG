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


    // Events
    public event System.Action<ItemHandler> OnItemAdded = delegate { };
    public event System.Action<ItemHandler> OnItemRemoved = delegate { };


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
            CharacterHandler characterHandler;
            if (TryGetComponent<CharacterHandler>(out characterHandler)) {
                return characterHandler.Attributes.GetAttribute(AttributeNames.Strength) * 15f;
            }

            return Mathf.Infinity;
        }
    }

    protected virtual void Awake() {
        
    }

    public virtual bool Add (ItemHandler item) {
        if (inventory.Count < slots) {
            inventory.Add(item);
            OnItemAdded(item);
            return true;
        } else {
            Debug.Log("Not enough room in inventory");
            return false;
        }
    }

    public virtual void Remove (ItemHandler item) {
        if (inventory.Remove(item))
            OnItemRemoved(item);
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
            (BaseStats baseStats, GameObject go) = PrefabManager.Instance.SpawnPrefab(node["prefabId"]);
            ObjectHandler item = go.GetComponent<ObjectHandler>();

            item.LoadObject(baseStats, node);
            item.GetComponent<ItemHandler>().PickUp(GetComponent<ObjectHandler>());
        }
    }

    public void CreateBase() {
        if (startingInventory != null) {
            coins = startingInventory.coins;

            foreach (BaseStats baseStats in startingInventory.items) {
                ItemHandler item = Instantiate(baseStats.prefab).GetComponent<ItemHandler>();
                item.GetComponent<ObjectHandler>().CreateBaseObject(baseStats);
                item.PickUp(GetComponent<ObjectHandler>());
            }
        }
    }
}
