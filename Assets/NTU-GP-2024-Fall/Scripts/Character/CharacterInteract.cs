using UnityEngine;


public class CharacterInteract : MonoBehaviour {
    Character character;
    [SerializeField] float grabbingStoneSpeed = 2f;

    void Awake() {}
    void Start() {
        character = GetComponent<Character>();
    }

    void Update() {
        GameObject movableGameObject = character.FacedMovableGameObject;
        if (movableGameObject == null) return;
        Stone stone = movableGameObject.GetComponent<Stone>();
        if (stone != null) {
            if (Input.GetButton("Interact")) {
                float horizontalGrabDirection = 2*Input.GetAxisRaw("Horizontal");
                stone.HorizontalMove(horizontalGrabDirection);
            } else {
                stone.HorizontalMove(0);
            }
            return;
        }
    }


}
