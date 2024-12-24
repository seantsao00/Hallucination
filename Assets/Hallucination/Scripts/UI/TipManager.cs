using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class TipManager : MonoBehaviour {
    [SerializeField] GameObject bottomTip;
    [SerializeField] CanvasGroup switchWorldTip;
    [SerializeField] CanvasGroup projectionTip;
    [SerializeField] CanvasGroup interactableTip;
    public static TipManager Instance { get; private set; }

    readonly HashSet<GameObject> interactableObjects = new();
    public void ShowInteractableObjectTip(GameObject gameObject) {
        bool success = interactableObjects.Add(gameObject);
        HandleInteractableTip();
    }
    public void CloseInteractableObjectTip(GameObject gameObject) {
        bool success = interactableObjects.Remove(gameObject);
        HandleInteractableTip();
    }

    void HandleInteractableTip() {
        if (interactableObjects.Count == 0) {
            interactableTip.gameObject.SetActive(false);
        } else {
            interactableTip.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start() {
        bottomTip.SetActive(false);
        switchWorldTip.gameObject.SetActive(false);
        projectionTip.gameObject.SetActive(false);
        HandleInteractableTip();
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
        switchWorldTip.gameObject.SetActive(false);
        projectionTip.gameObject.SetActive(false);
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
    }

    public void ShowSwitchWorldTip() {
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.AllInputDisabled;
        switchWorldTip.gameObject.SetActive(true);
        StartCoroutine(FadeInFallScreenTip(switchWorldTip, 1f));
    }

    public void ShowProjectionTip() {
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.AllInputDisabled;
        projectionTip.gameObject.SetActive(true);
        StartCoroutine(FadeInFallScreenTip(projectionTip, 1f));
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

    IEnumerator FadeInFallScreenTip(CanvasGroup canvasGroup, float fadeDuration) {
        canvasGroup.alpha = 0;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.FullScreenTip;
    }
}

