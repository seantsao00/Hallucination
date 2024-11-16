using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Lock : MonoBehaviour, Lockable {
    public Key[] keys;
    bool isLocked;

    void Start() {
        isLocked = false;
    }

    void Update() {
        if (!isLocked && !checkIfLocked(keys)) {
            isLocked = true;
            Unlock();
        }
    }

    public bool checkIfLocked(Key[] keys) {
        foreach (var key in keys) {
            if (key.IsLocked) {
                return true;
            }
        }
        return false;
    }

    public void Unlock() {
        // print("Unlocked!");
    }
}