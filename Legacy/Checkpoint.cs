using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Checkpoint : Syncable {
    public bool isChecked;

    protected void Awake() {
        isChecked = false;
    }
    public override void SyncState() {
        SyncedObject.SetActive(!isChecked);
    }
    public GameObject checkObject;


    // Detect when the checkObject overlaps with this checkpoint
    private void OnTriggerEnter2D(Collider2D other) {
        print("triggered");
        if (other.gameObject == checkObject) {
            isChecked = true;
        }
    }

    // Detect when the checkObject stops overlapping
    private void OnTriggerExit2D(Collider2D other) {
        print("not triggered");
        if (other.gameObject == checkObject) {
            isChecked = false;
        }
    }
}

