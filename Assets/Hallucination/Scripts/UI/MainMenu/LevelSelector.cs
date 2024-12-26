using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class levelSelector : MonoBehaviour
{
    [SerializeField] GameObject firstLevelButton;
    void Awake() {
        gameObject.SetActive(false);
    }

    void OnEnable() {
        EventSystem.current.SetSelectedGameObject(firstLevelButton);
        
    }
    public void SelectLevel(int levelIndex) {
        LevelNavigator.SetFirstPlay(true);
        LevelNavigator.SetStartLevelIndex(levelIndex);
        if (PlayerPrefs.GetInt("IntroWatched", 0) == 0) SceneManager.LoadSceneAsync("Intro");
        else SceneManager.LoadScene("Levels");
    }
}
