using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private void Awake() {
        instance = this;
    }

    public void LoadLevel(LevelData levelData) {
        // Unload previous level (save data)

        // Load new level
    }
}
