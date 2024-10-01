using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the game scene
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        // Exits the application
        Application.Quit();
    }
    /*
    public void OpenSettings()
    {
        // Logic to open settings menu (e.g., activate a settings panel)
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
    */
}

