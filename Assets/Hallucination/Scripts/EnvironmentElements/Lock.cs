using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Lock : MonoBehaviour {
    [SerializeField] Key[] keys;
    [SerializeField] Transform unlockedPosition;
    int numberOfKeys, numberOfCollectedKeys;
    [SerializeField] float slowDownLength = 1f;
    [SerializeField] float minSpeedRatio = 0.33f;
    [SerializeField] float speed = 5f;
    float eps = 1e-4f;

    void Awake() {
        numberOfKeys = keys.Length;
        numberOfCollectedKeys = 0;
        foreach (Key key in keys) {
            key.Unlock += UnlockHandler;
        }
    }

    void UnlockHandler(Key key) {
        numberOfCollectedKeys++;
        if (numberOfCollectedKeys == numberOfKeys) {
            StartCoroutine(UnlockCoroutine());
        }
    }

    IEnumerator UnlockCoroutine() {
        WorldSwitchManager.Instance.Lock(gameObject);
        Vector3 target = unlockedPosition.position;
        Vector3 offset = target - transform.position;
        while (offset.magnitude > eps) {
            target = unlockedPosition.position;
            offset = target - transform.position;
            WorldSwitchManager.Instance.Lock(gameObject);
            Vector3 Movement;
            if (offset.magnitude > slowDownLength) {
                Movement = Vector3.ClampMagnitude(offset, speed * Time.deltaTime);
            } else {
                float ratio = Mathf.Max(offset.magnitude / slowDownLength, minSpeedRatio);
                Movement = Vector3.ClampMagnitude(offset, speed * Time.deltaTime * ratio);
            }
            transform.position += Movement;
            yield return null;
        }
        WorldSwitchManager.Instance.Unlock(gameObject);
    }
}