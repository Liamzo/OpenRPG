using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

[RequireComponent(typeof(CharacterHandler))]
[RequireComponent(typeof(InventoryHandler))]
public class EquipmentHandler : MonoBehaviour, ISaveable
{
    public StartingEquipmentSO startingEquipment;

    InventoryHandler inventory;
    public ItemHandler[] currentEquipment;

    public delegate void OnEquipmentChanged (ItemHandler newItem, ItemHandler oldItem, EquipmentSlot equipmentSlot);
    public OnEquipmentChanged onEquipmentChanged;


    public Transform orbitPoint;
    public HandSpot rightMeleeSpot;
    public HandSpot leftMeleeSpot;

    public bool meleeDrawn = true;
    public bool wasMeleeDrawn = true;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        inventory = GetComponent<InventoryHandler>();
        GetComponent<ObjectHandler>().OnDeath += OnDeath;

        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new ItemHandler[numSlots];
    }

    public void SetStartingEquipment() {
        foreach (StartingEquipment equipment in startingEquipment.equipment) {
            ItemHandler item = Instantiate(equipment.baseStats.prefab).GetComponent<ItemHandler>();
            item.GetComponent<ObjectHandler>().CreateBaseObject(equipment.baseStats);
            item.objectHandler.spriteRenderer.enabled = false;
            item.PickUp(GetComponent<ObjectHandler>());
            Equip(item, equipment.slot);
        }

        ToggleMeleeRanged(true);
    }

    public void Equip (ItemHandler newItem, EquipmentSlot equipmentSlot) {
        int slot = (int) equipmentSlot;
        ItemHandler oldItem = currentEquipment[slot];

        Unequip(equipmentSlot);

        currentEquipment[slot] = newItem;

        inventory.Remove(newItem);

        newItem.gameObject.SetActive(true);

        newItem.objectHandler.spriteRenderer.enabled = false;

        newItem.Equip();
        onEquipmentChanged?.Invoke(newItem, oldItem, equipmentSlot);

        if (equipmentSlot == EquipmentSlot.RightHand || equipmentSlot == EquipmentSlot.RightRangedWeapon) {
            newItem.transform.parent = rightMeleeSpot.transform;
            WeaponHandler weapon = newItem.GetComponent<WeaponHandler>();

            if ((meleeDrawn && equipmentSlot == EquipmentSlot.RightHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.RightRangedWeapon)) {
                rightMeleeSpot.weapon = weapon;
                rightMeleeSpot.weapon.Unholster();
            } else {
                weapon.Holster();
            }
        } else if (equipmentSlot == EquipmentSlot.LeftHand || equipmentSlot == EquipmentSlot.LeftRangedWeapon) {
            newItem.transform.parent = leftMeleeSpot.transform;
            WeaponHandler weapon = newItem.GetComponent<WeaponHandler>();
            
            if ((meleeDrawn && equipmentSlot == EquipmentSlot.LeftHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.LeftRangedWeapon)) {
                leftMeleeSpot.weapon = weapon;
                leftMeleeSpot.weapon.Unholster();
            } else {
                weapon.Holster();
            }
        }
    }

    public void Unequip(EquipmentSlot equipmentSlot) {
        int slot = (int) equipmentSlot;
        if (currentEquipment[slot] != null) {
            ItemHandler oldItem = currentEquipment[slot];
            currentEquipment[slot] = null;

            inventory.Add(oldItem);

            oldItem.gameObject.SetActive(false);
            oldItem.Unequip();

            if ((meleeDrawn && equipmentSlot == EquipmentSlot.RightHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.RightRangedWeapon)) {
                rightMeleeSpot.weapon = null;
            } else if ((meleeDrawn && equipmentSlot == EquipmentSlot.LeftHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.LeftRangedWeapon)) {
                leftMeleeSpot.weapon = null;
            }

            onEquipmentChanged?.Invoke(null, oldItem, equipmentSlot);
        }
    }
    public void Unequip(ItemHandler equipment) {
        for (int i = 0; i < currentEquipment.Length; i++) {
            ItemHandler item = currentEquipment[i];

            if (equipment.Equals(item)) {
                currentEquipment[i] = null;

                inventory.Add(item);

                item.gameObject.SetActive(false);
                item.Unequip();

                EquipmentSlot equipmentSlot = (EquipmentSlot)i;
                if ((meleeDrawn && equipmentSlot == EquipmentSlot.RightHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.RightRangedWeapon)) {
                    rightMeleeSpot.weapon = null;
                } else if ((meleeDrawn && equipmentSlot == EquipmentSlot.LeftHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.LeftRangedWeapon)) {
                    leftMeleeSpot.weapon = null;
                }

                onEquipmentChanged?.Invoke(null, item, (EquipmentSlot)i);

                return;
            }
        }
    }

    public ItemHandler GetEquipment(EquipmentSlot equipmentSlot) {
        return currentEquipment[(int)equipmentSlot];
    }

    public void ToggleMeleeRanged(bool melee) {
        wasMeleeDrawn = meleeDrawn;

        if (meleeDrawn == melee) return;

        meleeDrawn = melee;

        if (melee == true) {
            ItemHandler weapon = currentEquipment[(int)EquipmentSlot.RightHand];
            if (weapon != null) {
                rightMeleeSpot.weapon = weapon.GetComponent<WeaponHandler>();
                rightMeleeSpot.weapon.Unholster();
            } else {
                rightMeleeSpot.weapon = null;
            }
            weapon = currentEquipment[(int)EquipmentSlot.LeftHand];
            if (weapon != null) {
                leftMeleeSpot.weapon = weapon.GetComponent<WeaponHandler>();
                leftMeleeSpot.weapon.Unholster();
            } else {
                leftMeleeSpot.weapon = null;
            }

            weapon = currentEquipment[(int)EquipmentSlot.RightRangedWeapon];
            if (weapon != null) {
                weapon.GetComponent<WeaponHandler>().Holster();
            }
            weapon = currentEquipment[(int)EquipmentSlot.LeftRangedWeapon];
            if (weapon != null) {
                weapon.GetComponent<WeaponHandler>().Holster();
            }
        } else {
            ItemHandler weapon = currentEquipment[(int)EquipmentSlot.RightRangedWeapon];
            if (weapon != null) {
                rightMeleeSpot.weapon = weapon.GetComponent<WeaponHandler>();
                rightMeleeSpot.weapon.Unholster();
            } else {
                rightMeleeSpot.weapon = null;
            }
            weapon = currentEquipment[(int)EquipmentSlot.LeftRangedWeapon];
            if (weapon != null) {
                leftMeleeSpot.weapon = weapon.GetComponent<WeaponHandler>();
                leftMeleeSpot.weapon.Unholster();
            } else {
                leftMeleeSpot.weapon = null;
            }

            weapon = currentEquipment[(int)EquipmentSlot.RightHand];
            if (weapon != null) {
                weapon.GetComponent<WeaponHandler>().Holster();
            }
            weapon = currentEquipment[(int)EquipmentSlot.LeftHand];
            if (weapon != null) {
                weapon.GetComponent<WeaponHandler>().Holster();
            }
        }
    }

    public void SpotLook(Vector3 target) {
        if (meleeDrawn) {
            rightMeleeSpot.AimAtTarget(target, EquipmentSlot.RightHand);
            //leftMeleeSpot.AimAtTarget(target, EquipmentSlot.LeftHand);
        } else {
            rightMeleeSpot.AimAtTarget(target, EquipmentSlot.RightRangedWeapon);
            leftMeleeSpot.AimAtTarget(target, EquipmentSlot.LeftRangedWeapon);
        }
    }



    void OnDeath(ObjectHandler obj) {
        // Drop all equipment
        for (int i = 0; i < currentEquipment.Length; i++) {
            if (currentEquipment[i] != null) {
                ItemHandler item = GetEquipment((EquipmentSlot) i);
                Unequip((EquipmentSlot) i);
                item.Drop();

                item.transform.position = item.transform.position + (Vector3) Random.insideUnitCircle;
                Vector3 randomDir = Random.insideUnitCircle.normalized;
                float randomDist = Random.Range(0.2f, 2f);
                item.transform.position += randomDir * randomDist;
            }
        }
    }



    public string SaveComponent()
    {
        string json = $"equipment: {{ items: [";

        for (int i = 0; i < currentEquipment.Length; i++) {
            if (currentEquipment[i] != null) {
                json += $"{{ slot: {i}, item: {JSONNode.Parse(currentEquipment[i].objectHandler.SaveObject())} }},";
            }
        }

        return json + "]}";
    }

    public void LoadComponent(JSONNode data)
    {
        foreach (JSONNode node in data["equipment"]["items"]) {
            (BaseStats baseStats, GameObject go) = PrefabManager.Instance.SpawnPrefab(node["item"]["prefabId"]);
            ObjectHandler item = go.GetComponent<ObjectHandler>();
            item.LoadObject(baseStats, node["item"]);
            
            //item.GetComponent<ItemHandler>().owner = GetComponent<CharacterHandler>();
            item.GetComponent<ItemHandler>().PickUp(GetComponent<ObjectHandler>());
            Equip(item.GetComponent<ItemHandler>(), (EquipmentSlot)((int)node["slot"]));
        }

        ToggleMeleeRanged(true);
    }

    public void CreateBase()
    {
        if (startingEquipment == null) {
            return;
        }

        SetStartingEquipment();
    }
}

public enum EquipmentSlot { 
    Head, 
    Face,
    Back,
    Body, 
    RightArm,
    LeftArm,
    RightHand, 
    LeftHand,
    RightRangedWeapon,
    LeftRangedWeapon,
    WornHands,
    Feet,
    FloatingNearby

}