using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterHandler))]
[RequireComponent(typeof(InventoryHandler))]
public class EquipmentHandler : MonoBehaviour
{
    public StartingEquipmentSO startingEquipment;

    InventoryHandler inventory;
    public ItemHandler[] currentEquipment;

    public delegate void OnEquipmentChanged (ItemHandler newItem, ItemHandler oldItem, EquipmentSlot equipmentSlot);
    public OnEquipmentChanged onEquipmentChanged;


    public HandSpot rightMeleeSpot;
    public HandSpot leftMeleeSpot;

    bool meleeDrawn = true;
    public bool wasMeleeDrawn = true;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GetComponent<InventoryHandler>();
        SetStartingEquipment();
        GetComponent<ObjectHandler>().OnDeath += OnDeath;
    }

    public void SetStartingEquipment() {
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new ItemHandler[numSlots];

        if (startingEquipment == null) {
            return;
        }

        foreach (StartingEquipment equipment in startingEquipment.equipment) {
            ItemHandler item = Instantiate(equipment.equipment).GetComponent<ItemHandler>();
            item.owner = GetComponent<CharacterHandler>();
            Equip(item, equipment.slot);
        }

        ToggleMeleeRanged(true);
    }

    public void Equip (ItemHandler newItem, EquipmentSlot equipmentSlot) {
        int slot = (int) equipmentSlot;
        ItemHandler oldItem = currentEquipment[slot];

        Unequip(equipmentSlot);

        if (onEquipmentChanged != null) {
            onEquipmentChanged.Invoke(newItem, oldItem, equipmentSlot);
        }

        currentEquipment[slot] = newItem;
        inventory.Remove(newItem);
        newItem.itemHitbox.enabled = false;
        newItem.gameObject.SetActive(true);

        if (equipmentSlot == EquipmentSlot.RightHand || equipmentSlot == EquipmentSlot.RightRangedWeapon) {
            newItem.transform.parent = rightMeleeSpot.transform;
            BaseWeaponHandler weapon = newItem.GetComponent<BaseWeaponHandler>();

            if ((meleeDrawn && equipmentSlot == EquipmentSlot.RightHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.RightRangedWeapon)) {
                rightMeleeSpot.weapon = weapon;
                rightMeleeSpot.weapon.Unholster();
            } else {
                weapon.Holster();
            }
        } else if (equipmentSlot == EquipmentSlot.LeftHand || equipmentSlot == EquipmentSlot.LeftRangedWeapon) {
            newItem.transform.parent = leftMeleeSpot.transform;
            BaseWeaponHandler weapon = newItem.GetComponent<BaseWeaponHandler>();
            
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
            inventory.Add(oldItem);

            currentEquipment[slot] = null;

            oldItem.gameObject.SetActive(false);
            oldItem.itemHitbox.enabled = false;
            oldItem.transform.parent = null;

            if ((meleeDrawn && equipmentSlot == EquipmentSlot.RightHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.RightRangedWeapon)) {
                rightMeleeSpot.weapon = null;
            } else if ((meleeDrawn && equipmentSlot == EquipmentSlot.LeftHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.LeftRangedWeapon)) {
                leftMeleeSpot.weapon = null;
            }

            if (onEquipmentChanged != null) {
                onEquipmentChanged.Invoke(null, oldItem, equipmentSlot);
            }
        }
    }
    public void Unequip(ItemHandler equipment) {
        for (int i = 0; i < currentEquipment.Length; i++) {
            ItemHandler item = currentEquipment[i];

            if (equipment.Equals(item)) {
                inventory.Add(item);

                currentEquipment[i] = null;

                item.gameObject.SetActive(false);
                item.itemHitbox.enabled = false;
                item.transform.parent = null;

                EquipmentSlot equipmentSlot = (EquipmentSlot)i;

                if ((meleeDrawn && equipmentSlot == EquipmentSlot.RightHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.RightRangedWeapon)) {
                    rightMeleeSpot.weapon = null;
                } else if ((meleeDrawn && equipmentSlot == EquipmentSlot.LeftHand) || (!meleeDrawn && equipmentSlot == EquipmentSlot.LeftRangedWeapon)) {
                    leftMeleeSpot.weapon = null;
                }

                if (onEquipmentChanged != null) {
                    onEquipmentChanged.Invoke(null, item, (EquipmentSlot)i);
                }

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
                rightMeleeSpot.weapon = weapon.GetComponent<BaseWeaponHandler>();
                rightMeleeSpot.weapon.Unholster();
            }
            weapon = currentEquipment[(int)EquipmentSlot.LeftHand];
            if (weapon != null) {
                leftMeleeSpot.weapon = weapon.GetComponent<BaseWeaponHandler>();
                leftMeleeSpot.weapon.Unholster();
            }

            weapon = currentEquipment[(int)EquipmentSlot.RightRangedWeapon];
            if (weapon != null) {
                weapon.GetComponent<BaseWeaponHandler>().Holster();
            }
            weapon = currentEquipment[(int)EquipmentSlot.LeftRangedWeapon];
            if (weapon != null) {
                weapon.GetComponent<BaseWeaponHandler>().Holster();
            }
        } else {
            ItemHandler weapon = currentEquipment[(int)EquipmentSlot.RightRangedWeapon];
            if (weapon != null) {
                rightMeleeSpot.weapon = weapon.GetComponent<BaseWeaponHandler>();
                rightMeleeSpot.weapon.Unholster();
            }
            weapon = currentEquipment[(int)EquipmentSlot.LeftRangedWeapon];
            if (weapon != null) {
                leftMeleeSpot.weapon = weapon.GetComponent<BaseWeaponHandler>();
                leftMeleeSpot.weapon.Unholster();
            }

            weapon = currentEquipment[(int)EquipmentSlot.RightHand];
            if (weapon != null) {
                weapon.GetComponent<BaseWeaponHandler>().Holster();
            }
            weapon = currentEquipment[(int)EquipmentSlot.LeftHand];
            if (weapon != null) {
                weapon.GetComponent<BaseWeaponHandler>().Holster();
            }
        }
    }

    public void SpotLook(Vector3 target) {
        if (meleeDrawn) {
            rightMeleeSpot.AimAtTarget(target, EquipmentSlot.RightHand);
            leftMeleeSpot.AimAtTarget(target, EquipmentSlot.LeftHand);
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
            }
        }
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