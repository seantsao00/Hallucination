using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.EventSystems;

public enum Language {
    English,
    Chinese
}

public class LocaleSelector : MonoBehaviour {
    int localeIndex;
    bool isChangingLocale = false;
    [SerializeField] GameObject EnglishButton;
    [SerializeField] GameObject ChineseButton;

    void Awake() {
        gameObject.SetActive(false);
        localeIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        // Debug.Log(LocalizationSettings.SelectedLocale);
    }

    void OnEnable() {
        switch (localeIndex) {
            case (int)Language.English:
                EventSystem.current.SetSelectedGameObject(EnglishButton);
                break;
            case (int)Language.Chinese:
                EventSystem.current.SetSelectedGameObject(ChineseButton);
                break;
        }
    }
    
    void OnDisable() {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectLocale(int localeIndex) {
        if (isChangingLocale) return;
        StartCoroutine(ChangeLocale(localeIndex));
    }

    IEnumerator ChangeLocale(int localeIndex) {
        isChangingLocale = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        isChangingLocale = false;
    }
}
