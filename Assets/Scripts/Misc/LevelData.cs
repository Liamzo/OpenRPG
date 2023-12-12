using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels/New Level")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public string x;
    public string y;
    public Sprite icon;
}
