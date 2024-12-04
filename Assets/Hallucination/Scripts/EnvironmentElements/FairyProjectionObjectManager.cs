using System.Collections.Generic;
using UnityEngine;

public class FairyObjectProjectionManager : MonoBehaviour {
    [SerializeField] LayerMask excludeLayerMask;

    public static FairyObjectProjectionManager Instance { get; private set; }
    public HashSet<GameObject> Projections= new HashSet<GameObject>();

    void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning($"{nameof(FairyObjectProjectionManager)}: " +
            "Duplicate instance detected and removed. Only one instance is allowed.");
            Destroy(Instance);
            return;
        }
        Instance = this;
        Projections = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("FairyWorldProjectionObject"));
        foreach (GameObject projection in Projections) {
            foreach (var collider in projection.GetComponentsInChildren<Collider2D>()) {
                // Debug.Log("Normal Collider");
                SetColliderExcludeLayers(collider);
                Projections.Add(collider.gameObject);
            }
        }
    }
    void SetColliderExcludeLayers(Collider2D collider) {
        if (collider != null) {
            collider.excludeLayers = excludeLayerMask;
        }
    }
}
