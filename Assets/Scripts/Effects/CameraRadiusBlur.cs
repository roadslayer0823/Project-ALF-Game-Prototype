using UnityEngine;

[RequireComponent( typeof( Camera ) )]
public class CameraRadiusBlur : MonoBehaviour
{
    private int radiusBlurDataId = Shader.PropertyToID("_RadiusData");
    private int radiusBlurIterationId = Shader.PropertyToID("_RadiusIterationData");
    private int radiusBlurCenterRange = Shader.PropertyToID("_RadiusCenterRange");

    const int RADIUS_BLUR_PASS = 0;
    [SerializeField] private Shader radiusBlurShader = null;
    private Material radiusBlurMaterial = null;
    public bool onShader = false;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (this.radiusBlurMaterial != null && onShader)
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

    public Vector3 RadiusData
    {
        get
        {
            if (this.radiusBlurMaterial != null)
            {
                return this.radiusBlurMaterial.GetVector(this.radiusBlurDataId);
            }
            return new Vector3(0.5f, 0.5f, 0.01f);
        }
        set
        {
            if (this.radiusBlurMaterial != null)
            {
                this.radiusBlurMaterial.SetVector(this.radiusBlurDataId, value);
            }
        }
    }

    public int Iteration
    {
        get
        {
            if (this.radiusBlurMaterial != null)
            {
                return (int)this.radiusBlurMaterial.GetVector(this.radiusBlurIterationId).x;
            }
            return 1;
        }
        set
        {
            if (this.radiusBlurMaterial != null)
            {
                float invIteration = 1.0f / value;
                this.radiusBlurMaterial.SetVector(this.radiusBlurIterationId, new Vector4(value, invIteration, 0f, 0f));
            }
        }
    }

    public float RadiusCenterRange
    {
        get
        {
            if (this.radiusBlurMaterial != null)
            {
                return this.radiusBlurMaterial.GetFloat(this.radiusBlurCenterRange);
            }
            return 0f;
        }
        set
        {
            if (this.radiusBlurMaterial != null)
            {
                if (value <= 0f)
                {
                    this.radiusBlurMaterial.DisableKeyword("_USE_CIRCLE_CENTER");
                }
                else
                {
                    this.radiusBlurMaterial.EnableKeyword("_USE_CIRCLE_CENTER");
                }
                this.radiusBlurMaterial.SetFloat(radiusBlurCenterRange, value);
            }
        }
    }

    public void CreateNewShaderMaterial()
    {
        if (radiusBlurShader != null)
        {
            this.radiusBlurMaterial = new Material(radiusBlurShader)
            {
                name = "Created Radius Blur Material",
                hideFlags = HideFlags.HideAndDontSave
            };
        }
    }

    public void SetShaderValue(Vector3 radiusData, int iteration, float radiusCenterRange)
    {
        this.RadiusData = radiusData;
        this.Iteration = iteration;
        this.RadiusCenterRange = radiusCenterRange;
    }
}
