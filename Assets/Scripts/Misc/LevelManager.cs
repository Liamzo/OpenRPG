using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public List<LevelData> allLevels;

    private void Awake() {
        instance = this;
    }

    public void LoadLevel(LevelData levelData) {
        // Unload previous level (save data)

        // Load new level
        SceneManager.LoadScene(levelData.sceneName);

        // Load level data
    }


    public LevelData FindLevelWithName(string levelName) {
        foreach(LevelData levelData in allLevels) {
            if (levelData.sceneName == levelName) {
                return levelData;
            }
        }

        return null;
    }
}
