using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCoverController : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }
    IEnumerator FadeIn() {
        while (true) {
            Color originalColor = gameObject.GetComponent<SpriteRenderer>().color;
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, 
                                        Mathf.Clamp01(originalColor.a + Time.deltaTime * 2));
            gameObject.GetComponent<SpriteRenderer>().color = newColor;
            if (newColor.a >= 1) {
                break;
            }
            yield return null;
        }
    }
    IEnumerator FadeOut() {
        while (true) {
            Color originalColor = gameObject.GetComponent<SpriteRenderer>().color;
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b,
                                        Mathf.Clamp01(originalColor.a - Time.deltaTime * 2));
            gameObject.GetComponent<SpriteRenderer>().color = newColor;
            if (newColor.a <= 0) {
                break;
            }
            yield return null;
        }
    }
}
