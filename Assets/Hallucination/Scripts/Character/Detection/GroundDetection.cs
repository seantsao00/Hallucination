using UnityEngine;

public class GroundDetection : MonoBehaviour {
    [SerializeField] float radius = 0.2f;
    [SerializeField] LayerMask groundLayerMask;
    CharacterStateController characterStateController;

    void Awake() {
        characterStateController = transform.parent.GetComponent<CharacterStateController>();
    }

    void Update() {
        bool isGrounded = Physics2D.OverlapCircle(transform.position, radius, groundLayerMask);
        if (isGrounded) characterStateController.AddState(CharacterState.StandingOnGround);
        else characterStateController.RemoveState(CharacterState.StandingOnGround);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
