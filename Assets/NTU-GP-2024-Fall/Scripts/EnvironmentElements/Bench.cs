using UnityEngine;
using UnityEngine.UI;  // For handling UI elements
using TMPro;

public class Bench : MonoBehaviour {
    public TextMeshProUGUI tipText;
    Character character;
    /// <summary>
    /// If a character is sitting on the bench.
    /// </summary>
    bool IsBeingSat = false;

    private void Start() {
        // Hide the tip text at the start
        tipText.enabled = false;
    }

    // Detect when the player enters the checkpoint area
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            tipText.text = "Press Up or W to sit";
            character = other.gameObject.GetComponent<Character>();
            tipText.enabled = true;
        }
    }

    // Detect when the player exits the checkpoint area
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            tipText.enabled = false;
            character = null;
        }
    }

    private void Update() {
        if (IsBeingSat) {
            if (character != null && (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))) {
                LeaveBench();
            }
        } else {
            if (character != null && Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0) {
                SitOnBench();
            }
        }
    }

    private void SitOnBench() {
        IsBeingSat = true;
        character.IsSittingOnBench = true;
        tipText.enabled = false;
    }

    private void LeaveBench() {
        IsBeingSat = false;
        character.IsSittingOnBench = false;
        tipText.enabled = true;
    }
}
