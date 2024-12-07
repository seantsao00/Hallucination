using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FairyObjectProjectionManager : MonoBehaviour {
    [SerializeField] LayerMask excludeLayerMask;

    public static FairyObjectProjectionManager Instance { get; private set; }
    public HashSet<GameObject> Projections = new HashSet<GameObject>();

    void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning($"{nameof(FairyObjectProjectionManager)}: " +
            "Duplicate instance detected and removed. Only one instance is allowed.");
            Destroy(Instance);
            return;
        }
        Instance = this;
        foreach (GameObject projection in GameObject.FindGameObjectsWithTag("FairyWorldProjectionObject")) {
            foreach (var tilemap in projection.GetComponentsInChildren<Tilemap>()) {
                tilemap.CompressBounds();
            }
            foreach (var collider in projection.GetComponentsInChildren<Collider2D>()) {
                collider.excludeLayers = excludeLayerMask;
                Projections.Add(collider.gameObject);
            }
            foreach (var effector in projection.GetComponentsInChildren<Effector2D>()) {
                effector.enabled = false;
                foreach (var collider in effector.GetComponents<Collider2D>()) {
                    collider.usedByEffector = false;
                }
            }
        }
    }
}
