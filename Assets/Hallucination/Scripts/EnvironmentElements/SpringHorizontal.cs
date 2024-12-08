using UnityEngine;

public class SpringHorizontal : Spring {
    [SerializeField] Vector2 speed;
    [SerializeField] float duration = 0.5f;

    protected override void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            base.OnTriggerEnter2D(other);
            characterStateController.AddState(CharacterState.HorizontalSpringFlying);
            float windBonusSpeed = other.GetComponent<CharacterHorizontalMove>().WindBonusSpeed;
            Vector2 targetSpeed;
            if (Mathf.Sign(windBonusSpeed) == Mathf.Sign(speed.x)) {
                targetSpeed = new Vector2(windBonusSpeed + speed.x, speed.y);
            } else {
                targetSpeed = new Vector2(speed.x, speed.y);
            }
            Bounds bounds = GetComponent<Collider2D>().bounds;
            Vector2 springStartPosition = new Vector2(bounds.min.x + 0.5f, bounds.max.y);
            character.GainSpringHorizontalSpeed(springStartPosition, targetSpeed, duration);
        }
    }


}
