using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class LevelController : MonoBehaviour {
    [System.Serializable]
    public class CheckpointData {
        public LevelCheckpoint Checkpoint;
        public CharacterTypeEnum WorldToSwitch;
        public GameObject FairySpawnPoint, BearSpawnPoint;
        public bool LockWorldSwitch;
        public bool UnlockWorldSwitch = true;
        public string DialogueName;
        public UnityEvent DialogueEndedEvent;
    }
    [System.Serializable]
    public class CharacterSyncMethod {
        public Syncable fairyWorldFairy;
        public Syncable fairyWorldBear;
        public Syncable bearWorldFairy;
        public Syncable bearWorldBear;
    }
    [SerializeField] float deactivateAfterCompleteDelay = 0f;
    [SerializeField] GameObject fairyWorldFairy, fairyWorldBear, bearWorldFairy, bearWorldBear;
    [SerializeField] CharacterSyncMethod characterSyncMethod;
    [SerializeField] CheckpointData startData;
    // For debugging (directly start the game with any level)
    public bool CanBeStartLevel => startData.WorldToSwitch != CharacterTypeEnum.None;
    [SerializeField]
    CheckpointData restartData = new CheckpointData {
        WorldToSwitch = CharacterTypeEnum.Bear,
    };
    [SerializeField] CheckpointData[] checkpointDataList;
    public GameObject fairyWorldMainCamera {get; private set;}
    public GameObject bearWorldMainCamera {get; private set;}

    int numberOfFulfilledCheckpoints;

    void Awake() {
        if (restartData.WorldToSwitch == CharacterTypeEnum.None) {
            Debug.LogError("You must specify a world to restart.");
        }
        fairyWorldMainCamera = transform.Find("FairyWorld").Find("FairyLevelMainCamera").gameObject;
        bearWorldMainCamera = transform.Find("BearWorld").Find("BearLevelMainCamera").gameObject;
    }

    void ApplyCharacterSyncMethod(GameObject character, Syncable method) {
        if (character == null) {
            if (method != null) {
                Debug.LogWarning(
                    "Syncable method provided but no corresponding character was specified. Skipping application."
                );
            }
            return;
        }
        // StartCoroutine(PerformApplyCharacterSyncMethods(character, method));
        Destroy(character.GetComponent<Syncable>());
        if (method != null) {
            var syncable = character.AddComponent<Syncable>();
            Syncable.CopyData(method, syncable);
            // Debug.Log($"{syncable.syncedObject}, {syncable.currentWorldReference}, {syncable.syncedWorldReference}");
            syncable.enabled = true;
        }
    }

    IEnumerator PerformApplyCharacterSyncMethods(GameObject character, Syncable method) {
        Destroy(fairyWorldFairy.GetComponent<Syncable>());
        yield return null;

        if (method != null) {
            var syncable = character.AddComponent<Syncable>();
            Syncable.CopyData(method, syncable);
            // Debug.Log($"{syncable.syncedObject}, {syncable.currentWorldReference}, {syncable.syncedWorldReference}");
            syncable.enabled = true;
        }
    }

    void LoadCheckpointData(CheckpointData checkpointData) {
        if (checkpointData.WorldToSwitch != CharacterTypeEnum.None) {
            WorldSwitchManager.Instance.SwitchToWorld(force: true, targetWorld: checkpointData.WorldToSwitch);
        } else {
            if (WorldSwitchManager.Instance.currentWorld == CharacterTypeEnum.Bear) {
                bearWorldMainCamera.SetActive(true);
                fairyWorldMainCamera.SetActive(false);
            } else {
                bearWorldMainCamera.SetActive(false);
                fairyWorldMainCamera.SetActive(true);
            }
        }
        if (checkpointData.LockWorldSwitch) WorldSwitchManager.Instance.LockWorldSwitch();
        if (checkpointData.UnlockWorldSwitch) WorldSwitchManager.Instance.UnlockWorldSwitch();
        if (checkpointData.FairySpawnPoint != null) {
            fairyWorldFairy.transform.position = checkpointData.FairySpawnPoint.transform.position;
        }
        if (checkpointData.BearSpawnPoint != null) {
            bearWorldBear.transform.position = checkpointData.BearSpawnPoint.transform.position;
        }
        if (!string.IsNullOrEmpty(checkpointData.DialogueName)) {
            StartCoroutine(WaitForWorldSwitchingAndStartDialogue(
                checkpointData.DialogueName,
                checkpointData.DialogueEndedEvent
            ));
        } else {
            checkpointData.DialogueEndedEvent?.Invoke();
        }
    }

    IEnumerator WaitForWorldSwitchingAndStartDialogue(string dialogueName, UnityEvent dialogueEndedEvent) {
        yield return new WaitUntil(
            () => GameStateManager.Instance.CurrentGamePlayState != GamePlayState.SwitchingWorld
        );
        DialogueManager.Instance.StartDialogueWithCallback(
            dialogueName,
            () => dialogueEndedEvent?.Invoke()
        );
    }

    void RegisterHandler() {
        foreach (var checkpointData in checkpointDataList) {
            checkpointData.Checkpoint.CheckpointCompleted.AddListener(Action => {
                LoadCheckpointData(checkpointData);
            });
        }
    }

    void ApplyCharacterSyncMethods() {
        StartCoroutine(PerformApplyCharacterSyncMethods());
    }

    IEnumerator PerformApplyCharacterSyncMethods() {
        ApplyCharacterSyncMethod(fairyWorldFairy, characterSyncMethod.fairyWorldFairy);
        ApplyCharacterSyncMethod(bearWorldBear, characterSyncMethod.bearWorldBear);
        ApplyCharacterSyncMethod(fairyWorldBear, characterSyncMethod.fairyWorldBear);
        ApplyCharacterSyncMethod(bearWorldFairy, characterSyncMethod.bearWorldFairy);
        yield return null;
        fairyWorldFairy.GetComponent<Syncable>()?.SyncState();
        bearWorldBear.GetComponent<Syncable>()?.SyncState();
    }

    public void StartLevel() {
        Debug.Log($"Start Level: {gameObject.name}");
        if (startData.Checkpoint != null) {
            Debug.LogWarning(
                $"The {nameof(LevelCheckpoint)} assignment in start data is redundant and will have no effect."
            );
        }
        HandleLevelBegin(startData);
    }

    public void RestartLevel() {
        Debug.Log($"Restart Level: {gameObject.name}");
        if (restartData.Checkpoint != null) {
            Debug.LogWarning(
                $"The {nameof(LevelCheckpoint)} assignment in restart data is redundant and will have no effect."
            );
        }
        HandleLevelBegin(restartData, fadeIn: true);
    }

    void HandleLevelBegin(CheckpointData levelData, bool fadeIn = false) {
        if (levelData == null) {
            Debug.LogError("Level data is null.");
            return;
        }
        LoadCheckpointData(levelData);

        if (fadeIn) StartCoroutine(Util.FadeOutCanvasGroup(1f, WorldSwitchManager.Instance.FadingMask));

        ApplyCharacterSyncMethods();
        RegisterHandler();
    }

    public void CompleteLevel() {
        Assert.IsTrue(this == LevelNavigator.Instance.CurrentLevel);

        StartCoroutine(PerformDeactivateAfterComplete());
    }

    IEnumerator PerformDeactivateAfterComplete() {
        yield return new WaitForSeconds(deactivateAfterCompleteDelay);
        gameObject.SetActive(false);
    }
}
