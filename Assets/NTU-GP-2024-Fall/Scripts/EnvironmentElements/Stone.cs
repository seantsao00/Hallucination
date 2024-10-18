using UnityEngine;
using System;
using UnityEngine.UI;  // For handling UI elements
using TMPro;

public class Stone : MonoBehaviour {
    Rigidbody2D rb;
    float horizontalGrabSpeed;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void HorizontalMove(float horizontalGrabSpeed) {
        this.horizontalGrabSpeed = horizontalGrabSpeed;
    }

    void FixedUpdate() {
        rb.velocity = new Vector2(horizontalGrabSpeed, rb.velocity.y);
    }
}
