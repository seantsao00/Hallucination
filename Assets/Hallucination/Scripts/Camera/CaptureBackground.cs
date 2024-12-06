using System.Collections;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class CaptureBackground : MonoBehaviour {
    public Camera captureCamera; // Assign a secondary camera in the inspector
    int captureWidth = 1920; // Width of the capture
    int captureHeight = 1080; // Height of the capture
    Vector2 circleCenter; // Circle center in the capture
    Vector2 captureCenter;
    float radius;
    public float localRadius; // Radius of the circle
    public Transform bearTransform;
    void Start() {
        StartCoroutine(DelayCapture());

    }

    void Update() {
        // Debug.Log(captureCamera.WorldToScreenPoint(bearTransform.position));
    }

    void CalculateParameter() {
        float pixelsPerUnit = captureWidth / 40f;
        radius = localRadius * pixelsPerUnit;
        circleCenter = captureCamera.WorldToScreenPoint(bearTransform.position);
    }
    IEnumerator DelayCapture() {
        yield return new WaitForSeconds(3);
        Capture();
    }
    void Capture() {
        print(Application.dataPath);
        CalculateParameter();
        // Set up a RenderTexture
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        captureCamera.targetTexture = renderTexture;

        // Render the camera
        captureCamera.Render();

        // Set the RenderTexture as active
        RenderTexture.active = renderTexture;

        // Read the rendered image into a Texture2D
        Texture2D capturedTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
        print(circleCenter);
        capturedTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        capturedTexture.Apply();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Hallucination/Captures/Original.png", capturedTexture.EncodeToPNG());

        // Reset the camera's targetTexture and RenderTexture
        captureCamera.targetTexture = null;
        RenderTexture.active = null;

        // Apply a circular mask to the captured texture
        Texture2D circularTexture = ApplyCircularMask(capturedTexture);

        // Save the circular texture as a PNG file
        byte[] bytes = circularTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/Hallucination/Captures/CapturedGameObjectsCircle.png", bytes);

        Debug.Log("Captured circular part saved as CapturedGameObjectsCircle.png");

        // Cleanup
        Destroy(renderTexture);
        Destroy(capturedTexture);
    }

    Texture2D ApplyCircularMask(Texture2D originalTexture) {
        // Calculate the bounding box of the circular region
        int minX = Mathf.Clamp(Mathf.FloorToInt(circleCenter.x - radius), 0, originalTexture.width);
        int maxX = Mathf.Clamp(Mathf.CeilToInt(circleCenter.x + radius), 0, originalTexture.width);
        int minY = Mathf.Clamp(Mathf.FloorToInt(circleCenter.y - radius), 0, originalTexture.height);
        int maxY = Mathf.Clamp(Mathf.CeilToInt(circleCenter.y + radius), 0, originalTexture.height);

        // Create a smaller texture to fit the bounding box
        int croppedWidth = maxX - minX;
        int croppedHeight = maxY - minY;
        Texture2D croppedTexture = new Texture2D(croppedWidth, croppedHeight, TextureFormat.RGBA32, false);

        // Copy pixels within the bounding box, applying the circular mask
        for (int y = 0; y < croppedHeight; y++) {
            for (int x = 0; x < croppedWidth; x++) {
                int sourceX = minX + x;
                int sourceY = minY + y;

                // Calculate the distance from the circle center
                float distance = Vector2.Distance(new Vector2(sourceX, sourceY), circleCenter);

                if (distance <= radius) {
                    // Within the circle: keep the pixel
                    croppedTexture.SetPixel(x, y, originalTexture.GetPixel(sourceX, sourceY));
                } else {
                    // Outside the circle: make it transparent
                    croppedTexture.SetPixel(x, y, Color.clear);
                }
            }
        }

        croppedTexture.Apply();
        return croppedTexture;
    }

}
