using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    void Start() {
        StartCoroutine(Util.InitializeLocalizationAsync());
    }
    

    public void PlayGame() {
        SceneManager.LoadSceneAsync("Intro");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
