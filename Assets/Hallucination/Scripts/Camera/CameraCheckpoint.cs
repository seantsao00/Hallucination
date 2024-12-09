using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraCheckpoint : MonoBehaviour {
    public CinemachineVirtualCamera checkpointCamera;
    readonly float activationDelay = 0.75f; // Time in seconds the player needs to stay in the zone
    private Coroutine activationCoroutine;
    private bool isPlayerInZone = false;

    void Start() {
        // Make sure the checkpoint camera is always on top
        checkpointCamera.Priority = 100;
        // checkpointCamera.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            isPlayerInZone = true;
            // Start the activation coroutine
            activationCoroutine = StartCoroutine(ActivateCameraAfterDelay());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            isPlayerInZone = false;
            if (activationCoroutine != null) {
                StopCoroutine(activationCoroutine);
                activationCoroutine = null;
            }
            checkpointCamera.gameObject.SetActive(false);
        }
    }

    private IEnumerator ActivateCameraAfterDelay() {
        yield return new WaitForSeconds(activationDelay);

        if (isPlayerInZone) {
            checkpointCamera.gameObject.SetActive(true);
        }
    }
}
