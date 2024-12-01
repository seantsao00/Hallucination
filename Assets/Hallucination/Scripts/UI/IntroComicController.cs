using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class IntroComicController : MonoBehaviour {
    [SerializeField] CanvasGroup NextTip;
    string levelSceneName = "Levels";
    [SerializeField] LoadingScreen loadingScreen;
    [Tooltip("The video path relative to StreamingAssets folder.")]
    [SerializeField] string videoName = "intro_converted.mp4";
    CanvasGroup canvasGroup;
    VideoPlayer videoPlayer;
    AsyncOperation loadingOperation;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        videoPlayer = GetComponent<VideoPlayer>();
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        NextTip.gameObject.SetActive(true);
        NextTip.alpha = 0f;
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);
        videoPlayer.loopPointReached += ComicEndHandler;
        videoPlayer.Play();
        GameStateManager.Init();
        InputManager.Init();
        GameStateManager.Instance.CurrentGameState = GameState.Animation;
        StartCoroutine(StartLoadLevels());
    }

    IEnumerator StartLoadLevels() {
        loadingOperation = SceneManager.LoadSceneAsync(levelSceneName);
        loadingOperation.allowSceneActivation = false;
        yield return null;
    }

    void ComicEndHandler(VideoPlayer vp) {
        InputManager.Control.Animation.Confirm.performed += ConfirmAction;
        StartCoroutine(Util.FadeInCanvasGroup(NextTip, 1f));
    }

    void ConfirmAction(InputAction.CallbackContext ctx) {
        loadingScreen.ShowLoadingScreen();
        loadingOperation.allowSceneActivation = true;
    }

    void OnDestroy() {
        InputManager.Control.Animation.Confirm.performed -= ConfirmAction;
    }
}
