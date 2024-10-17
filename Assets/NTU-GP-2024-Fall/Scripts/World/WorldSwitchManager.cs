using UnityEngine;
using Cinemachine;
using System.Collections;

public class WorldSwitchManager : MonoBehaviour {
    public GameObject WorldFairyEnvironment;
    public GameObject WorldBearEnvironment;
    public CanvasGroup FadeCanvasGroup;

    private bool isInWorldFairy = true;
    private bool disabled = false;

    void Start() {
        ActivateWorldFairy();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.X) && !disabled) {
            StartCoroutine(SwitchWorldsWithFade());
        }
    }

    IEnumerator SwitchWorldsWithFade() {
        yield return StartCoroutine(FadeOut());

        isInWorldFairy = !isInWorldFairy;
        if (isInWorldFairy) {
            ActivateWorldFairy();
        } else {
            ActivateWorldBear();
        }

        yield return StartCoroutine(FadeIn());
    }

    void ActivateWorldFairy() {
        WorldFairyEnvironment.SetActive(true);
        WorldBearEnvironment.SetActive(false);
    }

    void ActivateWorldBear() {
        WorldFairyEnvironment.SetActive(false);
        WorldBearEnvironment.SetActive(true);
    }

    public void Enable() {
        disabled = false;
    }

    public void Disable() {
        disabled = true;
    }

    IEnumerator FadeOut() {
        float fadeDuration = 0.4f;
        float fadeSpeed = 1f / fadeDuration;

        for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed) {
            FadeCanvasGroup.alpha = t;
            yield return null;
        }
        FadeCanvasGroup.alpha = 1;
    }

    IEnumerator FadeIn() {
        float fadeDuration = 0.4f;
        float fadeSpeed = 1f / fadeDuration;

        for (float t = 1; t > 0; t -= Time.deltaTime * fadeSpeed) {
            FadeCanvasGroup.alpha = t;
            yield return null;
        }
        FadeCanvasGroup.alpha = 0;
    }

    
}
