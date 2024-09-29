using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


[CreateAssetMenu(fileName = "Stats Info", menuName = "StatsInfo", order = 0)]
public class StatsInfoSO : ScriptableObject
{
    public List<WeaponStatInfo> weaponStatInfo;
}

[System.Serializable]
public class WeaponStatInfo {
    public WeaponStatNames statName;
    public string description;
    [SerializeField] public Sprite icon;

    public WeaponStatInfo (WeaponStatNames statName) {
        this.statName = statName;
    }
}