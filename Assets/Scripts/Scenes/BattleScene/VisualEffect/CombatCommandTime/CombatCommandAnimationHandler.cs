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
    [SerializeField] private Image rightDarkLayer = null;
    [SerializeField] private Image leftDarkLayer = null;
    [SerializeField] private Animator playerAnimator = null;

    public void Initialize()
    {
        ResetVerticalCutScreen();
    }

    public void VerticalCutIn()
    {
        ResetVerticalCutScreen();

        this.rightPartGo.SetActive(true);
        this.leftPartGo.SetActive(true);
        BrightnessLoopAnimation(rightDarkLayer);
        BrightnessLoopAnimation(leftDarkLayer);

        Vector3 _movingRightPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        Vector3 _movingLeftPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        LeanTween.move(this.rightPartGo, _movingLeftPosition, this.cutScreenMoveDuration);
        LeanTween.move(this.leftPartGo, _movingRightPosition, this.cutScreenMoveDuration);
    }

    public IEnumerator VerticalCutOut()
    {
        ResetVerticalCutScreen();
        this.rightPartGo.SetActive(true);
        this.leftPartGo.SetActive(true);

        this.playerAnimator.SetTrigger("isPlayer");

        yield return new WaitForSeconds(1f);

        // separate the cut screen
        Vector3 _movingRightPosition = new Vector3((Screen.width * 0.5f) + Screen.width, Screen.height * 0.5f, 0);
        Vector3 _movingLeftPosition = new Vector3((Screen.width * 0.5f) - Screen.width, Screen.height * 0.5f, 0);

        LeanTween.move(this.rightPartGo, _movingRightPosition, this.cutScreenMoveDuration).setOnComplete(() => {
            ResetVerticalCutScreen();
            this.playerAnimator.SetTrigger("reset");
        });
        LeanTween.move(this.leftPartGo, _movingLeftPosition, this.cutScreenMoveDuration).setOnComplete(() => { ResetVerticalCutScreen(); });
    }

    private void ResetVerticalCutScreen()
    {
        LeanTween.cancel(this.rightPartGo);
        LeanTween.cancel(this.leftPartGo);
        LeanTween.cancel(this.rightDarkLayer.gameObject);
        LeanTween.cancel(this.leftDarkLayer.gameObject);

        this.rightPartGo.SetActive(false);
        this.leftPartGo.SetActive(false);

        //this.leftPartGo.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        //this.rightPartGo.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        this.leftPartGo.transform.position = new Vector3(Screen.width * 0.4f - Screen.width, Screen.height * 0.5f, 0);
        this.rightPartGo.transform.position = new Vector3(Screen.width * 0.4f + Screen.width, Screen.height * 0.5f, 0);
    }

    public void BrightnessLoopAnimation(Image darkLayer)
    {
        LeanTween.value(darkLayer.gameObject, 0.8f, 1f, 1f).setLoopPingPong().
        setOnUpdate((float var) =>
        {
            var tempColor = darkLayer.color;
            tempColor.a = var;
            darkLayer.color = tempColor;
        });
    }

}
