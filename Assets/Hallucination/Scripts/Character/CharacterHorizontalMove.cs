using UnityEngine;
using System.Collections;

public class CharacterHorizontalMove : MonoBehaviour {
    CharacterStateController characterStateController;
    Rigidbody2D rb;
    [SerializeField] float normalSpeed = 5f;
    public float CurrentBasicSpeed;
    public float SpringBonusSpeed;
    public Spring CurrentSpring;
    public float WindBonusSpeed;
    float bonusSpeed => SpringBonusSpeed + WindBonusSpeed;
    [SerializeField] LayerMask groundLayer;

    public void ResetBasicSpeed() => CurrentBasicSpeed = normalSpeed;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
        CurrentBasicSpeed = normalSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0) {
            foreach (ContactPoint2D contact in collision.contacts) {
                float angle = Vector2.Angle(contact.normal, Vector2.up);
                if (CurrentSpring != null && Mathf.Approximately(angle, 90)) {
                    // Debug.Log("side hit");
                    CurrentSpring.StopSpringHorizontalBonus();
                    CurrentSpring = null;
                    break;
                }
            }
        }
    }

    void Update() {
        if (!InputManager.Control.Character.HorizontalMove.enabled) {
            characterStateController.RemoveState(CharacterState.Walking);
            return;
        }
        float direction = InputManager.Instance.CharacterHorizontalMove;
        rb.velocity = new Vector2(direction * CurrentBasicSpeed + bonusSpeed, rb.velocity.y);
        if (direction == 0) {
            characterStateController.RemoveState(CharacterState.Walking);
        } else {
            characterStateController.AddState(CharacterState.Walking);
        }
    }
}
