using UnityEngine;

public class SyncSamePosition : Syncable {
    public Transform syncedPosition;

    protected override Vector3 SyncedPosition() {
        return syncedPosition.position;
    }
}
