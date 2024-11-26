using UnityEngine;
using System.Collections.Generic;

public class MovableObject : MonoBehaviour, IButtonControlled, ISwitchControlled {
    Transform movableObject;
    [SerializeField] bool autoMove;
    [SerializeField] Transform[] points;
    [SerializeField] float slowDownLength = 2f;
    [SerializeField] float minSpeedRatio = 0.33f;
    [SerializeField] int initialPositionIndex;
    [SerializeField] float speed = 3f;
    float eps = 1e-4f;
    Vector3 currentPosition;
    int currentMovementTarget;
    private Dictionary<Collider2D, Transform> originalParents = new Dictionary<Collider2D, Transform>();

    public void SetTargetIndex(int index) {
        currentMovementTarget = index;
    }

    public void Activate() {
        if (initialPositionIndex != 0) {
            Debug.LogWarning("Movable Object is controlled by a button but its initial position index is not 0.");
        }
        if (points.Length < 2) {
            Debug.LogWarning("Movable Object is controlled by a button but its points length is less then 2.");
        }
        currentMovementTarget = 1;
    }

    public void Deactivate() {
        currentMovementTarget = 0;
    }

    public void SetState(int index) {
        currentMovementTarget = index % points.Length;
    }

    void Update() {
        Vector3 target = points[currentMovementTarget].position;
        Vector3 offset = target - currentPosition;
        if (offset.magnitude < eps) {
            WorldSwitchManager.Instance.Unlock(gameObject);
        } else {
            WorldSwitchManager.Instance.Lock(gameObject);
        }
        Vector3 Movement;
        if (offset.magnitude > slowDownLength) {
            Movement = Vector3.ClampMagnitude(offset, speed * Time.deltaTime);
        } else {
            float ratio = Mathf.Max(offset.magnitude / slowDownLength, minSpeedRatio);
            Movement = Vector3.ClampMagnitude(offset, speed * Time.deltaTime * ratio);
        }
        movableObject.transform.position += Movement;
        currentPosition += Movement;
        if (autoMove) {
            if (offset.magnitude < eps) {
                int nextTarget = (currentMovementTarget + 1) % points.Length;
                SetTargetIndex(nextTarget);
            }
        } else {
            if (offset.magnitude < eps) {
            }
        }
    }

    void Awake() {
        movableObject = GetComponent<Transform>();
        currentMovementTarget = initialPositionIndex;
        currentPosition = points[initialPositionIndex].position;
        if (movableObject != null && points.Length >= 2) {
            for (int i = 0; i < points.Length; i++) {
                if (!Mathf.Approximately(points[i].localPosition.z, 0f)) {
                    Debug.LogWarning($"Z-coordinate of point {i} in the movable object path is non-zero" +
                    $"({points[i].transform.position}). Automatically setting it to 0.");
                    Vector3 correctedPosition = points[i].position;
                    correctedPosition.z = 0f;
                    points[i].position = correctedPosition;
                }
            }
        }
    }

    void RestoreColliderParent(Collider2D collider) {
        // The world of the collider may be Deactivated
        // We do not care about this case
        if (!originalParents[collider].gameObject.activeInHierarchy) return;
        collider.transform.SetParent(null);
        collider.transform.SetParent(originalParents[collider]);
    }

    bool CanBeCarried(Collider2D collider) {
        return collider.CompareTag("Player") || collider.CompareTag("Stone");
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (CanBeCarried(collider)) {
            if (collider.transform.parent != transform) {
                originalParents[collider] = collider.transform.parent;
            }
            collider.transform.SetParent(transform);
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if (CanBeCarried(collider)) {
            RestoreColliderParent(collider);
        }
    }

    void OnDrawGizmos() {
        Color outlineColor = Color.white;
        Gizmos.color = outlineColor;
        if (movableObject != null && points.Length >= 2) {
            for (int i = 0; i < points.Length; i++) {
                Gizmos.DrawLine(points[i].position, points[(i + 1) % points.Length].position);
            }
        }
    }
}
