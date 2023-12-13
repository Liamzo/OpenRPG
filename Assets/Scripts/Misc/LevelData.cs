using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Level", menuName = "Levels/New Level")]
public class LevelData : ScriptableObject
{
    public string sceneName;
    public string levelName;
    public string x;
    public string y;
    public Sprite icon;
}
