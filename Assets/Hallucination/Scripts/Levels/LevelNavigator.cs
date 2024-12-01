using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelNavigator))]
public class LevelNavigatorInspector : Editor {
    int startLevelIndex;
    public override void OnInspectorGUI() {
        startLevelIndex = PlayerPrefs.GetInt("StartLevelIndex", 0);
        LevelNavigator levelNavigator = (LevelNavigator)target;
        int newStartLevelIndex = EditorGUILayout.IntField("Start Level Index", startLevelIndex);
        if (newStartLevelIndex != startLevelIndex) {
            startLevelIndex = newStartLevelIndex;
            LevelNavigator.SetStartLevelIndex(startLevelIndex);
        }
        base.OnInspectorGUI();
    }
}
#endif

public class LevelNavigator : MonoBehaviour {
    public static LevelNavigator Instance { get; private set; }
    [SerializeField] LevelController[] levels;
    int currentLevelIndex;
    bool firstLoad = true;

    public LevelController CurrentLevel => levels[currentLevelIndex];

    public static void SetStartLevelIndex(int levelIndex) {
        PlayerPrefs.SetInt("StartLevelIndex", levelIndex);
    }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            Debug.LogWarning($"{nameof(LevelNavigator)}: " +
            $"Duplicate instance detected and removed. Only one instance of {nameof(LevelNavigator)} is allowed.");
            Destroy(Instance);
            return;
        }
        Instance = this;
        currentLevelIndex = PlayerPrefs.GetInt("StartLevelIndex", 0);
        if (currentLevelIndex >= levels.Length) {
            Debug.LogWarning("currentLevelIndex is out of range. Set to last level.");
            currentLevelIndex = levels.Length - 1;
            PlayerPrefs.SetInt("StartLevelIndex", currentLevelIndex);
        }
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void RestartCurrentLevel() {
        StartCoroutine(PerformSceneRestart());
    }

    IEnumerator PerformSceneRestart() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        asyncLoad.allowSceneActivation = false;
        yield return Util.FadeOut(1f, WorldSwitchManager.Instance.FadingMask);
        asyncLoad.allowSceneActivation = true;
    }

    public void CompleteCurrentLevel() {
        currentLevelIndex += 1;
        PlayerPrefs.SetInt("StartLevelIndex", currentLevelIndex);
        if (currentLevelIndex == levels.Length) {
            Debug.LogWarning("The last level completed");
        } else {
            Debug.Log($"New Level: {CurrentLevel.gameObject.name}");
            CurrentLevel.StartLevel();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (firstLoad) {
            firstLoad = false;
            if (CurrentLevel.CanBeStartLevel) {
                CurrentLevel.StartLevel();
            } else {
                Debug.LogWarning(
                    $"Start the game from a level {CurrentLevel} that does not set start world." +
                    "Automatically invoke restart."
                );
                CurrentLevel.RestartLevel();
            }
        } else {
            CurrentLevel.RestartLevel();
        }
        GameStateManager.Instance.CurrentGameState = GameState.Play;
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
