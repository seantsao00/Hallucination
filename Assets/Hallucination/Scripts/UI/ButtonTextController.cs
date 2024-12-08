using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class ButtonTextController : MonoBehaviour {
    Color notSelectedColor = new Color(0, 0, 0, 50);
    Color selectedColor = new Color(155, 126, 195, 180);
    public TextMeshProUGUI buttonText;
    void Update() {
        HandleTextColor();
    }

    void HandleTextColor() {
        if (EventSystem.current.currentSelectedGameObject == gameObject) {
            buttonText.color = selectedColor;
        } else {
            buttonText.color = notSelectedColor;
        }
    }
}