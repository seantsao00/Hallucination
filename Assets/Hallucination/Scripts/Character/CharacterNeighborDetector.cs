using System.Collections.Generic;
using UnityEngine;
public class CharacterNeighborDetector : MonoBehaviour {
    public string targetTag = "Stone";
    List<GameObject> neighborObjects = new();
    public List<GameObject> NeighborObjects {get {return neighborObjects;}}
    
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(targetTag) && !neighborObjects.Contains(other.gameObject)) {
            neighborObjects.Add(other.gameObject);
            // print(other.gameObject.name + " Entered");
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag(targetTag) && gameObject.activeInHierarchy) {
            neighborObjects.Remove(other.gameObject);
            // print(other.gameObject.name + " Exited");
        }
    }
    public Vector2 GetRelativePosition(GameObject neighbor) {
        return (Vector2)(neighbor.transform.position - gameObject.transform.position);
    }

}