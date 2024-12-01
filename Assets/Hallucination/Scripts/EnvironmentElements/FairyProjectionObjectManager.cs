using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyObjectProjectionManager : MonoBehaviour {
        
    public LayerMask excludeLayerMask;
    void Awake() {
        GameObject[] projections = GetAllProjection();
        foreach (GameObject projection in projections)
        {
            SetColliderExcludeLayers(projection);
        }
    }
    GameObject[] GetAllProjection() {

        return GameObject.FindGameObjectsWithTag("FairyWorldProjectionObject");
    }
    void SetColliderExcludeLayers(GameObject obj)
    {
        Collider2D collider = obj.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.excludeLayers = excludeLayerMask; // Set the exclude layers
        }
    }
}
