using System.Collections.Generic;
using UnityEngine;
public class CharacterProjectionDetector : MonoBehaviour {
    public HashSet<GameObject> ProjectionObjects = new();

    void Awake() {
        WorldSwitchManager.Instance.WorldSwitching.AddListener(() => gameObject.SetActive(false));
        WorldSwitchManager.Instance.WorldSwitched.AddListener(() => gameObject.SetActive(true));
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (FairyObjectProjectionManager.Instance.Projections.Contains(other.gameObject)
            && !ProjectionObjects.Contains(other.gameObject)) {
            Debug.Log("Projection detected: " + other.gameObject.name + " Entered");
            ProjectionObjects.Add(other.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (FairyObjectProjectionManager.Instance.Projections.Contains(other.gameObject)
            && gameObject.activeInHierarchy) {
            Debug.Log("Projection detected" + other.gameObject.name + " Exited");
            ProjectionObjects.Remove(other.gameObject);
        }
    }
    public Vector2 GetRelativePosition(GameObject projection) {
        return (Vector2)(projection.transform.position - gameObject.transform.position);
    }
}
