using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TipManager : MonoBehaviour {
    [SerializeField] GameObject bottomTip;
    [SerializeField] CanvasGroup SwitchWorldTip;
    public static TipManager Instance { get; private set; }

    // Start is called before the first frame update
    void Start() {
        bottomTip.SetActive(false);
        SwitchWorldTip.gameObject.SetActive(false);
    }
    void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning($"{typeof(TipManager)}: " +
            $"Duplicate instance detected and removed. Only one instance of {typeof(TipManager)} is allowed.");
            Destroy(Instance);
            return;
        }
        Instance = this;
        InputManager.Control.Tip.Confirm.performed += Instance.HandleConfirm;
    }

    private void OnDestroy() {
        if (Instance == this) {
            InputManager.Control.Tip.Confirm.performed -= Instance.HandleConfirm;
            Instance = null;
        }
    }

    void HandleConfirm(InputAction.CallbackContext context) {
        SwitchWorldTip.gameObject.SetActive(false);
        GameStateManager.Instance.CurrentGamePalyState = GamePlayState.Normal;
    }

    public void ShowSwitchWorldTip() {
        GameStateManager.Instance.CurrentGamePalyState = GamePlayState.AllInputDisabled;
        SwitchWorldTip.gameObject.SetActive(true);
        StartCoroutine(FadeIn(SwitchWorldTip, 1f));
    }

    public void ShowTip(bool show, string tipText = "") {
        bottomTip.SetActive(show);
        TextMeshProUGUI tipTextComponent = bottomTip.GetComponentInChildren<TextMeshProUGUI>();
        if (tipTextComponent != null) tipTextComponent.enabled = show;
        if (show) {
            if (tipTextComponent != null) {
                tipTextComponent.text = tipText;
            } else {
                Debug.LogError("No Text component found in bottomTip GameObject.");
            }
        }
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup, float fadeDuration) {
        canvasGroup.alpha = 0;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        GameStateManager.Instance.CurrentGamePalyState = GamePlayState.FullScreenTip;
    }
}

