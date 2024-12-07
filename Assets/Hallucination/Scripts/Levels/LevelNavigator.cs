using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelNavigator))]
public class LevelNavigatorInspector : Editor {
    int startLevelIndex;
    public override void OnInspectorGUI() {
        // LevelNavigator levelNavigator = (LevelNavigator)target;
        if (GUILayout.Button("Set first time play")) {
            LevelNavigator.SetFirstPlay(true);
        }
        startLevelIndex = PlayerPrefs.GetInt("StartLevelIndex", 0);
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
    bool firstLoad;

    public LevelController CurrentLevel => levels[currentLevelIndex];

    public static void SetStartLevelIndex(int levelIndex) {
        PlayerPrefs.SetInt("StartLevelIndex", levelIndex);
    }

    public static void SetFirstPlay(bool isFirstPlay) {
        PlayerPrefs.SetInt("FirstPlay", isFirstPlay ? 1 : 0);
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
        firstLoad = PlayerPrefs.GetInt("FirstPlay", 1) == 1;
        foreach (var level in levels) {
            level.gameObject.SetActive(false);
        }
        if (currentLevelIndex >= levels.Length) {
            Debug.LogWarning("currentLevelIndex is out of range. Set to last level.");
            currentLevelIndex = 0;
            PlayerPrefs.SetInt("StartLevelIndex", currentLevelIndex);
        }

        CurrentLevel.gameObject.SetActive(true);
        GameStateManager.Instance.CurrentGameState = GameState.Play;
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
        if (firstLoad) {
            firstLoad = false;
            PlayerPrefs.SetInt("FirstPlay", 0);
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
    }

    public void RestartCurrentLevel() {
        StartCoroutine(PerformSceneRestart());
    }

    IEnumerator PerformSceneRestart() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        asyncLoad.allowSceneActivation = false;
        yield return Util.FadeInCanvasGroup(1f, WorldSwitchManager.Instance.FadingMask);
        asyncLoad.allowSceneActivation = true;
    }

    public void CompleteCurrentLevel() {
        Debug.Log("Complete Level" + CurrentLevel.gameObject.name);
        CurrentLevel.CompleteLevel();
        currentLevelIndex += 1;
        PlayerPrefs.SetInt("StartLevelIndex", currentLevelIndex);
        if (currentLevelIndex == levels.Length) {
            Debug.LogWarning("The last level completed");
        } else {
            Debug.Log($"New Level: {CurrentLevel.gameObject.name}");
            CurrentLevel.gameObject.SetActive(true);
            CurrentLevel.StartLevel();
        }
    }
}
