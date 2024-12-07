using System.Collections.Generic;
using UnityEngine;

public class CharacterTracker : MonoBehaviour {
    private float trackingDelay = 2f; // Delay in seconds
    private Queue<Vector3> positionHistory = new();

    void FixedUpdate() {
        positionHistory.Enqueue(transform.position);
    }

    public Vector3? GetOldestPosition() {
        if (positionHistory.Count < trackingDelay / Time.deltaTime) return null;

        while (positionHistory.Count > trackingDelay / Time.deltaTime) {
            Debug.Log("Dequeueing");
            positionHistory.Dequeue();
        }

        return positionHistory.Peek();
    }
}
