using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    [SerializeReference] public List<StatBlock> statBlocks;
    
    public T GetStatBlock<T>() where T : StatBlock {
        return statBlocks.OfType<T>().FirstOrDefault();
    }
}