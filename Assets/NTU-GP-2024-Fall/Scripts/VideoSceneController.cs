using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VideoSceneController : MonoBehaviour {
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] Button nextSceneButton;
    [SerializeField] string nextSceneName;

    void Start() {
        canvasGroup.alpha = 1;
        nextSceneButton.gameObject.SetActive(false);

        // 監聽影片結束事件
        videoPlayer.loopPointReached += OnVideoEnd;

        // 設置按鈕事件
        nextSceneButton.onClick.AddListener(GoToNextScene);
    }

    // 當影片結束時觸發
    void OnVideoEnd(VideoPlayer vp) {
        nextSceneButton.gameObject.SetActive(true); // 顯示按鈕
    }

    // 切換到下一個場景
    void GoToNextScene() {
        SceneManager.LoadScene(nextSceneName);
    }
}
