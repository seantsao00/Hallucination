using UnityEngine;

public class Syncable : MonoBehaviour {
    private WorldSwitchManager worldSwitchManager;
    public GameObject SyncedObject;

    void Awake() {
        worldSwitchManager = FindAnyObjectByType<WorldSwitchManager>();
    }

    private void OnEnable() {
        worldSwitchManager.OnWorldSwitch.AddListener(SyncState);
    }

    private void OnDisable() {
        worldSwitchManager.OnWorldSwitch.RemoveListener(SyncState);
    }

    public void SyncState() {
        SyncedObject.transform.localPosition = transform.localPosition;
    }
}
