using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour {
    public CanvasGroup fadeCanvasGroup; // Reference to the CanvasGroup component
    public float fadeDuration;     // Time to complete fade

    private void Start() {
        // Ensure the panel starts fully transparent and doesn't block clicks
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    public void FadeOutAndSwitchScene(string sceneName) {
        StartCoroutine(FadeOutAnimation(sceneName));
    }

    private IEnumerator FadeOutAnimation(string sceneName) {
        // Enable blocking of raycasts during the fade
        fadeCanvasGroup.blocksRaycasts = true;

        // Fade out over time
        float timer = 0f;
        while (timer <= fadeDuration) {
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;  // Wait until the next frame
        }

        // Ensure the alpha is fully set to 1
        fadeCanvasGroup.alpha = 1f;
        SceneManager.LoadSceneAsync(sceneName);
    }
}
