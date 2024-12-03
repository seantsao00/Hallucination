using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.InputSystem.Interactions;
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

    static Language cachedLanguage;
    static bool isInitialized = false;

    static public IEnumerator InitializeLocalizationAsync() {
        while (!LocalizationSettings.InitializationOperation.IsDone) {
            Debug.Log("Still initializing");
            yield return null;
            
        }
        Debug.Log("Initialized");
        int localeIndex = PlayerPrefs.GetInt(
            "Locale",
            LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale)
        );

        cachedLanguage = localeIndex switch {
            (int)Language.English => Language.English,
            (int)Language.Chinese => Language.Chinese,
            _ => Language.English,
        };
        isInitialized = true;
    }

    static public Language CurrentLanguage() {
        if (!isInitialized) {
            Debug.LogError("Localization system is not initialized yet. Call InitializeLocalizationAsync first.");
            return Language.English;
        }

        return cachedLanguage;
    }

}
