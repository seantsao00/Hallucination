using UnityEngine;

public class LedgeDetection : MonoBehaviour {
    [SerializeField] float radius = 0.2f;
    [SerializeField] LayerMask GroundLayerMask;
    Character character;
    bool facingWall;

    void Awake() {
        character = transform.parent.GetComponent<Character>();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == GroundLayerMask) {
            facingWall = true;
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.layer == GroundLayerMask) {
            facingWall = false;
        }
    }

    void Update() {
        character.LedgeDetected = !facingWall && Physics2D.OverlapCircle(transform.position, radius, GroundLayerMask);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}