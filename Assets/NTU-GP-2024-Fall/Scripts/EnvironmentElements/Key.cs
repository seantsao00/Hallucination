using UnityEngine;

public interface Lockable {
    bool checkIfLocked(Key[] keys);
    void Unlock();
}

public class Key : MonoBehaviour {
    [HideInInspector] public bool IsLocked;

    void Start() {
        IsLocked = true;
    }

    void OnTriggerEnter2D() {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(0f, 255f, 0f, 1f);
        IsLocked = false;
    }
}
