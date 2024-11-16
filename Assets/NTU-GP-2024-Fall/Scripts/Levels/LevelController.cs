using UnityEngine;
using UnityEngine.Assertions;

public class LevelController : MonoBehaviour {
    [System.Serializable]
    public class CheckpointData {
        public LevelCheckpoint checkpoint;
        public bool switchWorld;
        public bool lockWorldSwitch;
        public string dialogueName;
    }
    [SerializeField] GameObject fairyObject, bearObject;
    [SerializeField] GameObject fairySpawnPoint, bearSpawnPoint;
    [SerializeField] GameObject fairyRespawnPoint, bearRespawnPoint;
    [SerializeField] CheckpointData[] checkpointDataList;
    GameObject testObject;

    void Start() {
        RegisterHandler();
        Debug.Log(testObject == fairyObject);
    }

    void RegisterHandler() {
        foreach (var checkpointData in checkpointDataList) {
            checkpointData.checkpoint.CheckpointCompleted.AddListener(Action => {
                if (checkpointData.switchWorld) WorldSwitchManager.Instance.ForceSwitchWorld();
                if (checkpointData.lockWorldSwitch) WorldSwitchManager.Instance.Lock(gameObject);
                if (checkpointData.dialogueName != "") {
                    DialogueManager.Instance.StartDialogue(checkpointData.dialogueName);
                }
            });
        }
    }

    public void StartLevel() {
        if (fairySpawnPoint != null)
            fairyObject.transform.position = fairySpawnPoint.transform.position;
        if (bearSpawnPoint != null)
            bearObject.transform.position = bearSpawnPoint.transform.position;
    }

    public void RestartLevel() {
        if (fairyRespawnPoint != null)
            fairyObject.transform.position = fairyRespawnPoint.transform.position;
        if (bearRespawnPoint != null)
            bearObject.transform.position = bearRespawnPoint.transform.position;
        testObject = fairyObject;
        Debug.Log("Restarted");
    }

    public void CompleteLevel() {
        Assert.IsTrue(this == LevelNavigator.Instance.CurrentLevel);
        LevelNavigator.Instance.CompleteCurrentLevel();
    }
}
