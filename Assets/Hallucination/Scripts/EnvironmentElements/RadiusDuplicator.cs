using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RadiusDuplicator : MonoBehaviour {
    public string targetTag = "Stone";
    public float radius;
    public GameObject newCenterObject;
    Vector2 newCenterPosition;

    void Awake() {
        newCenterPosition = (Vector2)newCenterObject.transform.position;
    }
    GameObject[] FindObjectsInRadius() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        List<GameObject> objectInRadius = new();
        foreach (Collider2D collider in colliders) {
            print(collider.gameObject.name + " " + transform.position + " " + collider.gameObject.transform.position);
            if (collider.CompareTag(targetTag)) {
                Debug.Log($"Found object with tag '{targetTag}': {collider.gameObject.name}");
                objectInRadius.Add(collider.gameObject);
            }

        }
        return objectInRadius.ToArray();
    }

    public void DuplicateObjectsInRadius() {
        // Get all colliders within the radius
        GameObject[] objectInRadius = FindObjectsInRadius();

        foreach (GameObject originalObject in objectInRadius) {
            Vector2 originalPosition = originalObject.transform.position;
            Vector2 relativePosition = originalPosition - (Vector2)transform.position;

            Vector2 newPosition = newCenterPosition + relativePosition;

            GameObject duplicatedObject = Instantiate(originalObject, newPosition, originalObject.transform.rotation);

            duplicatedObject.name = originalObject.name + "_Copy";

            Debug.Log($"Duplicated {originalObject.name} to new position {newPosition}");
        }
    }


    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}


