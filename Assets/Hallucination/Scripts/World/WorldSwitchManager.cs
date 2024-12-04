using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WorldSwitchManager : MonoBehaviour {
    public static WorldSwitchManager Instance { get; private set; }
    [SerializeField] GameObject WorldSwitchIcon;
    public GameObject Bear, Fairy;
    GameObject[] fairyWorldEnvironments;
    GameObject[] bearWorldEnvironments;
    public CanvasGroup FadingMask;
    public UnityEvent WorldSwitching;
    public UnityEvent WorldSwitched;

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
        fairyWorldEnvironments = GameObject.FindGameObjectsWithTag("FairyWorldEnvironment");
        bearWorldEnvironments = GameObject.FindGameObjectsWithTag("BearWorldEnvironment");
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

    public GameObject CurrentPlayedCharacter => currentWorld == CharacterTypeEnum.Bear ? Bear : Fairy;

    /// <summary>
    /// Locks the world switch.
    /// This is used in scenarios where the same GameObject cannot lock and unlock the world switch,
    /// such as checkpoints.
    /// </summary>
    public void LockWorldSwitch() { Lock(gameObject); }

    /// <summary>
    /// Unlocks the world switch.
    /// This is used in scenarios where the same GameObject cannot lock and unlock the world switch,
    /// such as checkpoints.
    /// </summary>
    public void UnlockWorldSwitch() { Unlock(gameObject); }

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

    /// <summary>
    /// <remarks>Force</remarks> switches the world with a fade effect.
    /// </summary>
    /// <param name="world"></param>
    private void SwitchToWorldWithFade(CharacterTypeEnum world) {
        if (world == CharacterTypeEnum.None || currentWorld == world) return;
        StartCoroutine(PerformSwitchToWorldWithFade(world));
    }

    private void SwitchWorldByInput(InputAction.CallbackContext context) {
        SwitchWorldWithFade();
    }

    public void SwitchToWorld(CharacterTypeEnum world) {
        if (world == CharacterTypeEnum.None || currentWorld == world || locks.Count != 0) return;

        WorldSwitching?.Invoke();
        WorldSwitched?.Invoke();
        if (world == CharacterTypeEnum.Bear) {
            SetWorldBear();
        } else if (world == CharacterTypeEnum.Fairy) {
            SetWorldFairy();
        } else {
            Debug.LogError($"Unexpected {nameof(world)} value: {world}");
        }
    }

    public void ForceSwitchToWorldWithFade(CharacterTypeEnum world) {
        if (locks.Count != 0) {
            Debug.LogWarning("Lock count is not 0. Automatically release all locks.");
            locks.Clear();
        }
        StartCoroutine(PerformSwitchToWorldWithFade(world));
    }

    IEnumerator PerformSwitchToWorldWithFade(CharacterTypeEnum world) {
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.SwitchingWorld;
        WorldSwitching?.Invoke();
        yield return StartCoroutine(Util.FadeOut(0.4f, FadingMask));

        WorldSwitched?.Invoke();
        if (world == CharacterTypeEnum.Bear) {
            SetWorldBear();
        } else if (world == CharacterTypeEnum.Fairy) {
            SetWorldFairy();
        } else {
            Debug.LogError($"Unexpected {nameof(world)} value: {world}");
        }
        yield return StartCoroutine(Util.FadeIn(0.4f, FadingMask));
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
    }

    void SetWorldFairy() {
        currentWorld = CharacterTypeEnum.Fairy;
        foreach (var environment in fairyWorldEnvironments) { environment.SetActive(true); }
        foreach (var environment in bearWorldEnvironments) { environment.SetActive(false); }
    }

    void SetWorldBear() {
        currentWorld = CharacterTypeEnum.Bear;
        foreach (var environment in fairyWorldEnvironments) { environment.SetActive(false); }
        foreach (var environment in bearWorldEnvironments) { environment.SetActive(true); }
    }
}
