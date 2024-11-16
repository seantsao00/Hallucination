using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNavigator : MonoBehaviour {
    public static LevelNavigator Instance { get; private set; }
    [SerializeField] LevelController startLevel;
    [SerializeField] LevelController[] levels;
    string[] levelNames;
    int currentLevelIndex;

    public LevelController CurrentLevel => levels[currentLevelIndex];

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentLevelIndex = Array.IndexOf(levels, startLevel);
            SceneManager.sceneLoaded += OnSceneLoaded;
            levelNames = levels.Select(level => level.gameObject.name).ToArray();
        }
    }

    void Start() {
        startLevel.StartLevel();
    }

    public void RestartCurrentLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Debug.Log($"Scene Restarted. Current level: {currentLevelIndex}");
        InputManager.Instance.SetNormalMode();
        WorldSwitchManager.Instance.ClearLocks();
        CurrentLevel.RestartLevel();
    }

    public void CompleteCurrentLevel() {
        currentLevelIndex += 1;
        if (currentLevelIndex == levels.Length) {
            Debug.LogWarning("The last level completed");
        } else {
            CurrentLevel.StartLevel();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        var newLevels = FindObjectsOfType<LevelController>();
        Array.Sort(newLevels, (a, b) => {
            int indexA = Array.FindIndex(levelNames, name => name == a.gameObject.name);
            int indexB = Array.FindIndex(levelNames, name => name == b.gameObject.name);
            return indexA.CompareTo(indexB);
        });

        levels = newLevels;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
