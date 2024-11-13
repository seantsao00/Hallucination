using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour, ICheckpointControllable
{
    // Start is called before the first frame update

    public void Activate() {
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        gameObject.SetActive(false);
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}
