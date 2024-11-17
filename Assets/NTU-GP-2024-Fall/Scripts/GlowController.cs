using UnityEngine;

public class GlowController : MonoBehaviour
{
    private Material objectMaterial;
    private Color originalEmissionColor;
    private bool isGlowing = false;

    void Start()
    {
        // Get the object's material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("No Renderer found on the object.");
            return;
        }

        objectMaterial = renderer.material;

        // Store the original emission color
        if (objectMaterial.HasProperty("_EmissionColor"))
        {
            originalEmissionColor = objectMaterial.GetColor("_EmissionColor");
        }
        else
        {
            Debug.LogError("Material does not support emission.");
        }
        EnableGlow(Color.green);
    }

    // Function to make the object glow
    public void EnableGlow(Color glowColor, float intensity = 1.0f)
    {
        if (objectMaterial == null) return;

        isGlowing = true;
        Color emissionColor = glowColor * intensity;
        objectMaterial.SetColor("_EmissionColor", emissionColor);
        objectMaterial.EnableKeyword("_EMISSION");
    }

    // Function to turn off the glow
    public void DisableGlow()
    {
        if (objectMaterial == null || !isGlowing) return;

        isGlowing = false;
        objectMaterial.SetColor("_EmissionColor", originalEmissionColor);
        objectMaterial.DisableKeyword("_EMISSION");
    }
}
