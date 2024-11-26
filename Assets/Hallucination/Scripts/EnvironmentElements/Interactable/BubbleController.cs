using UnityEngine;

public class BubbleController : MonoBehaviour, ISwitchControlled {

    [SerializeField] Bubble[] bubbles;
    [SerializeField] int numberOfStates = 3;

    public void SetState(int index) {
        index %= numberOfStates;
        for (int i = 0; i < bubbles.Length; i++) {
            if ((i + numberOfStates + 1 - index) % numberOfStates == 0) {
                bubbles[i].turnOff();
            } else {
                bubbles[i].turnOn();
            }
        }
    }
}
