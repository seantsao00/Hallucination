using System.Collections;
using UnityEngine;

public class CapturedSurroundings : MonoBehaviour {
    SpriteRenderer spriteRenderer;

    void Awake() {
        //WorldSwitchManager.Instance.WorldSwitched.AddListener(UpdateSprite);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Activate(float activateDuration) {
        UpdateSprite();
        StartCoroutine(HandleSpriteFade(activateDuration));
        spriteRenderer.color = new Color(1, 1, 1, 0.2f);
        spriteRenderer.enabled = true;
    }
    IEnumerator HandleSpriteFade(float activateDuration) {
        float passedTime = 0;
        float startFadeTime = activateDuration - 1;
        while (passedTime < activateDuration) {
            passedTime += Time.deltaTime;
            if (passedTime > startFadeTime) {
                spriteRenderer.color = new Color(1, 1, 1, 0.2f * (activateDuration -  passedTime));
            }
            yield return null;
        }
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
