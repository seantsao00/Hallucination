using UnityEngine;

public class SurroundingCapturer : MonoBehaviour {
    public static SurroundingCapturer Instance { get; private set; }
    Camera captureCamera;
    int captureWidth = 1920; // Width of the capture
    int captureHeight = 1080; // Height of the capture
    Vector2 circleCenter;
    float radius;
    public Texture2D cachedTexture;
    string capturePath;

    float localRadius => WorldSwitchManager.Instance.Bear.GetComponentInChildren<CharacterProjectionDetector>().radius;

    void Awake() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning($"{typeof(SurroundingCapturer)}: " +
            "Duplicate instance detected and removed. Only one instance of SurroundingCapture is allowed.");
            Destroy(Instance);
            return;
        }
        Instance = this;
        capturePath = Application.dataPath + "/Hallucination/Captures";
        System.IO.Directory.CreateDirectory(capturePath);
        captureCamera = GetComponent<Camera>();
        WorldSwitchManager.Instance.WorldStartSwitching.AddListener(CaptureWhenSwitched);
    }

    void Update() {
        if (WorldSwitchManager.Instance != null && WorldSwitchManager.Instance.Bear != null) {
            Quaternion currentRotation = gameObject.transform.rotation;

            float bearYRotation = WorldSwitchManager.Instance.Bear.transform.eulerAngles.y;
            
            /*
            gameObject.transform.rotation = Quaternion.Euler(
                currentRotation.eulerAngles.x,
                bearYRotation,
                currentRotation.eulerAngles.z
            );
            */
            // print(currentRotation + " " + gameObject.transform.rotation);
        }
    }

    void CalculateParameter() {
        float pixelsPerUnit = captureWidth / 40f;
        radius = localRadius * pixelsPerUnit;
        circleCenter = captureCamera.WorldToScreenPoint(transform.position);
    }

    void CaptureWhenSwitched() {
        if (WorldSwitchManager.Instance.currentWorld == CharacterTypeEnum.Bear) Capture();
    }
    void Capture() {
        
        CalculateParameter();
        gameObject.transform.position = new Vector3(WorldSwitchManager.Instance.Bear.transform.position.x, 
                                                WorldSwitchManager.Instance.Bear.transform.position.y,
                                                -10);
        

        // Debug.Log("Capture called! Camera position:" + gameObject.transform.position);
        RenderTexture renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
        captureCamera.targetTexture = renderTexture;
        captureCamera.Render();
        RenderTexture.active = renderTexture;

        // Read the rendered image into a Texture2D
        Texture2D capturedTexture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
        capturedTexture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        capturedTexture.Apply();
        System.IO.File.WriteAllBytes(capturePath + "/Original.png", capturedTexture.EncodeToPNG());

        captureCamera.targetTexture = null;
        RenderTexture.active = null;

        Texture2D circularTexture = ApplyCircularMask(capturedTexture);
        cachedTexture = circularTexture;
        byte[] bytes = circularTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(capturePath + "/CapturedGameObjectsCircle.png", bytes);

        // Debug.Log("Captured circular part saved as CapturedGameObjectsCircle.png");
        
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
                float distance = Vector2.Distance(new Vector2(sourceX, sourceY), circleCenter);
                if (distance <= radius) {
                    croppedTexture.SetPixel(x, y, originalTexture.GetPixel(sourceX, sourceY));
                } else {
                    croppedTexture.SetPixel(x, y, Color.clear);
                }
            }
        }
        croppedTexture.Apply();
        return croppedTexture;
    }

}
