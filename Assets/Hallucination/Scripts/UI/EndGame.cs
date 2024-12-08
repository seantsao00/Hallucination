using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndGame : MonoBehaviour {
    [SerializeField] CanvasGroup lesson;
    [SerializeField] LineByLineTextPrinter lessonText;
    [SerializeField] CanvasGroup cover;
    [SerializeField] CanvasGroup thankYou;
    [SerializeField] CanvasGroup continueTip;
    string mainMenu = "MainMenu";

    void Start() {
        GameStateManager.Init();
        InputManager.Init();
        GameStateManager.Instance.CurrentGameState = GameState.Animation;

        cover.alpha = 0;
        thankYou.alpha = 0;
        continueTip.alpha = 0;

        lessonText.StartPrintLines();
        StartCoroutine(EndGameCoroutine());
    }

    IEnumerator EndGameCoroutine() {
        yield return new WaitUntil(() => !lessonText.isPrinting);
        PlayerPrefs.SetInt("StartLevelIndex", 0);
        PlayerPrefs.SetInt("isFirstPlay", 1);
        PlayerPrefs.SetInt("IntroWatched", 0);
        PlayerPrefs.Save();
        StartCoroutine(Util.FadeInCanvasGroup(3, cover));
        yield return StartCoroutine(Util.FadeOutCanvasGroup(3, lesson));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(Util.FadeInCanvasGroup(1, thankYou));
        yield return new WaitForSeconds(2);
        InputManager.Control.Animation.Confirm.performed += ConfirmAction;
        yield return StartCoroutine(Util.FadeInCanvasGroup(1, continueTip));
    }

    void ConfirmAction(InputAction.CallbackContext ctx) {
        SceneManager.LoadScene(mainMenu);
    }

    void OnDestroy() {
        InputManager.Control.Animation.Confirm.performed -= ConfirmAction;
    }
}
