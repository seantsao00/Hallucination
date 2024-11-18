using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VideoSceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer; // 拖入 VideoPlayer
    public Button nextSceneButton; // 拖入按鈕
    public string nextSceneName; // 下一個場景的名字

    void Start()
    {
        // 確保按鈕一開始是隱藏的
        nextSceneButton.gameObject.SetActive(false);

        // 監聽影片結束事件
        videoPlayer.loopPointReached += OnVideoEnd;
        
        // 設置按鈕事件
        nextSceneButton.onClick.AddListener(GoToNextScene);
    }

    // 當影片結束時觸發
    void OnVideoEnd(VideoPlayer vp)
    {
        nextSceneButton.gameObject.SetActive(true); // 顯示按鈕
    }

    // 切換到下一個場景
    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
