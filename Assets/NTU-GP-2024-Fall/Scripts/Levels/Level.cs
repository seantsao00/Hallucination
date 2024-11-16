using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject fairyRespawnPoint;
    public GameObject bearRespawnPoint;
    public int levelIndex;
    void Start() {
        LevelData levelData = new LevelData();
        if (LevelManager.Instance.FindLevelData(levelIndex) == null) {
            levelData.levelIndex = levelIndex;
            levelData.fairyRespawnPosition = fairyRespawnPoint.transform.position;
            levelData.bearRespawnPosition = bearRespawnPoint.transform.position;
            LevelManager.Instance.AddLevel(levelData);
        }
        
    }
    
}
