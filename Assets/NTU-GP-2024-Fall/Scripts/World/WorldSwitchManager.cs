using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WorldSwitchManager : MonoBehaviour {
    [SerializeField] GameObject WorldSwitchIcon;
    // [SerializeField] CharacterTypeEnum initialWorld;
    public static WorldSwitchManager Instance { get; private set; }
    public GameObject[] WorldFairyEnvironment;
    public GameObject[] WorldBearEnvironment;
    public CanvasGroup FadeCanvasGroup;
    public UnityEvent OnWorldSwitch;

    public CharacterTypeEnum currentWorld { get; private set; }

    /// <summary>
    /// A collection of locks that restrict the world switch operation.
    /// 
    /// Usage: <code>WorldSwitchManager.Instance.Locks.Add(gameObject);</code>
    /// If this collection contains any entries, the player will be unable to perform a world switch.
    /// </summary>
    HashSet<GameObject> locks = new HashSet<GameObject>();
    public void UpdateWorldSwitchIcon() => WorldSwitchIcon.SetActive(
        locks.Count == 0 && InputManager.Control.World.SwitchWorld.enabled
    );
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

    void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning($"{typeof(WorldSwitchManager)}: " +
            "Duplicate instance detected and removed. Only one instance of WorldSwitchManager is allowed.");
            Destroy(Instance);
            return;
        }
        Instance = this;
        SetWorldBear();
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

    void SwitchWorldByInput(InputAction.CallbackContext context) {
        SwitchWorld();
    }

    public void SwitchWorld() {
        if (locks.Count != 0) return;
        StartCoroutine(SwitchWorldsWithFade());
    }

    public void ForceSwitchWorld() {
        if (locks.Count != 0) {
            Debug.LogWarning("Lock count is not 0. Automatically release all locks.");
            locks.Clear();
        }
        StartCoroutine(SwitchWorldsWithFade());
    }

    IEnumerator SwitchWorldsWithFade(CharacterTypeEnum world) {
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

    IEnumerator SwitchWorldsWithFade() {
        CharacterTypeEnum targetWorld = currentWorld == CharacterTypeEnum.Bear ? CharacterTypeEnum.Fairy : CharacterTypeEnum.Bear;
        StartCoroutine(SwitchWorldsWithFade(targetWorld));
        yield return null;
    }

    public void SetWorldFairy() {
        currentWorld = CharacterTypeEnum.Fairy;
        foreach (var environment in WorldFairyEnvironment) { environment.SetActive(true); }
        foreach (var environment in WorldBearEnvironment) { environment.SetActive(false); }
    }

    public void SetWorldBear() {
        currentWorld = CharacterTypeEnum.Bear;
        foreach (var environment in WorldFairyEnvironment) { environment.SetActive(false); }
        foreach (var environment in WorldBearEnvironment) { environment.SetActive(true); }
    }

    public void SwitchToWorld(CharacterTypeEnum world) {
        if (world == CharacterTypeEnum.None || currentWorld == world) return;
        StartCoroutine(SwitchWorldsWithFade(world));
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
