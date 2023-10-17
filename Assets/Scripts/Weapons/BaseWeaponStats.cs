using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Stat Block", menuName = "Stats/New Weapon Stat Block")]
public class BaseWeaponStats : ScriptableObject {
        public List<WeaponStatValue> stats;

        public float restingAngle;
}