using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Starting Equipment", menuName = "Inventory/New Starting Equipment")]
public class StartingEquipmentSO : ScriptableObject {
    public List<StartingEquipment> equipment;
}

[System.Serializable]
public struct StartingEquipment {
    public EquipmentSlot slot;
    public GameObject equipment;
}