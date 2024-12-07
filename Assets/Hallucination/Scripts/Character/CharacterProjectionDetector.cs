using System.Collections.Generic;
using UnityEngine;
public class CharacterProjectionDetector : MonoBehaviour {
    public HashSet<GameObject> ProjectionObjects = new();
    public float radius => GetComponent<CircleCollider2D>().radius;

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.activeInHierarchy) return;
        if (FairyObjectProjectionManager.Instance.Projections.Contains(other.gameObject)
            && !ProjectionObjects.Contains(other.gameObject)) {
            // Debug.Log("Projection " + other.gameObject.name + " Entered");
            ProjectionObjects.Add(other.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (!other.gameObject.activeInHierarchy) return;
        if (FairyObjectProjectionManager.Instance.Projections.Contains(other.gameObject)
            && gameObject.activeInHierarchy) {
            // Debug.Log("Projection " + other.gameObject.name + " Exited");
            ProjectionObjects.Remove(other.gameObject);
        }
    }
    public Vector2 GetRelativePosition(GameObject projection) {
        return (Vector2)(projection.transform.position - gameObject.transform.position);
    }
}
