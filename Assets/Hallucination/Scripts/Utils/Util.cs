using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Localization.Settings;

public class Util {
    static public GameObject GetFairyObjects() {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        foreach (var characterObject in searchResults) {
            if (characterObject.name == "Fairy") return characterObject;
        }
        Debug.LogError("Can't find Fairy Object");
        return null;
    }
    static public GameObject GetBearObjects() {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        foreach (var characterObject in searchResults) {
            if (characterObject.name == "Bear") return characterObject;
        }
        Debug.LogError("Can't find Bear Object");
        return null;
    }
    static public GameObject FindCurrentPlayedCharacter() {
        GameObject[] searchResults = GameObject.FindGameObjectsWithTag("Player");
        Assert.IsTrue(searchResults.Length == 1);
        GameObject currentPlayedCharacter = searchResults[0];
        return currentPlayedCharacter;
    }

    static public IEnumerator FadeIn(float fadeDuration, CanvasGroup FadeMask) {
        float fadeSpeed = 1f / fadeDuration;

        for (float t = 1; t > 0; t -= Time.deltaTime * fadeSpeed) {
            FadeMask.alpha = t;
            yield return null;
        }
    }

    static public IEnumerator FadeOut(float fadeDuration, CanvasGroup FadeMask) {
        float fadeSpeed = 1f / fadeDuration;

        for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed) {
            FadeMask.alpha = t;
            yield return null;
        }
    }

    static public IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float fadeDuration, Action callback = null) {
        canvasGroup.alpha = 0f;
        float fadeSpeed = 1f / fadeDuration;
        for (float t = 0f; t < 1f; t += Time.deltaTime * fadeSpeed) {
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        callback?.Invoke();
    }

    static public Language CurrentLanguage() {
        int localeIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        switch (localeIndex) {
            case (int)Language.English:
                return Language.English;
            case (int)Language.Chinese:
                return Language.Chinese;
            default:
                return Language.English;
        }
    }
}