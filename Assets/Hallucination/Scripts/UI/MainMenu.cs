using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    AsyncOperation loadingOperation;
    void Start() {
        loadingOperation = SceneManager.LoadSceneAsync("Intro");
        loadingOperation.allowSceneActivation = false;
    }

    public void PlayGame() {
        loadingOperation.allowSceneActivation = true;
    }

    public void QuitGame() {
        Application.Quit();
    }
}
