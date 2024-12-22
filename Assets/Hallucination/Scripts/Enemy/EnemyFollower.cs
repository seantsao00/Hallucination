using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollower : MonoBehaviour {
    public CharacterTracker characterTracker;
    Queue<Vector3> initialPath = new();
    Vector3 originalFairyPosition;
    Vector3 originalBearPosition;
    bool isPursuing = false;
    void Start() {
        isPursuing = true;
        originalFairyPosition = WorldSwitchManager.Instance.Fairy.transform.position;
        originalBearPosition = gameObject.transform.position;
        int maxPathPoint = 90;
        for (int i = 0; i < maxPathPoint; i++) {
            Vector3 initialPathPoint = ((originalBearPosition * (maxPathPoint - i) + originalFairyPosition * i) / maxPathPoint);
            initialPath.Enqueue(initialPathPoint);
        }
    }
    void FixedUpdate() {
        if (!isPursuing) return;
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

    public void EndPursue() {
        gameObject.GetComponent<Collider2D>().enabled = false;
        isPursuing = false;
        // StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut() {
        while (true) {
            Color originalColor = gameObject.GetComponent<SpriteRenderer>().color;
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b,
                                        Mathf.Clamp01(originalColor.a - Time.deltaTime * 2));
            gameObject.GetComponent<SpriteRenderer>().color = newColor;
            if (newColor.a <= 0) {
                break;
            }
            yield return null;
        }
        yield return null;
    }
}
