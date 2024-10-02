using UnityEngine;
using UnityEngine.SceneManagement; // For SceneManager

public class MainMenu : MonoBehaviour
{
    public SceneFader sceneFader;  // Reference to the SceneFader script

    public void PlayGame()
    {
        // Use the SceneFader to fade and load the game scene
        sceneFader.FadeOutAndSwitchScene("SampleScene");
    }

    public void QuitGame()
    {
        // Exits the application
        Application.Quit();
    }
}


