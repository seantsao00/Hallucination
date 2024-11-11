using UnityEngine;
using UnityEngine.UI;  // For handling UI elements
using TMPro;

public class Bench : MonoBehaviour {
    [SerializeField] TipManager tipManager;
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
        float horizontal = InputManager.Instance.Character.HorizontalMove;
        float vertical = InputManager.Instance.Character.VerticalMove;
        if (IsBeingSat) {
            if (character != null && (horizontal != 0 || vertical != 0)) {
                LeaveBench();
            }
        } else {
            if (character != null && vertical > 0) {
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
        tipManager.ShowTip(true, tip);
    }
}
