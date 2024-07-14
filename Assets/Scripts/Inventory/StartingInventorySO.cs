using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Starting Inventory", menuName = "Inventory/New Starting Inventory")]
public class StartingInventorySO : ScriptableObject {
    public int coins;
    public List<BaseStats> items;
}