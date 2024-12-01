using UnityEngine;

public class ClimbableCheck : MonoBehaviour {
    [SerializeField] LayerMask climbableLayerMask;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] Transform bodyCheck;
    [SerializeField] float bodyCheckSize = 0.2f;
    [SerializeField] Transform bottomCheck;
    [SerializeField] float bottomCheckSize = 0.2f;
    [SerializeField] Transform footCheck;
    [SerializeField] Vector2 footCheckSize;
    [SerializeField] Transform bellyCheck;
    [SerializeField] Vector2 bellyCheckSize;
    Character character;

    void Awake() {
        character = transform.parent.GetComponent<Character>();
    }

    void Update() {
        character.IsBodyOnClimbable = Physics2D.OverlapCircle(bodyCheck.position, bodyCheckSize, climbableLayerMask);
        character.IsStandOnClimbable = Physics2D.OverlapCircle(bottomCheck.position, bottomCheckSize, climbableLayerMask);
        character.IsFootOnGround = Physics2D.OverlapBox(footCheck.position, footCheckSize, 0, groundLayerMask);
        character.IsBellyInGround = Physics2D.OverlapBox(bellyCheck.position, bellyCheckSize, 0, groundLayerMask);
        // Debug.Log("Log: " + character.IsFootOnGround + character.IsBellyInGround);
    }

    void OnDrawGizmosSelected() {
        if (bodyCheck == null || bottomCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(bodyCheck.position, bodyCheckSize);
        Gizmos.DrawWireSphere(bottomCheck.position, bottomCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(footCheck.position, footCheckSize);
        Gizmos.DrawWireCube(bellyCheck.position, bellyCheckSize);
    }
}