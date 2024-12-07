using System;
using UnityEngine;

public class CharacterHorizontalMove : MonoBehaviour {
    CharacterStateController characterStateController;
    Rigidbody2D rb;
    [SerializeField] float normalSpeed = 5f;
    [SerializeField] float grabSpeed = 3f;
    public float CurrentBasicSpeed { get; private set; }
    [HideInInspector] public Spring CurrentSpring;
    [HideInInspector] public float WindBonusSpeed;
    [SerializeField] LayerMask groundLayer;

    public void ResetBasicSpeed() => CurrentBasicSpeed = normalSpeed;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        characterStateController = GetComponent<CharacterStateController>();
        CurrentBasicSpeed = normalSpeed;
        characterStateController.OnStateChanged += HandleStateChange;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0) {
            foreach (ContactPoint2D contact in collision.contacts) {
                float angle = Vector2.Angle(contact.normal, Vector2.up);
                if (CurrentSpring != null && Mathf.Approximately(angle, 90)) {
                    CurrentSpring.StopSpringHorizontalSpeed();
                    CurrentSpring = null;
                    break;
                }
            }
        }
    }

    void HandleStateChange(CharacterState state, bool added) {
        if (characterStateController.HasState(CharacterState.Grabbing)) {
            CurrentBasicSpeed = grabSpeed;
            return;
        }
        CurrentBasicSpeed = normalSpeed;
    }

    void FixedUpdate() {
        if (rb.bodyType == RigidbodyType2D.Static) return;
        if (
            !InputManager.Control.Character.HorizontalMove.enabled ||
            characterStateController.HasState(CharacterState.HorizontalSpringFlying)
        ) {
            characterStateController.RemoveState(CharacterState.Walking);
            return;
        }
        float direction = InputManager.Instance.CharacterHorizontalMove;
        rb.velocity = new Vector2(direction * CurrentBasicSpeed + WindBonusSpeed, rb.velocity.y);
        if (direction == 0) {
            characterStateController.RemoveState(CharacterState.Walking);
        } else {
            characterStateController.AddState(CharacterState.Walking);
        }
    }

    void OnDestroy() {
        characterStateController.OnStateChanged -= HandleStateChange;
    }
}
