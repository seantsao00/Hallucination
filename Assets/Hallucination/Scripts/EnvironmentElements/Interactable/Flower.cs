using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class Flower : MonoBehaviour {
    bool isPlayerInRange;
    public CharacterNeighborDetector detector;
    List<GameObject> duplicatedObjects;
    public float activateDuration = 3f;
    bool isFlowerActivated = false;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !isFlowerActivated) {
            
            print("entered");
            isPlayerInRange = true;
            StartCoroutine(HandleActivation());
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
        }
    }

    IEnumerator HandleActivation() {
        isFlowerActivated = true;
        duplicatedObjects = new();
        WorldSwitchManager.Instance.Lock(gameObject);
        DuplicateNeighborObjects();
        yield return new WaitForSecondsRealtime(activateDuration);
        DestroyNeighborObjects();
        isFlowerActivated = false;
        duplicatedObjects = null;
        WorldSwitchManager.Instance.Unlock(gameObject);
    }
    
    void DuplicateNeighborObjects() {
        GameObject[] neighbors = detector.NeighborObjects.ToArray();
        foreach (var neighbor in neighbors) {
            Vector2 relativePosition = detector.GetRelativePosition(neighbor);
            Vector2 newPosition = (Vector2)gameObject.transform.position + relativePosition;
            Transform parent = gameObject.transform.parent;
            GameObject duplicatedObject = Instantiate(neighbor, newPosition, neighbor.transform.rotation, parent);
            duplicatedObjects.Add(duplicatedObject);

            duplicatedObject.name = neighbor.name + "_Copy";

            Debug.Log($"Duplicated {neighbor.name} to new position {newPosition}");
        }
    }
    void DestroyNeighborObjects() {
        print(duplicatedObjects);
        foreach (var duplicatedObject in duplicatedObjects) {
            Destroy(duplicatedObject);
        }
    }

    
}
