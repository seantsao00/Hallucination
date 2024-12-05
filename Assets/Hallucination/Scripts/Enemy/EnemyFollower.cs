using UnityEngine;

public class EnemyFollower : MonoBehaviour {
    public CharacterTracker characterTracker;
    private readonly float followSpeed = 2f;

    void Update() {
        Vector3 targetPosition = characterTracker.GetOldestPosition();

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
