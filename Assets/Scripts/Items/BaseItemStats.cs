using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class BaseItemStats : ScriptableObject
{
    public Sprite icon;
    public List<BaseItemAction> itemActions;
    public BaseItemAction defaultAction;

    public List<EquipmentSlot> possibleSlots;

    public int baseValue;
}
