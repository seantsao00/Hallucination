using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem;

public enum World { Fairy, Bear };

public class WorldSwitchManager : MonoBehaviour {
    public static WorldSwitchManager Instance { get; private set; }
    public GameObject WorldFairyEnvironment;
    public GameObject WorldBearEnvironment;
    public CanvasGroup FadeCanvasGroup;
    public UnityEvent OnWorldSwitch;

    private bool isInWorldFairy = true;
    private bool disabled = false;
    public World currentWorld { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("WorldSwitchManager: " +
            "Duplicate instance detected and removed. Only one instance of WorldSwitchManager is allowed.");
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    void OnEnable() {
        InputManager.Control.World.SwitchWorld.performed += SwitchWorldByInput;
    }
    void OnDisable() {
        InputManager.Control.World.SwitchWorld.performed -= SwitchWorldByInput;
    }

    void Start() {
        ActivateWorldFairy();
    }

    void SwitchWorldByInput(InputAction.CallbackContext context) {
        SwitchWorld();
    }

    public void SwitchWorld() {
        // TODO: refactor this.
        if (disabled) return;
        StartCoroutine(SwitchWorldsWithFade());
    }

    IEnumerator SwitchWorldsWithFade() {
        yield return StartCoroutine(FadeOut());

        OnWorldSwitch?.Invoke();
        isInWorldFairy = !isInWorldFairy;
        if (isInWorldFairy) {
            ActivateWorldFairy();
        } else {
            ActivateWorldBear();
        }

        yield return StartCoroutine(FadeIn());
    }

    void ActivateWorldFairy() {
        currentWorld = World.Fairy;
        WorldFairyEnvironment.SetActive(true);
        WorldBearEnvironment.SetActive(false);
    }

    void ActivateWorldBear() {
        currentWorld = World.Bear;
        WorldFairyEnvironment.SetActive(false);
        WorldBearEnvironment.SetActive(true);
    }

    public void Enable() {
        disabled = false;
        print(disabled);
    }

    public void Disable() {
        disabled = true;
        print(disabled);
    }

    IEnumerator FadeOut() {
        float fadeDuration = 0.4f;
        float fadeSpeed = 1f / fadeDuration;

        for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed) {
            FadeCanvasGroup.alpha = t;
            yield return null;
        }
        FadeCanvasGroup.alpha = 1;
    }

    IEnumerator FadeIn() {
        float fadeDuration = 0.4f;
        float fadeSpeed = 1f / fadeDuration;

        for (float t = 1; t > 0; t -= Time.deltaTime * fadeSpeed) {
            FadeCanvasGroup.alpha = t;
            yield return null;
        }
        FadeCanvasGroup.alpha = 0;
    }

    void OnDrawGizmos() {
        Awake();
    }
}
