using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseWeaponStats : StatBlock {
	public List<WeaponModSlot> neededModSlots;
	public List<WeaponModSlot> optionalModSlots;
	public List<WeaponMod> startingWeaponMods;

	public List<WeaponStatValue> stats;

	public float restingAngle;

	public BaseWeaponStats() {
		stats = new List<WeaponStatValue>() {
			new WeaponStatValue(WeaponStatNames.DamageRollCount, 0),
			new WeaponStatValue(WeaponStatNames.DamageRollValue, 0),
			new WeaponStatValue(WeaponStatNames.PenetrationValue, 0),
			new WeaponStatValue(WeaponStatNames.StaminaCostHold, 0),
			new WeaponStatValue(WeaponStatNames.StaminaCostEnd, 0),
			new WeaponStatValue(WeaponStatNames.Range, 0),
			new WeaponStatValue(WeaponStatNames.Blocking, 0),
		};
	}
}