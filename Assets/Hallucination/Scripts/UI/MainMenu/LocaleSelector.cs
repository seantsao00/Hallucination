using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.EventSystems;
using System.Xml.Serialization;

public enum Language {
    English,
    Chinese
}

public class LocaleSelector : MonoBehaviour {
    bool isChangingLocale = false;
    [SerializeField] GameObject EnglishButton;
    [SerializeField] GameObject ChineseButton;

    void Awake() {
        gameObject.SetActive(false);
    }
    void OnEnable() {
        switch (Util.CurrentLanguage()) {
            case Language.English:
                EventSystem.current.SetSelectedGameObject(EnglishButton);
                break;
            case Language.Chinese:
            default:
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
        Debug.Log("Changing Locale");
        isChangingLocale = true;

        while (!LocalizationSettings.InitializationOperation.IsDone) {
            yield return null;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];

        PlayerPrefs.SetString("Locale", LocalizationSettings.SelectedLocale.Identifier.Code);
        PlayerPrefs.Save();

        isChangingLocale = false;
    }

    
}
