using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Util {
    static public IEnumerator FadeOutCanvasGroup(float fadeDuration, CanvasGroup FadeMask) {
        float fadeSpeed = 1f / fadeDuration;

        for (float t = 1; t > 0; t -= Time.deltaTime * fadeSpeed) {
            FadeMask.alpha = t;
            yield return null;
        }
    }

    static public IEnumerator FadeInCanvasGroup(float fadeDuration, CanvasGroup FadeMask) {
        float fadeSpeed = 1f / fadeDuration;

        for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed) {
            FadeMask.alpha = t;
            yield return null;
        }
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
