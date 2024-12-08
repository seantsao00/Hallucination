using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Flower : MonoBehaviour {
    bool isPlayerInRange = false;
    CharacterProjectionDetector detector;
    List<GameObject> duplicatedObjects;
    public float activateDuration = 3f;
    [SerializeField] CapturedSurroundings capturedSurroundings;
    public bool isActivated { get; private set; } = false;
    float targetAlpha = 1f;

    void Awake() {
        detector = WorldSwitchManager.Instance.Bear.GetComponentInChildren<CharacterProjectionDetector>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !isActivated) {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
        }
    }

    void Update() {
        if (isPlayerInRange && !isActivated) {
            StartCoroutine(HandleActivation());
        }
    }

    IEnumerator HandleActivation() {
        isActivated = true;
        capturedSurroundings.Activate(activateDuration);
        duplicatedObjects = new();
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.0f);
        WorldSwitchManager.Instance.Lock(gameObject);
        DuplicateProjectionObjects();
        yield return new WaitForSeconds(activateDuration);
        DestroyProjectionObjects();
        capturedSurroundings.Deactivate();
        duplicatedObjects = null;
        WorldSwitchManager.Instance.Unlock(gameObject);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
        isActivated = false;
    }

    void DuplicateProjectionObjects() {
        Vector2 offsetF = transform.position - detector.transform.position;
        Vector2Int offset = new Vector2Int(Mathf.RoundToInt(offsetF.x), Mathf.RoundToInt(offsetF.y));
        // Debug.Log($"duplicating ${detector.ProjectionObjects.Count} projection objects");
        foreach (var projection in detector.ProjectionObjects) {
            GameObject duplicatedObject;
            Vector2 newPosition = (Vector2)projection.transform.position + offset;
            Transform parent = transform.parent;
            duplicatedObject = Instantiate(projection, newPosition, projection.transform.rotation, parent);
            if (duplicatedObject.GetComponent<Tilemap>() != null) {
                TrimTilemap(detector.transform.position, detector.radius,
                    projection.GetComponent<Tilemap>(), duplicatedObject.GetComponent<Tilemap>());
            } else {
                var spriteRenderer = duplicatedObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) {
                    spriteRenderer.color = new Color(1, 1, 1, targetAlpha);
                }
            }
            RecoverCollisionLayer(duplicatedObject);
            duplicatedObject.name = projection.name + "_Copy";
            duplicatedObjects.Add(duplicatedObject);
        }
    }

    void TrimTilemap(Vector2 center, float radius, Tilemap sourceTilemap, Tilemap duplicatedTilemap) {
        duplicatedTilemap.color = new Color(1, 1, 1, targetAlpha);
        BoundsInt bounds = sourceTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++) {
            for (int y = bounds.yMin; y < bounds.yMax; y++) {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = sourceTilemap.GetTile(position);
                if (tile != null) {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    Vector3 tileWorldPos = sourceTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);
                    if (Vector3.Distance(tileWorldPos, center) > radius) {
                        duplicatedTilemap.SetTile(position, null);
                    }
                }
            }
        }
        duplicatedTilemap.RefreshAllTiles();
    }

    void RecoverCollisionLayer(GameObject obj) {
        if (obj.layer == LayerMask.NameToLayer("ProjectionGround")) {
            obj.layer = LayerMask.NameToLayer("Ground");
        } else if (obj.layer == LayerMask.NameToLayer("FairyProjection")) {
            obj.layer = LayerMask.NameToLayer("Default");
        }
        Vector2 fairyPosition = WorldSwitchManager.Instance.Fairy.transform.position;
        foreach (var collider in obj.GetComponents<Collider2D>()) {
            collider.excludeLayers = 0;
            // if (obj.layer == LayerMask.NameToLayer("Ground")) {
            //     if (collider.OverlapPoint(fairyPosition)) {
            //         Vector2 closestPoint = collider.ClosestPoint(fairyPosition);
            //         Debug.Log(closestPoint);
            //         Vector2 direction = (fairyPosition - closestPoint).normalized;
            //         obj.transform.position = closestPoint + direction * 0.1f;
            //         // WorldSwitchManager.Instance.Fairy.GetComponent<CharacterDeath>().TakeDamage();
            //     }
            // }
        }
        foreach (var effector in obj.GetComponents<Effector2D>()) {
            effector.enabled = true;
            foreach (var collider in effector.GetComponents<Collider2D>()) {
                collider.usedByEffector = true;
            }
        }
    }
    void DestroyProjectionObjects() {
        // print(duplicatedObjects);
        foreach (var duplicatedObject in duplicatedObjects) {
            Flower flower = duplicatedObject.GetComponent<Flower>();
            Spring spring = duplicatedObject.GetComponent<Spring>();
            if (flower != null) {
                flower.DestroySelfAfterDeactivated();
            } else if (spring != null) {
                spring.DestroySelfAfterDeactivated();
            } else {
                Destroy(duplicatedObject);
            }
        }
    }

    void DestroySelfAfterDeactivated() => StartCoroutine(DestroyCoroutine());
    IEnumerator DestroyCoroutine() {
        yield return new WaitUntil(() => !isActivated);
        Destroy(gameObject);
    }
}
