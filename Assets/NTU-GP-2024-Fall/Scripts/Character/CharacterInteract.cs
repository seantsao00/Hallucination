using UnityEngine;


public class CharacterInteract : MonoBehaviour {
    Character character;

    void Awake() { }
    void Start() {
        character = GetComponent<Character>();
    }

    void Update() {
        GameObject movableGameObject = character.FacedMovableGameObject;
        if (movableGameObject == null) return;
        Stone stone = movableGameObject.GetComponent<Stone>();
        if (stone != null) {
            if (Input.GetButton("Interact")) {
                character.CurrentState = new CharacterState.GrabbingMovable();
                float horizontalGrabDirection = character.GrabbingStoneSpeed * Input.GetAxisRaw("Horizontal");
                stone.HorizontalMove(horizontalGrabDirection);
            } else {
                character.CurrentState = new CharacterState.Free();
                stone.HorizontalMove(0);
            }
            return;
        }
    }


}
