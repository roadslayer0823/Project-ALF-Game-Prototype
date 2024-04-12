using UnityEngine;

[RequireComponent( typeof( Camera ) )]
public class CameraRadiusBlur : MonoBehaviour
{
    [SerializeField] private Shader radiusBlurShader = null;
    private Material radiusBlurMaterial = null;
    static int RADIUS_BLUR_PASS = 0;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (this.radiusBlurMaterial != null && BattleVisualEffectManager.onShader)
        {
            Graphics.Blit(source, destination, this.radiusBlurMaterial, RADIUS_BLUR_PASS);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    void OnDestroy()
    {
        if (this.radiusBlurMaterial != null)
        {
            DestroyImmediate(this.radiusBlurMaterial);
        }
    }

    public Material CreateNewShaderMaterial()
    {
        if (radiusBlurShader != null)
        {
            this.radiusBlurMaterial = new Material(radiusBlurShader);
            this.radiusBlurMaterial.name = "Created Radius Blur Material";
            this.radiusBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
        return this.radiusBlurMaterial;
    }
}
