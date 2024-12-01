using System.Collections.Generic;
using UnityEngine;
public class CharacterProjectionDetector : MonoBehaviour {
    List<GameObject> projectionObjects = new();
    public List<GameObject> ProjectionObjects {get {return projectionObjects;}}
    
    void OnTriggerEnter2D(Collider2D other) {
        
        if (other.CompareTag("FairyWorldProjectionObject") && !projectionObjects.Contains(other.gameObject)) {
            print(other.gameObject.name + " Entered");
            projectionObjects.Add(other.gameObject);
            
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("FairyWorldProjectionObject") && gameObject.activeInHierarchy) {
            projectionObjects.Remove(other.gameObject);
            print(other.gameObject.name + " Exited");
        }
    }
    public Vector2 GetRelativePosition(GameObject projection) {
        return (Vector2)(projection.transform.position - gameObject.transform.position);
    }

    Transform GetParentTransform(Collider2D other) {
        return other.transform.parent.transform;
    }

}