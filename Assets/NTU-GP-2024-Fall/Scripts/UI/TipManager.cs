using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TipManager : MonoBehaviour {
    public GameObject bottomTip;

    // Start is called before the first frame update
    void Start() {
        bottomTip.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

    public void ShowTip(bool show, string tipText = "") {
        bottomTip.SetActive(show);
        TextMeshProUGUI tipTextComponent = bottomTip.GetComponentInChildren<TextMeshProUGUI>();
        if (tipTextComponent != null) tipTextComponent.enabled = show;
        if (show) {
            if (tipTextComponent != null) {
                tipTextComponent.text = tipText;
            } else {
                Debug.LogError("No Text component found in bottomTip GameObject.");
            }
        }
    }
}

