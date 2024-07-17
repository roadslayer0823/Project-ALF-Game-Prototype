using UnityEngine;
using UnityEngine.UI;

public class LayerBlur : MonoBehaviour
{
    private int distortionId = Shader.PropertyToID("_BumpAmt");
    private int sizeId = Shader.PropertyToID("_Size");

    [SerializeField] private Shader layerBlurShader = null;
    [SerializeField] private Image image = null;

    private Material layerBlurMaterial = null;

    void OnDestroy()
    {
        if (this.layerBlurMaterial != null)
        {
            DestroyImmediate(this.layerBlurMaterial);
        }
    }

    public int Distortion
    {
        get
        {
            if(this.layerBlurMaterial != null)
            {
                return this.layerBlurMaterial.GetInt(this.distortionId);
            }
            return 10;
        }
        set
        {
            if(this.layerBlurMaterial != null)
            {
                this.layerBlurMaterial.SetInt(this.distortionId,value);
            }
        }
    }

    public int Size
    {
        get
        {
            if (this.layerBlurMaterial != null)
            {
                return this.layerBlurMaterial.GetInt(this.sizeId);
            }
            return 1;
        }
        set
        {
            if (this.layerBlurMaterial != null)
            {
                this.layerBlurMaterial.SetInt(this.sizeId, value);
            }
        }
    }

    public void CreateNewShaderMaterial()
    {
        if(this.layerBlurShader != null)
        {
            this.layerBlurMaterial = new Material(layerBlurShader)
            {
                name = "Created Layer Blur Material",
                hideFlags = HideFlags.HideAndDontSave
            };

            this.image.material = this.layerBlurMaterial;
        }
    }

    public void SetShaderValue(int distortion, int size)
    {
        this.Distortion = distortion;
        this.Size = size;
    }
}
