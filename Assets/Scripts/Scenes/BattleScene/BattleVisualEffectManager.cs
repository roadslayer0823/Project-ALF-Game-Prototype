using System;
using System.Collections;
using UnityEngine;

public class BattleVisualEffectManager : MonoBehaviour
{
    [SerializeField] private CameraRadiusBlur cameraRadiusBlur = null;
    [SerializeField] private CutScreenHandlerV2 cutScreenHandler = null;
    [SerializeField] private CombatCommandAnimationHandler combatCommandAnimationHandler = null;
    [SerializeField] private Animator darkEffectAnimator = null;

    public void SetUp()
    {
        if (this.cameraRadiusBlur != null)
        {
            this.cameraRadiusBlur.CreateNewShaderMaterial();
        }

        if (this.cutScreenHandler != null)
        {
            this.cutScreenHandler.Initialize();
        }

        if(this.combatCommandAnimationHandler != null)
        {
            this.combatCommandAnimationHandler.Initialize();
        }
    }

#region Radius Blur

    public void ApplyBlurShaderAtPartB()
    {
        this.cameraRadiusBlur.onShader = true;

        float _centerX = 0.5f;
        float _centerY = 0.5f;
        float _radiusOffset = 0.004f;
        Vector3 _radiusData = new Vector3(_centerX, _centerY, _radiusOffset);

        this.cameraRadiusBlur.SetShaderValue(_radiusData, 5, 0);
    }

    public void ApplyBlurShaderAnimationAtRepulse()
    {
        this.cameraRadiusBlur.onShader = true;
        StartCoroutine(SetBlurDelay());
    }

    IEnumerator SetBlurDelay()
    {
        this.cameraRadiusBlur.Iteration = 20;

        yield return new WaitForSeconds(0.3f);

        float _from = 30f;
        float _to = 5f;
        float _time = 0.5f;

        LeanTween.value(_from, _to, _time).setOnUpdate((float val) => { cameraRadiusBlur.Iteration = (int)val; });
    }

    public void TurnOffBlurShader()
    {
        this.cameraRadiusBlur.onShader = false;
    }

#endregion

#region Battle Transition Animation

    public void TransitionToNextPart( Action onCompleteCallback = null )
    {
        this.cutScreenHandler.OnHorizontalScreenshot( onCompleteCallback );
    }

    public void TransitionToNextATL( Action onCompleteCallback = null )
    {
        this.cutScreenHandler.OnVerticalScreenshot( onCompleteCallback );
    }

#endregion

#region Darken Effect
    public void TriggerAnimationSetDarkenPartA()
    {
        this.darkEffectAnimator.SetTrigger("darkenPartA");
    }

    public void TriggerAnimationSetDarkenPartB()
    {
        this.darkEffectAnimator.SetTrigger("darkenPartB");
    }

#endregion

#region Combat Command Time Cut In Animation

    public void TriggerCombatCommandCutIn( Action onCompleteCallback = null )
    {
        StartCoroutine( this.combatCommandAnimationHandler.VerticalCutIn( onCompleteCallback ) );
    }

    public void TriggerCombatCommandCutOut(bool hasPlayerAnimation, Action onCompleteCallback = null )
    {
        StartCoroutine( this.combatCommandAnimationHandler.VerticalCutOut(hasPlayerAnimation, onCompleteCallback ) );
    }

    public bool IsShowingCombatCommandCutScreen()
    {
        return this.combatCommandAnimationHandler.GetIsShowing();
    }

#endregion
}
