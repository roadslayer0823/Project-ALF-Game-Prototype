using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BattleVisualEffectManager : MonoBehaviour
{
    static int radius_blur_dataId = Shader.PropertyToID("_RadiusData"),
        radius_blur_iterationId = Shader.PropertyToID("_RadiusIterationData"),
        radius_blur_centerRange = Shader.PropertyToID("_RadiusCenterRange");

    [SerializeField] private CameraRadiusBlur cameraRadiusBlur = null;
    public Material radius_material = null;
    public static bool onShader = false;

    public Vector3 radius_data
    {
        get
        {
            if (radius_material != null)
            {
                return radius_material.GetVector(radius_blur_dataId);
            }
            return new Vector3(0.5f, 0.5f, 0.01f);
        }
        set
        {
            if (radius_material != null)
            {
                radius_material.SetVector(radius_blur_dataId, value);
            }
        }
    }

    public int iteration
    {
        get
        {
            if (radius_material != null)
            {
                return (int)radius_material.GetVector(radius_blur_iterationId).x;
            }
            return 1;
        }
        set
        {
            if (radius_material != null)
            {
                float invIteration = 1.0f / value;
                radius_material.SetVector(radius_blur_iterationId, new Vector4(value, invIteration, 0f, 0f));
            }
        }
    }

    public  float radius_center_range
    {
        get
        {
            if (radius_material != null)
            {
                return radius_material.GetFloat(radius_blur_centerRange);
            }
            return 0f;
        }
        set
        {
            if (radius_material != null)
            {
                if (value <= 0f)
                {
                    radius_material.DisableKeyword("_USE_CIRCLE_CENTER");
                }
                else
                {
                    radius_material.EnableKeyword("_USE_CIRCLE_CENTER");
                }
                radius_material.SetFloat(radius_blur_centerRange, value);
            }
        }
    }

    public void SetUp()
    {
        if(cameraRadiusBlur != null)
        {
            this.radius_material = cameraRadiusBlur.CreateNewShaderMaterial();
        }
    }

    public void ApplyBlurShaderAtPartB()
    {
        onShader = true;
        float _centerX = 0.5f;
        float _centerY = 0.5f;
        float _radiusOffset = 0.004f;

        iteration = 5;
        radius_data = new Vector3(_centerX, _centerY, _radiusOffset);
        radius_center_range = 0;
    }

    public void ApplyBlurShaderAnimationAtRepulse()
    {
        onShader = true;
        float _from = 30f;
        float _to = 5f;
        float _time = 1f;

        LeanTween.value(_from, _to, _time).setOnUpdate((float val) =>
        {
            iteration = (int)val;
        });
    }
}
