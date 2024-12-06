using UnityEngine;

public class CapturedSurroundings : MonoBehaviour {
    SpriteRenderer spriteRenderer;

    void Awake() {
        //WorldSwitchManager.Instance.WorldSwitched.AddListener(UpdateSprite);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Activate() {
        UpdateSprite();
        spriteRenderer.enabled = true;
    }

    public void Deactivate() {
        spriteRenderer.enabled = false;
    }

    void UpdateSprite() {
        Texture2D newTexture = SurroundingCapturer.Instance.cachedTexture;
        if (newTexture == null) {
            Debug.LogError("New texture is null!");
            return;
        }


        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(0, 0, newTexture.width, newTexture.height),
            new Vector2(0.5f, 0.5f)
        );


        if (spriteRenderer != null) {
            spriteRenderer.sprite = newSprite;
        } else {
            Debug.LogError("SpriteRenderer component not found!");
        }
    }
}
