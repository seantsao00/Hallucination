using UnityEngine;

public class ClimbableCheck : MonoBehaviour {
    [SerializeField] LayerMask climbableLayerMask;
    [SerializeField] Transform bodyCheck;
    [SerializeField] float bodyCheckSize = 0.2f;
    [SerializeField] Transform bottomCheck;
    [SerializeField] float bottomCheckSize = 0.2f;
    Character character;

    void Awake() {
        character = transform.parent.GetComponent<Character>();
    }

    void Update() {
        character.IsBodyOnClimbable = Physics2D.OverlapCircle(bodyCheck.position, bodyCheckSize, climbableLayerMask);
        character.IsStandOnClimbable = Physics2D.OverlapCircle(bottomCheck.position, bottomCheckSize, climbableLayerMask);
    }

    void OnDrawGizmosSelected() {
        if (bodyCheck == null || bottomCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(bodyCheck.position, bodyCheckSize);
        Gizmos.DrawWireSphere(bottomCheck.position, bottomCheckSize);
    }
}