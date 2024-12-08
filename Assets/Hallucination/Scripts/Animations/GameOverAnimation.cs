using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverAnimation : MonoBehaviour {
    public Transform bear;
    public Transform bearDestination;
    public Transform fairy;
    public Transform[] points;
    float speed = 5f;
    float eps = 1e-4f;
    string endGameSceneName = "EndGame";
    string dialogueName = "Level 4 Complete";

    public void StartAnimation() {
        WorldSwitchManager.Instance.Bear.GetComponent<Character>().StopMotion();
        WorldSwitchManager.Instance.Bear.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        WorldSwitchManager.Instance.Fairy.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        WorldSwitchManager.Instance.Bear.GetComponent<SpriteRenderer>().enabled = false;
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
        yield return StartCoroutine(BearMove(bearDestination));
        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
        DialogueManager.Instance.StartDialogueWithCallback(dialogueName, () => SceneManager.LoadScene(endGameSceneName));
    }

    IEnumerator BearMove(Transform destination) {
        Animator animator = bear.GetComponent<Animator>();
        animator.SetBool("Movement", true);
        animator.speed = 0.5f;
        while (!Move(bear, destination, 4.5f)) {
            yield return null;
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
        if (points.Length < 2) return;
        for (int i = 0; i < points.Length - 1; i++) {
            Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }
    }
}
