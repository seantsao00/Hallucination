using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Flower : MonoBehaviour {
    bool isPlayerInRange = false;
    CharacterProjectionDetector detector;
    List<GameObject> duplicatedObjects;
    public float activateDuration = 3f;
    public CapturedSurroundings capturedSurroundings;
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
        capturedSurroundings.Activate();
        duplicatedObjects = new();
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
        WorldSwitchManager.Instance.Lock(gameObject);
        DuplicateProjectionObjects();
        yield return new WaitForSecondsRealtime(activateDuration);
        DestroyProjectionObjects();
        isFlowerActivated = false;
        capturedSurroundings.Deactivate();
        duplicatedObjects = null;
        WorldSwitchManager.Instance.Unlock(gameObject);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
    }

    void DuplicateProjectionObjects() {
        Vector2 offsetF = transform.position - detector.transform.position;
        Vector2Int offset = new Vector2Int(Mathf.RoundToInt(offsetF.x), Mathf.RoundToInt(offsetF.y));
        foreach (var projection in detector.ProjectionObjects) {
            GameObject duplicatedObject;
            Vector2 newPosition = (Vector2)projection.transform.position + offset;
            Transform parent = transform.parent;
            duplicatedObject = Instantiate(projection, newPosition, projection.transform.rotation, parent);
            RecoverCollisionLayer(duplicatedObject);
            if (duplicatedObject.GetComponent<Tilemap>() != null) {
                TrimTilemap(detector.transform.position, detector.radius,
                    projection.GetComponent<Tilemap>(), duplicatedObject.GetComponent<Tilemap>());
            } else {
                var spriteRenderer = duplicatedObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) {
                    spriteRenderer.color = new Color(1, 1, 1, 1f);
                }
            }
            duplicatedObject.name = projection.name + "_Copy";
            if (projection.layer == LayerMask.NameToLayer("ProjectionGround")) {
                duplicatedObject.layer = LayerMask.NameToLayer("Ground");
            }
            duplicatedObjects.Add(duplicatedObject);
        }
    }

    void TrimTilemap(Vector2 center, float radius, Tilemap sourceTilemap, Tilemap duplicatedTilemap) {
        duplicatedTilemap.color = new Color(1, 1, 1, 1f);
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

        // BoundsInt bounds = new BoundsInt(
        //     sourceTilemap.WorldToCell(center),
        //     new Vector3Int(Mathf.CeilToInt(radius * 2) + 1, Mathf.CeilToInt(radius * 2) + 1)
        // );
        // for (int x = bounds.xMin; x <= bounds.xMax; x++) {
        //     for (int y = bounds.yMin; y <= bounds.yMax; y++) {
        //         Vector3Int tilePos = new Vector3Int(x, y, 0);
        //         Vector3 tileWorldPos = sourceTilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);
        //         if (Vector3.Distance(tileWorldPos, center) <= radius) {
        //             TileBase tile = sourceTilemap.GetTile(tilePos);
        //             if (tile != null) {
        //                 duplicatedTilemap.SetTile(tilePos, tile);
        //             }
        //         }
        //     }
        // }
        duplicatedTilemap.RefreshAllTiles();
    }

    void RecoverCollisionLayer(GameObject obj) {
        Collider2D[] colliders = obj.GetComponents<Collider2D>();
        foreach (var collider in colliders) collider.excludeLayers = 0;
    }
    void DestroyProjectionObjects() {
        // print(duplicatedObjects);
        foreach (var duplicatedObject in duplicatedObjects) {
            Destroy(duplicatedObject);
        }
    }


}
