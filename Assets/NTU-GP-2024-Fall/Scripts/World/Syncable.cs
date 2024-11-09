using UnityEngine;
using UnityEditor;

public class Syncable : MonoBehaviour {
    public GameObject SyncedObject;

    protected void OnEnable() {
        WorldSwitchManager.Instance.OnWorldSwitch.AddListener(SyncState);
    }

    protected void OnDisable() {
        WorldSwitchManager.Instance.OnWorldSwitch.RemoveListener(SyncState);
    }

    protected virtual Vector3 SyncedLocalPosition() {
        return transform.localPosition;
    }

    public virtual void SyncState() {
        SyncedObject.transform.localPosition = SyncedLocalPosition();
    }

    void OnDrawGizmosSelected() {
        if (SyncedObject == null || WorldSwitchManager.Instance?.WorldFairyEnvironment == null) return;

        GameObject fairyWorld = WorldSwitchManager.Instance.WorldFairyEnvironment;
        GameObject bearWorld = WorldSwitchManager.Instance.WorldBearEnvironment;
        GameObject syncedWorld = transform.IsChildOf(fairyWorld.transform) ? bearWorld : fairyWorld;

        Vector3 syncedLocalPosition = SyncedLocalPosition();

        // Draw a rim around the position in WorldFairyEnvironment's local space
        Gizmos.color = Color.cyan;
        Vector3 syncedPosition = syncedWorld.transform.TransformPoint(syncedLocalPosition);
        
        // Draw a wire sphere at this position
        Gizmos.DrawWireSphere(syncedPosition, 0.3f);
        // Gizmos.DrawIcon(syncedPosition, "sv_icon_dot14_sml");
        Handles.Label(syncedPosition, "synced position");
    }
}
