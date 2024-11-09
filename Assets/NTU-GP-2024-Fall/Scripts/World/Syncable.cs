using UnityEngine;
using UnityEditor;

public class Syncable : MonoBehaviour {
    public GameObject SyncedObject;
    GameObject currentWorld, syncedWorld;

    void Start() {
        currentWorld = CurrentWorld();
        syncedWorld = SyncedWorld();
    }

    protected void OnEnable() {
        WorldSwitchManager.Instance.OnWorldSwitch.AddListener(SyncState);
    }

    protected void OnDisable() {
        WorldSwitchManager.Instance.OnWorldSwitch.RemoveListener(SyncState);
    }

    protected Vector3 CurrentWorldLocalPosition() {
        return currentWorld.transform.InverseTransformPoint(transform.position);
    }

    protected virtual Vector3 SyncedPosition() {
        Vector3 currentWorldPosition = CurrentWorldLocalPosition();
        return syncedWorld.transform.TransformPoint(currentWorldPosition);
    }

    public virtual void SyncState() {
        SyncedObject.transform.position = SyncedPosition();
    }

    void OnDrawGizmosSelected() {
        if (SyncedObject == null || WorldSwitchManager.Instance?.WorldFairyEnvironment == null) return;
        currentWorld = CurrentWorld();
        syncedWorld = SyncedWorld();

        // Draw a rim around the position in WorldFairyEnvironment's local space
        Gizmos.color = Color.cyan;
        Vector3 syncedPosition = SyncedPosition();
        
        // Draw a wire sphere at this position
        Gizmos.DrawWireSphere(syncedPosition, 0.3f);
        // Gizmos.DrawIcon(syncedPosition, "sv_icon_dot14_sml");
        Handles.Label(syncedPosition, "synced position");
    }

    protected GameObject CurrentWorld() {
        GameObject fairyWorld = WorldSwitchManager.Instance.WorldFairyEnvironment;
        GameObject bearWorld = WorldSwitchManager.Instance.WorldBearEnvironment;
        return transform.IsChildOf(fairyWorld.transform) ? fairyWorld : bearWorld;
    }

    protected GameObject SyncedWorld() {
        GameObject fairyWorld = WorldSwitchManager.Instance.WorldFairyEnvironment;
        GameObject bearWorld = WorldSwitchManager.Instance.WorldBearEnvironment;
        return transform.IsChildOf(bearWorld.transform) ? fairyWorld : bearWorld;
    }
}
