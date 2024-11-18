using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNavigator : MonoBehaviour {
    public static LevelNavigator Instance { get; private set; }
    [SerializeField] LevelController startLevel;
    [SerializeField] LevelController[] levels;
    string[] levelNames;
    int currentLevelIndex;
    bool firstLoad = true;

    public LevelController CurrentLevel => levels[currentLevelIndex];

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            currentLevelIndex = Array.IndexOf(levels, startLevel);
            levelNames = levels.Select(level => level.gameObject.name).ToArray();
        }
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void RestartCurrentLevel() {
        StartCoroutine(PerformSceneRestart());
    }

    IEnumerator PerformSceneRestart() {
        yield return Util.FadeOut(1f, WorldSwitchManager.Instance.FadingMask);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CompleteCurrentLevel() {
        currentLevelIndex += 1;
        if (currentLevelIndex == levels.Length) {
            Debug.LogWarning("The last level completed");
        } else {
            Debug.Log($"New Level: {CurrentLevel.gameObject.name}");
            CurrentLevel.StartLevel();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "MainMenu") return;
        var newLevels = FindObjectsOfType<LevelController>();
        Array.Sort(newLevels, (a, b) => {
            int indexA = Array.FindIndex(levelNames, name => name == a.gameObject.name);
            int indexB = Array.FindIndex(levelNames, name => name == b.gameObject.name);
            return indexA.CompareTo(indexB);
        });

        levels = newLevels;
        if (firstLoad) {
            firstLoad = false;
            CurrentLevel.StartLevel();
        } else {
            CurrentLevel.RestartLevel();
        }
        GameStateManager.Instance.CurrentGameState = GameState.Play;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
