using UnityEngine;

public class GroundDetection : MonoBehaviour {
    [SerializeField] float radius = 0.2f;
    [SerializeField] LayerMask groundLayerMask;
    Character character;

    void Awake() {
        character = transform.parent.GetComponent<Character>();
    }

    void Update() {
        character.IsGrounded = Physics2D.OverlapCircle(transform.position, radius, groundLayerMask);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
