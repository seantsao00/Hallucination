using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour {
    bool isPlayerInRange = false;
    CharacterProjectionDetector detector;
    List<GameObject> duplicatedObjects;
    public float activateDuration = 3f;
    public LayerMask excludeLayerMask;
    bool isFlowerActivated = false;

    void Awake() {
        detector = WorldSwitchManager.Instance.Bear.GetComponentInChildren<CharacterProjectionDetector>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !isFlowerActivated) {
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
        DuplicateProjectionObjects();
        yield return new WaitForSecondsRealtime(activateDuration);
        DestroyProjectionObjects();
        isFlowerActivated = false;
        duplicatedObjects = null;
        WorldSwitchManager.Instance.Unlock(gameObject);
    }

    void DuplicateProjectionObjects() {
        GameObject[] projections = detector.ProjectionObjects.ToArray();
        // Debug.Log($"Duplicating {projections.Length} objects");
        foreach (var projection in projections) {
            Vector2 relativePosition = detector.GetRelativePosition(projection);
            Vector2 newPosition = (Vector2)gameObject.transform.position + relativePosition;
            Transform parent = gameObject.transform.parent;
            GameObject duplicatedObject = Instantiate(projection, newPosition, projection.transform.rotation, parent);
            duplicatedObjects.Add(duplicatedObject);
            RecoverCollisionLayer(duplicatedObject);
            duplicatedObject.name = projection.name + "_Copy";
            if (duplicatedObject.layer == LayerMask.NameToLayer("ProjectionGround")) {
                duplicatedObject.layer = LayerMask.NameToLayer("Ground");
            }

            Debug.Log($"Duplicated {projection.name} to new position {newPosition}");
        }
    }
    void RecoverCollisionLayer(GameObject obj) {
        Collider2D collider = obj.GetComponent<Collider2D>();
        collider.excludeLayers = excludeLayerMask;
    }
    void DestroyProjectionObjects() {
        // print(duplicatedObjects);
        foreach (var duplicatedObject in duplicatedObjects) {
            Destroy(duplicatedObject);
        }
    }


}
