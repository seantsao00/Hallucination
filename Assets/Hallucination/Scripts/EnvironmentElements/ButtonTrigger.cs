using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public interface IButtonControlled {
    void Activate();
    void Deactivate();
}

public class ButtonTrigger : MonoBehaviour {
    [SerializeField] GameObject[] controlledObjects;
    int objectsOnButtonCount = 0;

    List<IButtonControlled> controlledList = new List<IButtonControlled>();

    private void Awake() {
        foreach (var obj in controlledObjects) {
            IButtonControlled controlled = obj.GetComponent<IButtonControlled>();
            if (controlled != null) {
                controlledList.Add(controlled);
            } else {
                Debug.LogWarning($"Omit object that has not implemented IButtonControlled: {obj}");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") || other.CompareTag("Stone")) {
            if (objectsOnButtonCount == 0) {
                foreach (var controlled in controlledList) {
                    controlled.Activate();
                    GetComponent<Animator>().SetBool("Trigger", true);
                }
            }
            objectsOnButtonCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") || other.CompareTag("Stone")) {
            objectsOnButtonCount--;
            if (objectsOnButtonCount == 0) {
                foreach (var controlled in controlledList) {
                    controlled.Deactivate();
                    GetComponent<Animator>().SetBool("Trigger", false);
                }
            }
            Assert.IsFalse(objectsOnButtonCount < 0);
        }
    }
}
