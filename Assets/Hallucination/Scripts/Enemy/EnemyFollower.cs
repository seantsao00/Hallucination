using System.Collections.Generic;
using UnityEngine;

public class EnemyFollower : MonoBehaviour {
    public CharacterTracker characterTracker;
    Queue<Vector3> initialPath = new();
    Vector3 originalFairyPosition;
    Vector3 originalBearPosition;
    void Start() {
        originalFairyPosition = WorldSwitchManager.Instance.Fairy.transform.position;
        originalBearPosition = gameObject.transform.position;
        int maxPathPoint = 90;
        for (int i = 0; i < maxPathPoint; i++) {
            Vector3 initialPathPoint = ((originalBearPosition * (maxPathPoint - i) + originalFairyPosition * i) / maxPathPoint);
            initialPath.Enqueue(initialPathPoint);
        }
    }
    void FixedUpdate() {
        Vector3? targetPosition;
        if (initialPath.Count != 0) {
            targetPosition = initialPath.Peek();
            initialPath.Dequeue();
            transform.position = targetPosition.Value + new Vector3(0, 0.8f, 0);
            return;
        }
        targetPosition = characterTracker.GetOldestPosition();
        if (targetPosition == null) {
            return;
        } else {
            transform.position = targetPosition.Value + new Vector3(0, 0.8f, 0);
        }
    }
}
