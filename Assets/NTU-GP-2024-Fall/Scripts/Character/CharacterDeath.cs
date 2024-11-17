using UnityEngine;

public class CharacterDeath : MonoBehaviour {
    // public AudioClip deathSound;
    public ParticleSystem deathParticles;
    public float deathDelay = 2.0f;
    bool isDead = false;
    AudioSource audioSource;

    void Start() {
        // audioSource = GetComponent<AudioSource>();
    }

    // Call this method when the character takes damage
    public void TakeDamage() {
        if (!isDead) Die();
    }

    void Die() {
        isDead = true;

        // Optional: Play death sound
        // if (deathSound != null) {
        //     audioSource.PlayOneShot(deathSound);
        // }

        if (deathParticles != null) {
            Instantiate(deathParticles, transform.position, Quaternion.identity).Play();
        }

        GetComponent<Character>().IsDead = true;

        Invoke(nameof(ResetScene), deathDelay);
    }

    void ResetScene() {
        LevelNavigator.Instance.RestartCurrentLevel();
    }

    // Detect when the character falls into the DeathZone
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("DeathZone")) {
            TakeDamage();
        }
    }
}
