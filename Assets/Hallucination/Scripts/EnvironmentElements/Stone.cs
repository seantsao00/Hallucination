using UnityEngine;

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
        float direction = InputManager.Instance.CharacterHorizontalMove;
        transform.position = transform.position + new Vector3(direction * horizontalGrabSpeed * Time.deltaTime, 0, 0);
    }

    public void Leash() {
        isLeashed = true;
    }

    public void Unleash() {
        isLeashed = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
}
