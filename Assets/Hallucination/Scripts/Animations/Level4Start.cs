using System;
using System.Collections;
using UnityEngine;

public class Level4Start : MonoBehaviour {
    [Serializable]
    public class AnimationObjects {
        public Transform body;
        public Transform[] points;
    }
    [SerializeField] string dialogueName = "Level 4 Start";
    [SerializeField] AnimationObjects train;
    [SerializeField] float trainSpeed = 8f;
    [SerializeField] AnimationObjects bear, fairy;
    float eps = 1e-4f;

    public void StartAnimation() {
        StartCoroutine(Animation());
    }

    IEnumerator Animation() {
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Cinematic;
        while (!Move(train.body, train.points[0], 6f, 4f)) {
            yield return null;
        }
        StartCoroutine(FairyAnimation());
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(BearAnimation());
        // Debug.Log("Animation Finished");
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
        bear.body.gameObject.SetActive(false);
        WorldSwitchManager.Instance.Bear.transform.position = bear.points[bear.points.Length - 1].position;
        DialogueManager.Instance.StartDialogue(dialogueName);
    }

    IEnumerator FairyAnimation() {
        Animator animator = fairy.body.GetComponent<Animator>();
        animator.SetBool("Movement", true);
        foreach (var point in fairy.points) {
            while (!Move(fairy.body, point, 3f)) {
                yield return null;
            }
        }
        animator.SetBool("Movement", false);
    }

    IEnumerator BearAnimation() {
        Animator animator = bear.body.GetComponent<Animator>();
        animator.SetBool("Movement", true);
        animator.speed = 0.5f;
        foreach (var point in bear.points) {
            while (!Move(bear.body, point, 3f)) {
                yield return null;
            }
        }
        animator.SetBool("Movement", false);
    }

    bool Move(Transform body, Transform target, float speed, float slowDownLength = 0f, float minSpeedRatio = 0.33f) {
        Vector3 offset = target.position - body.position;
        Vector3 Movement;
        if (offset.magnitude > slowDownLength) {
            Movement = Vector3.ClampMagnitude(offset, speed * Time.deltaTime);
        } else {
            float ratio = Mathf.Max(offset.magnitude / slowDownLength, minSpeedRatio);
            Movement = Vector3.ClampMagnitude(offset, speed * Time.deltaTime * ratio);
        }
        body.transform.position += Movement;
        return (target.position - body.position).magnitude < eps;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        foreach (AnimationObjects obj in new AnimationObjects[] { train, bear, fairy }) {
            if (obj?.points.Length < 1) continue;
            Gizmos.DrawLine(obj.body.position, obj.points[0].position);
            for (int i = 0; i < obj.points.Length - 1; i++) {
                Gizmos.DrawLine(obj.points[i].position, obj.points[i + 1].position);
            }
        }
    }
}