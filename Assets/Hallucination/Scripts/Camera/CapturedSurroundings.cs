using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturedSurroundings : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
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

    void UpdateSprite()
    {
        Texture2D newTexture = SurroundingCapturer.Instance.cachedTexture;
        print("updated!");
        if (newTexture == null)
        {
            Debug.LogError("New texture is null!");
            return;
        }
        
        
        // Create a new sprite from the Texture2D
        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(0, 0, newTexture.width, newTexture.height),
            new Vector2(0.5f, 0.5f) // pivot
        );

        
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found!");
        }
    }
}
