using System;
using System.Collections;
using UnityEngine;

public class Level3End : MonoBehaviour {
    [Serializable]
    public class AnimationObjects {
        public Transform body;
        public Transform[] points;
    }
    [SerializeField] AnimationObjects bear, fairy;
    float eps = 1e-4f;
    bool isBearMoveFinished;
    bool isFairyMoveFinished;

    public void StartAnimation() {
        WorldSwitchManager.Instance.Fairy.GetComponent<Character>().StopMotion();
        WorldSwitchManager.Instance.Bear.GetComponent<Character>().StopMotion();
        bear.body.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        fairy.body.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        WorldSwitchManager.Instance.Bear.GetComponent<SpriteRenderer>().enabled = false;
        WorldSwitchManager.Instance.Fairy.GetComponent<SpriteRenderer>().enabled = false;
        StartCoroutine(Animation());
    }


    IEnumerator Animation() {
        
        Animator fairyAnimator = fairy.body.GetComponent<Animator>();
        int currentPointIndex = 0;
        string[] dialogueNames = {"Level 3 Comprehend_0", "Level 3 Comprehend_1", "Level 3 Comprehend_2"};
        while (currentPointIndex < fairy.points.Length) {
            GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Cinematic;
            isBearMoveFinished = false;
            isFairyMoveFinished = false;
            StartCoroutine(FairyMove(fairy.points[currentPointIndex]));
            StartCoroutine(BearMove(bear.points[currentPointIndex]));
            while (!(isBearMoveFinished && isFairyMoveFinished)) yield return null;
            bool dialogueComplete = false;
            GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
            DialogueManager.Instance.StartDialogueWithCallback(dialogueNames[currentPointIndex], () => dialogueComplete = true);
            currentPointIndex++;
            while (!dialogueComplete) {
                yield return null;
            }
        }
        print(GameStateManager.Instance.CurrentGamePlayState);
        LevelNavigator.Instance.CompleteCurrentLevel();
    }

    IEnumerator FairyMove(Transform point) {
        Animator animator = fairy.body.GetComponent<Animator>();
        animator.SetBool("Movement", true);
        animator.speed = 0.5f;
        while (!Move(fairy.body, point, 3f)) {
            yield return null;
        }
        animator.SetBool("Movement", false);
        isFairyMoveFinished = true;
    }

    IEnumerator BearMove(Transform point) {
        while (!Move(bear.body, point, 4.5f)) {
            yield return null;
        }
        isBearMoveFinished = true;
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
    void EndAnimation() {

        GameStateManager.Instance.CurrentGamePlayState = GamePlayState.Normal;
        bear.body.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        fairy.body.gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }
}
