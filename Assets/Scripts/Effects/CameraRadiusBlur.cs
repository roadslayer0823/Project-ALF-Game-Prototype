using UnityEngine;

[RequireComponent( typeof( Camera ) )]
public class CameraRadiusBlur : MonoBehaviour
{
    static int radiusBlurDataId = Shader.PropertyToID("_RadiusData");
    static int radiusBlurIterationId = Shader.PropertyToID("_RadiusIterationData");
    static int radiusBlurCenterRange = Shader.PropertyToID("_RadiusCenterRange");

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
            if (radiusBlurMaterial != null)
            {
                return radiusBlurMaterial.GetVector(radiusBlurDataId);
            }
            return new Vector3(0.5f, 0.5f, 0.01f);
        }
        set
        {
            if (radiusBlurMaterial != null)
            {
                radiusBlurMaterial.SetVector(radiusBlurDataId, value);
            }
        }
    }

    public int Iteration
    {
        get
        {
            if (radiusBlurMaterial != null)
            {
                return (int)radiusBlurMaterial.GetVector(radiusBlurIterationId).x;
            }
            return 1;
        }
        set
        {
            if (radiusBlurMaterial != null)
            {
                float invIteration = 1.0f / value;
                radiusBlurMaterial.SetVector(radiusBlurIterationId, new Vector4(value, invIteration, 0f, 0f));
            }
        }
    }

    public float RadiusCenterRange
    {
        get
        {
            if (radiusBlurMaterial != null)
            {
                return radiusBlurMaterial.GetFloat(radiusBlurCenterRange);
            }
            return 0f;
        }
        set
        {
            if (radiusBlurMaterial != null)
            {
                if (value <= 0f)
                {
                    radiusBlurMaterial.DisableKeyword("_USE_CIRCLE_CENTER");
                }
                else
                {
                    radiusBlurMaterial.EnableKeyword("_USE_CIRCLE_CENTER");
                }
                radiusBlurMaterial.SetFloat(radiusBlurCenterRange, value);
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
