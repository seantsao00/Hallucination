using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData {
    public int levelIndex;
    public Vector3 fairyRespawnPosition;
    public Vector3 bearRespawnPosition;
}

public class LevelManager : MonoBehaviour {
    public static LevelManager Instance;
    public int currentLevelIndex;
    List<LevelData> levelDatas = new List<LevelData>();
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
    }
    public void ChangeLevel(int levelIndex) {
        currentLevelIndex = levelIndex;
    }

    public void AddLevel(LevelData levelData) {
        levelDatas.Add(levelData);
    }

    public LevelData GetCurrentLevelData() {
        foreach (var levelData in levelDatas) {
            if (levelData.levelIndex == currentLevelIndex) {
                return levelData;
            }
        }
        return null;
    }
    
    public LevelData FindLevelData(int levelIndex) {
        foreach (var levelData in levelDatas) {
            if (levelData.levelIndex == levelIndex) {
                return levelData;
            }
        }
        return null;
    }

}
