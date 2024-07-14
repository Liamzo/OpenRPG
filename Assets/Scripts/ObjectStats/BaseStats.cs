using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Block", menuName = "Stats/New Stat Block")]
public class BaseStats : ScriptableObject {
    public string objectName;
    public Sprite sprite;
    public GameObject prefab;
    public string description;
    public ObjectType type;
    public int expValue;

    public List<ObjectStatValue> stats;

    [SerializeReference] public List<TestClass> statBlocks;
}