using UnityEngine;

public class BattleVisualEffectManager : MonoBehaviour
{
    [SerializeField] private CameraRadiusBlur cameraRadiusBlur = null;

    public void SetUp()
    {
        if(cameraRadiusBlur != null)
        {
            cameraRadiusBlur.CreateNewShaderMaterial();
        }
    }

    public void ApplyBlurShaderAtPartB()
    {
        cameraRadiusBlur.onShader = true;
        float _centerX = 0.5f;
        float _centerY = 0.5f;
        float _radiusOffset = 0.004f;
        Vector3 _radiusData = new Vector3(_centerX, _centerY, _radiusOffset);

        cameraRadiusBlur.SetShaderValue(_radiusData, 30, 0);
    }

    public void ApplyBlurShaderAnimationAtRepulse()
    {
        cameraRadiusBlur.onShader = true;
        float _from = 30f;
        float _to = 5f;
        float _time = 1f;

        LeanTween.value(_from, _to, _time).setOnUpdate((float val) =>
        {
            cameraRadiusBlur.Iteration = (int)val;
        });
    }

    public void TurnOffBlurShader()
    {
        cameraRadiusBlur.onShader = false;
    }
}
