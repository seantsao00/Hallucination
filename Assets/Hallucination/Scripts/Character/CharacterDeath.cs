using System.Collections;
using UnityEngine;

public class CharacterDeath : MonoBehaviour {
    public ParticleSystem deathParticles;

    // Call this method when the character takes damage
    public void TakeDamage() {
        if (GetComponent<CharacterStateController>().HasState(CharacterState.Dead)) return;

        Die();
    }

    void Die() {
        GetComponent<CharacterStateController>().AddState(CharacterState.Dead);
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.AllInputDisabled;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<SpriteRenderer>().enabled = false;

        Instantiate(deathParticles, transform.position, Quaternion.identity).Play();

        StartCoroutine(DelayedRestartLevel());
    }

    IEnumerator DelayedRestartLevel() {
        yield return new WaitForSeconds(0.5f);
        LevelNavigator.Instance.RestartCurrentLevel();
    }

    // Detect when the character falls into the DeathZone
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("DeathZone")) {
            TakeDamage();
        }
    }
}
