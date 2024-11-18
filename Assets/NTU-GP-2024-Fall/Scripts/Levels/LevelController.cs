using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class LevelController : MonoBehaviour {
    [System.Serializable] public class CheckpointData {
        public LevelCheckpoint Checkpoint;
        public CharacterTypeEnum WorldToSwitch;
        public GameObject FairySpawnPoint, BearSpawnPoint;
        public bool LockWorldSwitch;
        public bool UnlockWorldSwitch;
        public string DialogueName;
        public UnityEvent DialogueEndedEvent;
    }
    [SerializeField] GameObject fairyObject, bearObject;
    [SerializeField] CheckpointData startData;
    [SerializeField]
    CheckpointData restartData = new CheckpointData {
        WorldToSwitch = CharacterTypeEnum.Bear,
        UnlockWorldSwitch = true
    };
    [SerializeField] CheckpointData[] checkpointDataList;
    int numberOfFulfilledCheckpoints;

    void Awake() {
        if (restartData.WorldToSwitch == CharacterTypeEnum.None) {
            Debug.LogError("You must specify a world to restart.");
        }
    }

    void Start() {
        RegisterHandler();
    }

    void LoadCheckpointData(CheckpointData checkpointData) {
        WorldSwitchManager.Instance.SwitchToWorld(checkpointData.WorldToSwitch);
        if (checkpointData.LockWorldSwitch) WorldSwitchManager.Instance.Lock(gameObject);
        if (checkpointData.UnlockWorldSwitch) WorldSwitchManager.Instance.Unlock(gameObject);
        if (checkpointData.FairySpawnPoint != null) {
            fairyObject.transform.position = checkpointData.FairySpawnPoint.transform.position;
        }
        if (checkpointData.BearSpawnPoint != null) {
            bearObject.transform.position = checkpointData.BearSpawnPoint.transform.position;
        }
        if (!string.IsNullOrEmpty(checkpointData.DialogueName)) {
            StartCoroutine(WaitForWorldSwitchingAndStartDialogue(
                checkpointData.DialogueName,
                checkpointData.DialogueEndedEvent
            ));
        }
    }

    IEnumerator WaitForWorldSwitchingAndStartDialogue(string dialogueName, UnityEvent dialogueEndedEvent) {
        yield return new WaitUntil(
            () => GameStateManager.Instance.CurrentGamePlayState != GamePlayState.SwitchingWorld
        );
        DialogueManager.Instance.StartDialogue(
            dialogueName,
            () => dialogueEndedEvent?.Invoke()
        );
    }

    void FulfillCheckpoint() {
        numberOfFulfilledCheckpoints++;
        if (numberOfFulfilledCheckpoints == checkpointDataList.Length) {
            LevelNavigator.Instance.CompleteCurrentLevel();
        }
    }

    void RegisterHandler() {
        foreach (var checkpointData in checkpointDataList) {
            checkpointData.Checkpoint.CheckpointCompleted.AddListener(Action => {
                LoadCheckpointData(checkpointData);
                FulfillCheckpoint();
            });
        }
    }

    public void StartLevel() {
        if (startData != null) {
            if (startData.Checkpoint != null) {
                Debug.LogWarning(
                    $"The {nameof(LevelCheckpoint)} assignment in start data is redundant and will have no effect."
                );
            }
            LoadCheckpointData(startData);

            transform.Find("FairyWorld").Find("LevelMainCamera").gameObject.SetActive(true);
            transform.Find("BearWorld").Find("LevelMainCamera").gameObject.SetActive(true);
        }
    }

    public void RestartLevel() {
        if (restartData != null) {
            if (restartData.Checkpoint != null) {
                Debug.LogWarning(
                    $"The {nameof(LevelCheckpoint)} assignment in restart data is redundant and will have no effect."
                );
            }
            LoadCheckpointData(restartData);

            transform.Find("FairyWorld").Find("LevelMainCamera").gameObject.SetActive(true);
            transform.Find("BearWorld").Find("LevelMainCamera").gameObject.SetActive(true);
        }
    }

    public void CompleteLevel() {
        Assert.IsTrue(this == LevelNavigator.Instance.CurrentLevel);
        LevelNavigator.Instance.CompleteCurrentLevel();

        transform.Find("FairyWorld").Find("LevelMainCamera").gameObject.SetActive(false);
        transform.Find("BearWorld").Find("LevelMainCamera").gameObject.SetActive(false);
    }
}
