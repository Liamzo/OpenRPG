using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Block", menuName = "Stats/New Stat Block")]
public class BaseStats : ScriptableObject {
    public string objectName;
    public string description;
    public ObjectType type;
    public int expValue;

    public List<ObjectStatValue> stats;
}