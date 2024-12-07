using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public LoadingScreen loadingScreen;
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        loadingScreen.ShowLoadingScreen();
        StartCoroutine(Util.InitializeLocalizationAsync(loadingScreen));
    }
    

    public void PlayGame() {
        SceneManager.LoadSceneAsync("Intro");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
