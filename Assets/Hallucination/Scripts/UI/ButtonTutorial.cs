using TMPro;
using UnityEngine;

public class ButtonTutorial : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public Character fairy;
    Material textMaterial;
    public float MaxDistance;
    public float MinDistance;
    Color currentColor;
    void Start() {
        textMaterial = tutorialText.fontMaterial;
        
    } 

    void Update() {
        AdjustGlow(fairy.transform.position, gameObject.transform.position);
        currentColor = textMaterial.GetColor("_FaceColor");
    }

    void AdjustGlow(Vector2 fairyPosition, Vector2 objectPosition) {
        float distance = Vector2.Distance(fairyPosition, objectPosition);
        if (distance < MinDistance) distance = MinDistance;
        if (distance > MaxDistance) distance = MaxDistance;
        float intensity = 1 - ((distance - MinDistance) / (MaxDistance - MinDistance));
        float opacity = 1 - intensity;
        currentColor.a = Mathf.Clamp01(opacity);
        textMaterial.SetFloat("_GlowPower", 0.4f * intensity);
        textMaterial.SetColor("_FaceColor", currentColor);
    }
}
