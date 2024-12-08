using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class levelSelector : MonoBehaviour
{
    [SerializeField] GameObject firstLevelButton;
    void Awake() {
        gameObject.SetActive(false);
    }

    void OnEnable() {
        EventSystem.current.SetSelectedGameObject(firstLevelButton);
        
    }
    public void SelectLevel(int levelIndex) {
        LevelNavigator.SetFirstPlay(true);
        LevelNavigator.SetStartLevelIndex(levelIndex);
    }
}
