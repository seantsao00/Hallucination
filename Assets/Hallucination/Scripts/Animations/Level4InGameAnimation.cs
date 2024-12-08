using System.Collections;
using UnityEngine;

public class Level4InGameAnimation : MonoBehaviour {
    public Transform fairy;
    public Transform[] points;
    float speed = 5f;
    float eps = 1e-4f;

    public void StartAnimation() {
        fairy.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(Animation());
    }

    IEnumerator Animation() {
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Cinematic;
        // wait for sync complete
        // Very ugly implementation but the ddl is approaching
        yield return null;
        yield return null;
        fairy.GetComponent<SpriteRenderer>().enabled = true;
        fairy.position = points[0].position;
        while (!Move(fairy, points[1], speed, 0f)) {
            yield return null;
        }
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
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
        if (points.Length < 2) return;
        for (int i = 0; i < points.Length - 1; i++) {
            Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }
    }
}
