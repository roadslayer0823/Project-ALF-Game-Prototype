using System.Collections;
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

        cameraRadiusBlur.SetShaderValue(_radiusData, 5, 0);
    }

    public void ApplyBlurShaderAnimationAtRepulse()
    {
        cameraRadiusBlur.onShader = true;
        StartCoroutine(SetBlurDelay());
    }

    IEnumerator SetBlurDelay()
    {
        cameraRadiusBlur.Iteration = 20;

        yield return new WaitForSeconds(0.3f);

        float _from = 30f;
        float _to = 5f;
        float _time = 0.5f;

        LeanTween.value(_from, _to, _time).setOnUpdate((float val) => { cameraRadiusBlur.Iteration = (int)val; });
    }

    public void TurnOffBlurShader()
    {
        cameraRadiusBlur.onShader = false;
    }
}
