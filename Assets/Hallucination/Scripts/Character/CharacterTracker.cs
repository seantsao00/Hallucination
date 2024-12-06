using System.Collections.Generic;
using UnityEngine;

public class CharacterTracker : MonoBehaviour {
    private float trackingDelay = 2f; // Delay in seconds
    private Queue<Vector3> positionHistory = new();

    void Update() {
        positionHistory.Enqueue(transform.position);

        while (positionHistory.Count > 0 && positionHistory.Count > trackingDelay / Time.deltaTime) {
            positionHistory.Dequeue();
        }
    }

    public Vector3 GetOldestPosition() {
        return positionHistory.Count > 0 ? positionHistory.Peek() : transform.position;
    }
}
