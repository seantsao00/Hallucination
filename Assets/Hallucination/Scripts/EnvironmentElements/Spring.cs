using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour {
    [SerializeField] float verticalLaunchSpeed;
    [SerializeField] float horizontalLaunchSpeed;
    [SerializeField] float springFullSpeedDuration = 0.2f;
    [SerializeField] float springFadeDuration = 0.4f;
    Rigidbody2D rb;
    CharacterHorizontalMove horizontalMove;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            rb = other.GetComponent<Rigidbody2D>();
            horizontalMove = other.GetComponent<CharacterHorizontalMove>();
            StartCoroutine(LaunchSpring());
        }
    }

    IEnumerator LaunchSpring() {
        rb.velocity = new Vector2(rb.velocity.x, verticalLaunchSpeed);
        horizontalMove.SpringBonusSpeed = horizontalLaunchSpeed;
        yield return new WaitForSeconds(springFullSpeedDuration);
        for (float t = 0f; t < springFadeDuration; t += Time.deltaTime) {
            horizontalMove.SpringBonusSpeed = Mathf.Lerp(horizontalLaunchSpeed, 0, t / springFadeDuration);
            yield return null;
        }
        horizontalMove.SpringBonusSpeed = 0;
    }
}
