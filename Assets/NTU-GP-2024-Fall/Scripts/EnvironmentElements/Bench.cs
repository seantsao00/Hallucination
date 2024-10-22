using UnityEngine;
using UnityEngine.UI;  // For handling UI elements
using TMPro;

public class Bench : MonoBehaviour {
    public TipManager tipManager;
    [SerializeField] string tip = "Press Up or W to sit";
    Character character;
    /// <summary>
    /// If a character is sitting on the bench.
    /// </summary>
    bool IsBeingSat = false;

    private void Start() {
    }

    // Detect when the player enters the checkpoint area
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            character = other.gameObject.GetComponent<Character>();
            tipManager.ShowTip(true, tip);
        }
    }

    // Detect when the player exits the checkpoint area
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            tipManager.ShowTip(false);
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
        character.CurrentState = new CharacterState.SittingOnBench();
        tipManager.ShowTip(false);
    }

    private void LeaveBench() {
        IsBeingSat = false;
        character.CurrentState = new CharacterState.Free();
        tipManager.ShowTip(true);
    }
}
