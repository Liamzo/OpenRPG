using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public StatsInfoSO statsInfo;

    public WeaponStatInfo FindStatInfo(WeaponStatNames statName) {
        foreach (WeaponStatInfo statInfo in statsInfo.weaponStatInfo)
        {
            if (statInfo.statName == statName) {
                return statInfo;
            }
        }

        Debug.LogWarning("No stat found with that name: " + statName);
        return null;
    }
}
