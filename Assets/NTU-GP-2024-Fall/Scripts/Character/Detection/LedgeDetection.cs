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
        Debug.Log($"{collider.gameObject.layer.ToString()} Entered");
        if (((1 << collider.gameObject.layer) & GroundLayerMask) != 0) {
            Debug.Log("Trigger Entered");
            facingWall = true;
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        Debug.Log($"{collider.gameObject.layer.ToString()} Exited");
        if (((1 << collider.gameObject.layer) & GroundLayerMask) != 0) {
            Debug.Log("Trigger Exited");
            facingWall = false;
        }
    }

    void Update() {
        character.IsLedgeDetected = !facingWall && Physics2D.OverlapCircle(transform.position, radius, GroundLayerMask);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
