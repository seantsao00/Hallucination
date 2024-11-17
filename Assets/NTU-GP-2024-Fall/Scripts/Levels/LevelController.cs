using UnityEngine;
using UnityEngine.Assertions;

public class LevelController : MonoBehaviour {
    [System.Serializable]
    public class CheckpointData {
        public LevelCheckpoint checkpoint;
        public CharacterTypeEnum WorldToSwitch;
        public GameObject fairySpawnPoint, bearSpawnPoint;
        public bool lockWorldSwitch;
        public bool unlockWorldSwitch;
        public string dialogueName;
    }
    [SerializeField] GameObject fairyObject, bearObject;
    [SerializeField] CheckpointData startData;
    [SerializeField]
    CheckpointData restartData = new CheckpointData {
        WorldToSwitch = CharacterTypeEnum.Bear,
        unlockWorldSwitch = true
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
        if (checkpointData.lockWorldSwitch) WorldSwitchManager.Instance.Lock(gameObject);
        if (checkpointData.unlockWorldSwitch) WorldSwitchManager.Instance.Unlock(gameObject);
        if (checkpointData.fairySpawnPoint != null) {
            fairyObject.transform.position = checkpointData.fairySpawnPoint.transform.position;
        }
        if (checkpointData.bearSpawnPoint != null) {
            bearObject.transform.position = checkpointData.bearSpawnPoint.transform.position;
        }
        if (!string.IsNullOrEmpty(checkpointData.dialogueName)) {
            DialogueManager.Instance.StartDialogue(checkpointData.dialogueName);
        }
    }

    void FulfillCheckpoint() {
        numberOfFulfilledCheckpoints++;
        if (numberOfFulfilledCheckpoints == checkpointDataList.Length) {
            LevelNavigator.Instance.CompleteCurrentLevel();
        }
    }

    void RegisterHandler() {
        foreach (var checkpointData in checkpointDataList) {
            checkpointData.checkpoint.CheckpointCompleted.AddListener(Action => {
                LoadCheckpointData(checkpointData);
                FulfillCheckpoint();
            });
        }
    }

    public void StartLevel() {
        if (startData != null) {
            if (startData.checkpoint != null) {
                Debug.LogWarning(
                    $"The {nameof(LevelCheckpoint)} assignment in start data is redundant and will have no effect."
                );
            }
            LoadCheckpointData(startData);
        }
    }

    public void RestartLevel() {
        if (restartData != null) {
            if (restartData.checkpoint != null) {
                Debug.LogWarning(
                    $"The {nameof(LevelCheckpoint)} assignment in restart data is redundant and will have no effect."
                );
            }
            LoadCheckpointData(restartData);
        }
    }

    public void CompleteLevel() {
        Assert.IsTrue(this == LevelNavigator.Instance.CurrentLevel);
        LevelNavigator.Instance.CompleteCurrentLevel();
    }
}
