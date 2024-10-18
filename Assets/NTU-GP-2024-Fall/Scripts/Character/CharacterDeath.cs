using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterDeath : MonoBehaviour {
    // public AudioClip deathSound;
    // public ParticleSystem deathParticles;
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

        // if (deathSound != null) {
        //     audioSource.PlayOneShot(deathSound);
        // }

        // Optional: Play death particle effects
        // if (deathParticles != null) {
        //     Instantiate(deathParticles, transform.position, Quaternion.identity);
        // }

        // Optional: Disable character movement or other components
        GetComponent<Character>().IsDead = true;

        // Reset the scene after a short delay
        Invoke(nameof(ResetScene), deathDelay);
    }

    void ResetScene() {
        // Reload the current scene to reset everything
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
