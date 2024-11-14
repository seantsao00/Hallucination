using UnityEngine;
using UnityEditor;

public class Syncable : MonoBehaviour {
    [SerializeField] protected GameObject syncedObject;
    [SerializeField] GameObject currentWorldReference, syncedWorldReference;
    // GameObject currentWorld, syncedWorld;

    void Start() {
        Position();
    }

    void Position() {
        // currentWorld = CurrentWorld();
        // syncedWorld = SyncedWorld();
        // if (currentWorldReference == null) currentWorldReference = currentWorld;
        // if (syncedWorldReference == null) syncedWorldReference = syncedWorld;
    }

    protected void OnEnable() {
        WorldSwitchManager.Instance.OnWorldSwitch.AddListener(SyncState);
    }

    protected void OnDisable() {
        // WorldSwitchManager may be destroyed before this object
        // We do not care about this case
        if (WorldSwitchManager.Instance == null) return; 
    
        WorldSwitchManager.Instance.OnWorldSwitch.RemoveListener(SyncState);
    }

    // protected Vector3 CurrentWorldLocalPosition() {
        // return currentWorld.transform.InverseTransformPoint(transform.position);
    // }

    protected Vector3 CurrentWorldReferenceLocalPosition() {
        return currentWorldReference.transform.InverseTransformPoint(transform.position);
    }

    protected virtual Vector3 SyncedPosition() {
        return syncedWorldReference.transform.TransformPoint(CurrentWorldReferenceLocalPosition());
    }

    public virtual void SyncState() {
        syncedObject.transform.position = SyncedPosition();
    }

    void OnDrawGizmosSelected() {
        if (syncedObject == null || WorldSwitchManager.Instance?.WorldFairyEnvironment == null) return;

        Position();

        // Draw a rim around the position in WorldFairyEnvironment's local space
        Gizmos.color = Color.cyan;
        Vector3 syncedPosition = SyncedPosition();

        // Draw a wire sphere at this position
        Gizmos.DrawWireSphere(syncedPosition, 0.3f);
        // Gizmos.DrawIcon(syncedPosition, "sv_icon_dot14_sml");
        Handles.Label(syncedPosition, "synced position");
    }
}
