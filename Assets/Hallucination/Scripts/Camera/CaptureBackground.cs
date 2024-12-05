using System.Collections;
using UnityEngine;

public class CaptureBackground : MonoBehaviour {
    public Camera captureCamera; // Assign a secondary camera in the inspector
    public int captureWidth = 512; // Width of the capture
    public int captureHeight = 512; // Height of the capture
    public Vector2 circleCenter = new Vector2(256, 256); // Circle center in the capture
    public float radius = 200; // Radius of the circle
    void Start() {
        StartCoroutine(DelayCapture());
    }
    IEnumerator DelayCapture() {
        yield return new WaitForSeconds(3);
        Capture();
    }
    void Capture()
    {   
        // Set up a RenderTexture
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        captureCamera.targetTexture = renderTexture;

        // Render the camera
        captureCamera.Render();
        
        // Set the RenderTexture as active
        RenderTexture.active = renderTexture;

        // Read the rendered image into a Texture2D
        Texture2D capturedTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
        System.IO.File.WriteAllBytes("C:\\Users\\user\\Game Programming Project\\Hallucination\\Captures" + "/Original.png", capturedTexture.EncodeToPNG());
        capturedTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        capturedTexture.Apply();

        // Reset the camera's targetTexture and RenderTexture
        captureCamera.targetTexture = null;
        RenderTexture.active = null;

        // Apply a circular mask to the captured texture
        Texture2D circularTexture = ApplyCircularMask(capturedTexture);

        // Save the circular texture as a PNG file
        byte[] bytes = circularTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes("C:\\Users\\user\\Game Programming Project\\Hallucination\\Captures" + "/CapturedGameObjectsCircle.png", bytes);

        Debug.Log("Captured circular part saved as CapturedGameObjectsCircle.png");

        // Cleanup
        Destroy(renderTexture);
        Destroy(capturedTexture);
    }

    Texture2D ApplyCircularMask(Texture2D originalTexture)
    {
        Texture2D circularTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);

        for (int y = 0; y < originalTexture.height; y++)
        {
            for (int x = 0; x < originalTexture.width; x++)
            {
                // Calculate the distance from the circle center
                float distance = Vector2.Distance(new Vector2(x, y), circleCenter);

                if (distance <= radius)
                {
                    // Within the circle: keep the pixel
                    circularTexture.SetPixel(x, y, originalTexture.GetPixel(x, y));
                }
                else
                {
                    // Outside the circle: make it transparent
                    circularTexture.SetPixel(x, y, Color.clear);
                }
            }
        }

        circularTexture.Apply();
        return circularTexture;
    }
}
