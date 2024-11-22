using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class IntroComicController : MonoBehaviour {
    [SerializeField] CanvasGroup NextTip;
    [SerializeField] string nextSceneName = "SampleScene";
    CanvasGroup canvasGroup;
    VideoPlayer videoPlayer;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        videoPlayer = GetComponent<VideoPlayer>();
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.alpha = 1;
        NextTip.alpha = 0;
        videoPlayer.loopPointReached += ComicEndHandler;
        GameStateManager.Init();
        InputManager.Init();
        GameStateManager.Instance.CurrentGameState = GameState.Animation;
    }

    void ComicEndHandler(VideoPlayer vp) {
        StartCoroutine(Util.FadeInCanvasGroup(NextTip, 1f, () => {
            InputManager.Control.Animation.Confirm.performed += ConfirmAction;
        }));
    }

    void OnDestroy() {
        InputManager.Control.Animation.Confirm.performed -= ConfirmAction;
    }

    void ConfirmAction(InputAction.CallbackContext ctx) {
        SceneManager.LoadScene(nextSceneName);
    }
}
