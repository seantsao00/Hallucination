using UnityEngine;

public class Syncable : MonoBehaviour {
    private WorldSwitchManager worldSwitchManager;
    public GameObject SyncedObject;

    protected virtual void Awake() {
        worldSwitchManager = FindAnyObjectByType<WorldSwitchManager>();
    }

    protected void OnEnable() {
        worldSwitchManager.OnWorldSwitch.AddListener(SyncState);
    }

    protected void OnDisable() {
        worldSwitchManager.OnWorldSwitch.RemoveListener(SyncState);
    }

    public virtual void SyncState() {
        SyncedObject.transform.localPosition = transform.localPosition;
    }
}
