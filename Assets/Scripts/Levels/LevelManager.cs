using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public List<LevelData> allLevels;
    Dictionary<LevelData, Level> savedLevels = new Dictionary<LevelData, Level>();

    public Level currentLevel {get; private set;}


    // Events
    public event System.Action LoadLevelPre = delegate { }; // Current Level, New Level

    private void Awake() {
        instance = this;
    }

    public void LoadLevel(LevelData levelData) {
        // Unload previous level (save data)
        currentLevel?.SaveLevel();

        LoadLevelPre();

        // Load new level
        SceneManager.LoadScene(levelData.sceneName);

        if (savedLevels.ContainsKey(levelData)) {
            // Load level data
            currentLevel = savedLevels[levelData];
            StartCoroutine(StartLevel(false));
        } else {
            // First time entering level, so generate
            Level newLevel = new Level(levelData);
            currentLevel = newLevel;
            if (levelData.persistent)
                savedLevels.Add(levelData, newLevel);
            StartCoroutine(StartLevel(true));
        }
    }

    IEnumerator StartLevel(bool newLevel) {
        // Wait a frame for scene to load
        yield return null;

        if (newLevel) {
            currentLevel.levelData.GenerateLevel();
        } else {
            currentLevel.LoadLevel();
        }

        // Do Quest Level Events here, maybe with a LoadLevelPost Event
        // A Quest Level Event needs to have a Recurring or Once option
        // To handle levels that regen every time vs save data

        Player.instance.transform.position = currentLevel.levelData.spawnPosition;
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
