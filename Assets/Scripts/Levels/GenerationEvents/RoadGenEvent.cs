using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoadGenEvent : BaseGenEvent
{
    public int minWidth;
    public int maxWidth;
    public int minAmount;
    public int maxAmount;
    
    public override void Generate(LevelData level)
    {
        Debug.Log("gooby");
    }
}
