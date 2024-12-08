using UnityEngine;

public class SpringVertical : Spring {
    [SerializeField] float speed;

    protected override void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            base.OnTriggerEnter2D(other);
            characterRb.velocity = new Vector2(characterRb.velocity.x, speed);
        }
    }
}
