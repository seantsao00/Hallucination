using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum World { Fairy, Bear };

public class WorldSwitchManager : MonoBehaviour {
    [SerializeField] GameObject WorldSwitchIcon;
    public static WorldSwitchManager Instance { get; private set; }
    public GameObject[] WorldFairyEnvironment;
    public GameObject[] WorldBearEnvironment;
    public CanvasGroup FadeCanvasGroup;
    public UnityEvent OnWorldSwitch;

    private bool isInWorldFairy = true;
    private bool disabled = false;
    public World currentWorld { get; private set; }

    /// <summary>
    /// A collection of locks that restrict the world switch operation.
    /// 
    /// Usage: <code>WorldSwitchManager.Instance.Locks.Add(gameObject);</code>
    /// If this collection contains any entries, the player will be unable to perform a world switch.
    /// </summary>
    HashSet<GameObject> locks = new HashSet<GameObject>();
    void UpdateWorldSwitchIcon() => WorldSwitchIcon.SetActive(locks.Count == 0);
    public bool Lock(GameObject gameObject) {
        bool success = locks.Add(gameObject);
        UpdateWorldSwitchIcon();
        return success;
    }
    public bool Unlock(GameObject gameObject) {
        bool success = locks.Remove(gameObject);
        UpdateWorldSwitchIcon();
        return success;
    }


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
        if (disabled) return;
        if (locks.Count != 0) return;
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
        foreach (var environment in WorldFairyEnvironment) { environment.SetActive(true); }
        foreach (var environment in WorldBearEnvironment) { environment.SetActive(false); }
    }

    void ActivateWorldBear() {
        currentWorld = World.Bear;
        foreach (var environment in WorldFairyEnvironment) { environment.SetActive(false); }
        foreach (var environment in WorldBearEnvironment) { environment.SetActive(true); }
    }

    public void Enable() {
        disabled = false;
    }

    public void Disable() {
        disabled = true;
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
