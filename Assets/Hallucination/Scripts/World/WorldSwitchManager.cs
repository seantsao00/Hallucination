using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class WorldSwitchManager : MonoBehaviour {
    public static WorldSwitchManager Instance { get; private set; }
    [SerializeField] GameObject WorldSwitchIcon;
    public GameObject Bear, Fairy;
    GameObject[] fairyWorldEnvironments;
    GameObject[] bearWorldEnvironments;
    public CanvasGroup FadingMask;
    public UnityEvent WorldStartSwitching;
    public UnityEvent WorldSwitching;
    public UnityEvent WorldSwitched;
    [SerializeField] Sprite worldSwitchIconInFairyWorld;
    [SerializeField] Sprite worldSwitchIconInBearWorld;
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

    public void LogLocks() {
        Debug.Log($"{locks.Count} Locks: {string.Join(", ", locks)}");
    }

    public void UpdateWorldSwitchIcon() => WorldSwitchIcon.SetActive(
        locks.Count == 0 && InputManager.Control.World.SwitchWorld.enabled
    );

    public void SwitchWorld(bool force = false, bool withFade = false) {
        if (currentWorld == CharacterTypeEnum.Bear) {
            SwitchToWorld(CharacterTypeEnum.Fairy, force, withFade);
        } else if (currentWorld == CharacterTypeEnum.Fairy) {
            SwitchToWorld(CharacterTypeEnum.Bear, force, withFade);
        } else {
            Debug.LogError($"Unexpected {nameof(currentWorld)} value: {currentWorld}");
        }
    }

    public void SwitchToWorld(CharacterTypeEnum targetWorld, bool force = false, bool withFade = false) {
        if (!force && locks.Count != 0) return;
        if (force) ClearLocks();

        WorldStartSwitching?.Invoke();
        if (withFade) {
            StartCoroutine(PerformSwitchToWorldWithFade(targetWorld));
        } else {
            WorldSwitching?.Invoke();
            if (targetWorld == CharacterTypeEnum.Bear) {
                SetWorldBear();
            } else if (targetWorld == CharacterTypeEnum.Fairy) {
                SetWorldFairy();
            } else {
                Debug.LogError($"Unexpected {nameof(targetWorld)} value: {targetWorld}");
            }
            WorldSwitched?.Invoke();
        }
    }

    private void SwitchWorldByInput(InputAction.CallbackContext context) {
        SwitchWorld(withFade: true);
    }

    IEnumerator PerformSwitchToWorldWithFade(CharacterTypeEnum world) {
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.SwitchingWorld;
        yield return StartCoroutine(Util.FadeInCanvasGroup(0.4f, FadingMask));

        WorldSwitching?.Invoke();

        if (world == CharacterTypeEnum.Bear) {
            SetWorldBear();
        } else if (world == CharacterTypeEnum.Fairy) {
            SetWorldFairy();
        } else {
            Debug.LogError($"Unexpected {nameof(world)} value: {world}");
        }

        WorldSwitched?.Invoke();

        yield return StartCoroutine(Util.FadeOutCanvasGroup(0.4f, FadingMask));
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
    }

    private void SetWorldFairy() {
        WorldSwitchIcon.GetComponent<Image>().sprite = worldSwitchIconInFairyWorld;
        currentWorld = CharacterTypeEnum.Fairy;
        foreach (var environment in fairyWorldEnvironments) { environment.SetActive(true); }
        foreach (var environment in bearWorldEnvironments) { environment.SetActive(false); }
    }

    private void SetWorldBear() {
        WorldSwitchIcon.GetComponent<Image>().sprite = worldSwitchIconInBearWorld;
        currentWorld = CharacterTypeEnum.Bear;
        foreach (var environment in fairyWorldEnvironments) { environment.SetActive(false); }
        foreach (var environment in bearWorldEnvironments) { environment.SetActive(true); }
    }
}
