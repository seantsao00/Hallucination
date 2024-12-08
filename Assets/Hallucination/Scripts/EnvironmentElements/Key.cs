using UnityEngine;

public class Key : MonoBehaviour {
    bool isCollected;

    public delegate void UnlockHandler(Key key);
    public event UnlockHandler Unlock;
    [SerializeField] Color eatenColor = Color.green;

    void OnTriggerEnter2D(Collider2D collision) {
        if (isCollected) return;
        if (collision.CompareTag("Player")) {
            Collect();
        }
    }

    void Collect() {
        isCollected = true;
        Unlock?.Invoke(this);
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = eatenColor;
    }
}
