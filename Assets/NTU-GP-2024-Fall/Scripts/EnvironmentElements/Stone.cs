using UnityEngine;
using System;
using UnityEngine.UI;  // For handling UI elements
using TMPro;

public class Stone : MonoBehaviour {
    Rigidbody2D rb;
    float horizontalGrabSpeed;
    bool isLeashed;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetSpeed(float horizontalGrabSpeed) {
        this.horizontalGrabSpeed = horizontalGrabSpeed;
    }

    void Update() {
        if (!isLeashed) return;
        float direction = InputManager.Instance.Character.HorizontalMove;
        rb.velocity = new Vector2(direction * horizontalGrabSpeed, rb.velocity.y);
    }

    public void Leash() {
        isLeashed = true;
    }

    public void Unleash() {
        isLeashed = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
}
