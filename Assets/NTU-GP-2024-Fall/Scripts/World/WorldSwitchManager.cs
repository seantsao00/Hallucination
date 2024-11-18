using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WorldSwitchManager : MonoBehaviour {
    public static WorldSwitchManager Instance { get; private set; }
    [SerializeField] GameObject WorldSwitchIcon;
    // [SerializeField] CharacterTypeEnum initialWorld;
    public GameObject[] WorldFairyEnvironment;
    public GameObject[] WorldBearEnvironment;
    [SerializeField] CanvasGroup FadeCanvasGroup;
    public UnityEvent OnWorldSwitch;

    public CharacterTypeEnum currentWorld { get; private set; }

    /// <summary>
    /// A collection of locks that restrict the world switch operation.
    /// 
    /// Usage: <code>WorldSwitchManager.Instance.Locks.Add(gameObject);</code>
    /// If this collection contains any entries, the player will be unable to perform a world switch.
    /// </summary>
    readonly HashSet<GameObject> locks = new();

    void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning($"{typeof(WorldSwitchManager)}: " +
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

    public void ClearLocks() {
        locks.Clear();
        UpdateWorldSwitchIcon();
    }

    public void UpdateWorldSwitchIcon() => WorldSwitchIcon.SetActive(
        locks.Count == 0 && InputManager.Control.World.SwitchWorld.enabled
    );

    public void SwitchWorldWithFade() {
        if (locks.Count != 0) return;

        ForceSwitchWorldWithFade();
    }

    public void ForceSwitchWorldWithFade() {
        if (locks.Count != 0) {
            Debug.LogWarning("Lock count is not 0. Automatically release all locks.");
            locks.Clear();
        }
        if (currentWorld == CharacterTypeEnum.Bear) {
            SwitchToWorldWithFade(CharacterTypeEnum.Fairy);
        } else if (currentWorld == CharacterTypeEnum.Fairy) {
            SwitchToWorldWithFade(CharacterTypeEnum.Bear);
        } else {
            Debug.LogError($"Unexpected {nameof(currentWorld)} value: {currentWorld}");
        }
    }

    public void SwitchToWorldWithFade(CharacterTypeEnum world) {
        if (world == CharacterTypeEnum.None || currentWorld == world) return;
        StartCoroutine(PerformSwitchToWorldWithFade(world));
    }

    void SwitchWorldByInput(InputAction.CallbackContext context) {
        SwitchWorldWithFade();
    }

    public void SwitchToWorld(CharacterTypeEnum world) {
        if (world == CharacterTypeEnum.None || currentWorld == world || locks.Count != 0) return;

        OnWorldSwitch?.Invoke();
        if (world == CharacterTypeEnum.Bear) {
            SetWorldBear();
        } else if (world == CharacterTypeEnum.Fairy) {
            SetWorldFairy();
        } else {
            Debug.LogError($"Unexpected {nameof(world)} value: {world}");
        }
    }

    IEnumerator PerformSwitchToWorldWithFade(CharacterTypeEnum world) {
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.SwitchingWorld;
        yield return StartCoroutine(FadeOut());

        OnWorldSwitch?.Invoke();
        if (world == CharacterTypeEnum.Bear) {
            SetWorldBear();
        } else if (world == CharacterTypeEnum.Fairy) {
            SetWorldFairy();
        } else {
            Debug.LogError($"Unexpected {nameof(world)} value: {world}");
        }
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
        yield return StartCoroutine(FadeIn());
    }

    void SetWorldFairy() {
        currentWorld = CharacterTypeEnum.Fairy;
        foreach (var environment in WorldFairyEnvironment) { environment.SetActive(true); }
        foreach (var environment in WorldBearEnvironment) { environment.SetActive(false); }
    }

    void SetWorldBear() {
        currentWorld = CharacterTypeEnum.Bear;
        foreach (var environment in WorldFairyEnvironment) { environment.SetActive(false); }
        foreach (var environment in WorldBearEnvironment) { environment.SetActive(true); }
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
}
