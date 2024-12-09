using UnityEngine;

public class EnemyFollower : MonoBehaviour {
    public CharacterTracker characterTracker;

    void FixedUpdate() {
        Vector3? targetPosition = characterTracker.GetOldestPosition();
        if (targetPosition == null) {
            return;
        } else {
            transform.position = targetPosition.Value + new Vector3(0, 0.8f, 0);
        }
    }
}
