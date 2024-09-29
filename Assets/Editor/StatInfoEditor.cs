using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatsInfoSO)), CanEditMultipleObjects]
public class StatInfoEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();


        StatsInfoSO statsInfo = (StatsInfoSO) target;

        if (statsInfo.weaponStatInfo == null || statsInfo.weaponStatInfo.Count == 0) {
            statsInfo.weaponStatInfo = new List<WeaponStatInfo>();

            foreach (WeaponStatNames weaponStatNames in System.Enum.GetValues(typeof(WeaponStatNames)))
            {
                statsInfo.weaponStatInfo.Add(new WeaponStatInfo(weaponStatNames));
            }
        }
    }
}
