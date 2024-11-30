using UnityEngine;
using UnityEngine.Tilemaps;

public class Bubble : Syncable {
    // We should make it glow by Light and modify related code.
    // Here, we just make a simpler one by change its sprite color first.
    bool isGlowing;
    SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void turnOn() {
        isGlowing = true;
        spriteRenderer.color = Color.white;
    }
    public void turnOff() {
        isGlowing = false;
        spriteRenderer.color = Color.black;
    }

    override public void SyncState() {
        if (isGlowing) {
            syncedObject.GetComponent<Tilemap>().color = new Color(1f, 1f, 1f, 1f);
            syncedObject.GetComponent<Collider2D>().enabled = true;
        } else {
            syncedObject.GetComponent<Tilemap>().color = new Color(1f, 1f, 1f, 0.4f);
            syncedObject.GetComponent<Collider2D>().enabled = false;
        }
    }

}
