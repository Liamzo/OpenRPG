using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public class BaseWeaponStats : StatBlock {
	public List<WeaponModSlot> neededModSlots;
	public List<WeaponModSlot> optionalModSlots;
	public List<WeaponMod> startingWeaponMods;

	public List<WeaponStatValue> stats;

	public AnimatorController animationController;
	public Vector3 spriteRotation;
	public Vector3 attackPoint;
	public Vector4 colliderSize;

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