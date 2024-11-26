using UnityEngine;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour {
    [SerializeField] TextMeshProUGUI loadingText;
    void Awake() {
        gameObject.SetActive(false);
    }

    public void ShowLoadingScreen() {
        gameObject.SetActive(true);
        StartCoroutine(StartLoadingAnimation());
    }

    IEnumerator StartLoadingAnimation() {
        while (true) {
            loadingText.text = "Loading";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "Loading..";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "Loading...";
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnDestroy() {
        StopAllCoroutines();
    }
}