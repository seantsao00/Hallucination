using UnityEngine;
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
        if (PlayerPrefs.GetInt("IntroWatched", 0) == 0) SceneManager.LoadSceneAsync("Intro");
        else SceneManager.LoadScene("Levels");
    }

    public void ClearAllData() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void QuitGame() {
        Application.Quit();
    }
}
