using UnityEngine;
using UnityEditor;

public class Airstream : MonoBehaviour {
    Character character;
    CompositeCollider2D compositeCollider;
    CharacterStateController characterStateController;

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            character = collision.GetComponent<Character>();
            characterStateController = collision.GetComponent<CharacterStateController>();
            characterStateController.AddState(CharacterState.BeingBlown);
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") && character != null) {
            characterStateController.RemoveState(CharacterState.BeingBlown);
            character = null;
        }
    }

    private void OnDrawGizmos() {
        if (compositeCollider == null) {
            compositeCollider = GetComponent<CompositeCollider2D>();
            if (compositeCollider == null) return;
        }
        Color outlineColor = Color.blue;
        Color fillColor = new Color(0, 0, 1, 0.25f);
        // Iterate through each path in the CompositeCollider2D
        for (int i = 0; i < compositeCollider.pathCount; i++) {
            Vector2[] points = new Vector2[compositeCollider.GetPathPointCount(i)];
            compositeCollider.GetPath(i, points);

            // Draw filled shape
            #if UNITY_EDITOR
            Handles.color = fillColor;
            Handles.DrawAAConvexPolygon(System.Array.ConvertAll(points, p => (Vector3)transform.TransformPoint(p)));
            #endif

            // Draw outline
            Gizmos.color = outlineColor;
            for (int j = 0; j < points.Length; j++) {
                Vector2 startPoint = points[j];
                Vector2 endPoint = points[(j + 1) % points.Length];
                Gizmos.DrawLine(transform.TransformPoint(startPoint), transform.TransformPoint(endPoint));
            }
        }
    }
}
