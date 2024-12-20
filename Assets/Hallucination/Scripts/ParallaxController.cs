using UnityEngine;

public class ParallaxController : MonoBehaviour {
    Transform cam;
    Vector3 camPrevPos; // To calculate both X and Y distances
    float distanceX, distanceY;

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;

    float farthestBack;

    [Range(0.01f, 0.05f)]
    public float parallaxSpeed;

    void Start() {
        cam = Camera.main.transform;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++) {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount) {
        for (int i = 0; i < backCount; i++) {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack) {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }

        for (int i = 0; i < backCount; i++) {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }

    private void FixedUpdate() {
        if (camPrevPos != null && !(camPrevPos.x == 0 && camPrevPos.y == 0)) {
            distanceX = cam.position.x - camPrevPos.x;
            distanceY = cam.position.y - camPrevPos.y;
        } else {
            distanceX = 0;
            distanceY = 0;
        }

        if (((transform.parent.name == "FairyRelated"
                    && WorldSwitchManager.Instance.currentWorld == CharacterTypeEnum.Fairy)
                || (transform.parent.name == "BearRelated"
                    && WorldSwitchManager.Instance.currentWorld == CharacterTypeEnum.Bear))
            && (cam.position.x != 0 || cam.position.y != 0)) {
            camPrevPos = cam.position;
        }

        transform.position = new Vector3(cam.position.x, transform.position.y + distanceY * 0.85f, transform.position.z);

        for (int i = 0; i < backgrounds.Length; i++) {
            float speed = backSpeed[i] * parallaxSpeed;

            // Update texture offset for parallax effect in both X and Y axes
            Vector2 offset = mat[i].GetTextureOffset("_MainTex");
            offset += new Vector2(distanceX, 0) * speed;
            mat[i].SetTextureOffset("_MainTex", offset);
        }
    }
}
