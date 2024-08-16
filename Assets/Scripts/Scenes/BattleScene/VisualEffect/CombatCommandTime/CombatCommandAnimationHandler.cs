using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatCommandAnimationHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cutScreenMoveDuration = 1.5f;

    [Header("Vertical Cut Scene")]
    [SerializeField] private GameObject rightPartGo = null;
    [SerializeField] private GameObject leftPartGo = null;
    [SerializeField] private Image rightPartImage = null;
    [SerializeField] private Image leftPartImage = null;
    [SerializeField] private Image rightPartBackground = null;
    [SerializeField] private Image leftPartBackground = null;
    [SerializeField] private Image rightDarkLayer = null;
    [SerializeField] private Image leftDarkLayer = null;
    [SerializeField] private Animator playerAnimator = null;

    [Header("Backgrounds")]
    [SerializeField] private Sprite rightPartBackgroundOne = null;
    [SerializeField] private Sprite rightPartBackgroundTwo = null;
    [SerializeField] private Sprite leftPartBackgroundOne = null;
    [SerializeField] private Sprite leftPartBackgroundTwo = null;

    private bool isShowing = false;
    private bool isPlayingCharacterTurningAnimation = false;

    public void Initialize()
    {
        ResetVerticalCutScreen();
    }

    public IEnumerator VerticalCutIn( Action onCompleteCallback = null )
    {
        this.isShowing = true;

        ResetVerticalCutScreen();

        this.rightPartGo.SetActive(true);
        this.leftPartGo.SetActive(true);
        BrightnessLoopAnimation(rightDarkLayer);
        BrightnessLoopAnimation(leftDarkLayer);

        Vector3 _movingRightPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Vector3 _movingLeftPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        LeanTween.move(this.rightPartGo, _movingLeftPosition, this.cutScreenMoveDuration);
        LeanTween.move(this.leftPartGo, _movingRightPosition, this.cutScreenMoveDuration);

        yield return new WaitForSeconds( this.cutScreenMoveDuration );
        onCompleteCallback?.Invoke();
    }

    public void RunCharacterTurningAnimation()
    {
        this.isPlayingCharacterTurningAnimation = true;
        this.playerAnimator.Play( "Turn", 0, 0.0f );
        LeanTween.delayedCall( this.playerAnimator.GetCurrentAnimatorClipInfo( 0 ).Length, () => { this.isPlayingCharacterTurningAnimation = false; } );
    }

    public IEnumerator VerticalCutOut(Action onCompleteCallback = null)
    {
        this.rightPartGo.SetActive(true);
        this.leftPartGo.SetActive(true);

        Vector3 _movingRightPosition = new Vector3((Screen.width * 0.4f) + Screen.width, Screen.height * 0.5f, 0);
        Vector3 _movingLeftPosition = new Vector3((Screen.width * 0.4f) - Screen.width, Screen.height * 0.5f, 0);

        LeanTween.move(this.rightPartGo, _movingRightPosition, this.cutScreenMoveDuration).setOnComplete(() => {
            ResetVerticalCutScreen();
        });
        LeanTween.move(this.leftPartGo, _movingLeftPosition, this.cutScreenMoveDuration).setOnComplete(() => { ResetVerticalCutScreen(); });

        yield return new WaitForSeconds( this.cutScreenMoveDuration );
        this.isShowing = false;
        onCompleteCallback?.Invoke();
    }

    public void ResetVerticalCutScreen()
    {
        LeanTween.cancel(this.rightPartGo);
        LeanTween.cancel(this.leftPartGo);
        LeanTween.cancel(this.rightDarkLayer.gameObject);
        LeanTween.cancel(this.leftDarkLayer.gameObject);

        this.rightPartGo.SetActive(false);
        this.leftPartGo.SetActive(false);

        this.leftPartGo.transform.position = new Vector3(Screen.width * 0.4f - Screen.width, Screen.height * 0.5f, 0);
        this.rightPartGo.transform.position = new Vector3(Screen.width * 0.4f + Screen.width, Screen.height * 0.5f, 0);

        this.playerAnimator.Play( "Default" );
    }

    private void BrightnessLoopAnimation(Image darkLayer)
    {
        LeanTween.value(darkLayer.gameObject, 0.8f, 1f, 1f).setLoopPingPong().
        setOnUpdate((float var) =>
        {
            var tempColor = darkLayer.color;
            tempColor.a = var;
            darkLayer.color = tempColor;
        });
    }

    public bool GetIsShowing()
    {
        return this.isShowing;
    }

    public bool GetIsPlayingCharacterTurningAnimation()
    {
        return this.isPlayingCharacterTurningAnimation;
    }

    public void SetBackgroundSprites(int backgroundId = 1)
    {
        if(backgroundId == 1)
        {
            this.leftPartBackground.sprite = this.leftPartBackgroundOne;
            this.rightPartBackground.sprite = this.rightPartBackgroundOne;
        }
        else if(backgroundId == 2)
        {
            this.leftPartBackground.sprite = this.leftPartBackgroundTwo;
            this.rightPartBackground.sprite = this.rightPartBackgroundTwo;
        }
    }
}
