using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutScreenHandlerV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cutScreenMoveDuration = 1.5f;
    [SerializeField] private float grayscaleDuration = 1.0f;
    [SerializeField] private float arrowMoveDuration = 1.1f;
    [SerializeField] private float arrowMoveForwardDuration = 0.2f;

    [Header("Outside References")]
    [SerializeField] private ScreenshotHandler screenshotHandler = null;

    [Header("Inner References")]
    [SerializeField] private GameObject firstPartGO = null;
    [SerializeField] private GameObject secondPartGO = null;
    [SerializeField] private Image firstPartImage = null;
    [SerializeField] private Image secondPartImage = null;
    [SerializeField] private SpriteRenderer backgroundImage = null;

    [Header("Horizontal Cut Scene")]
    [SerializeField] private Image horizontalCutLine = null;
    [SerializeField] private RectTransform targetPoint = null;
    [SerializeField] private RectTransform defaultPoint = null;

    [Header("Vertical Cut Scene")]
    [SerializeField] private GameObject rightPartGo = null;
    [SerializeField] private GameObject leftPartGo = null;
    [SerializeField] private Image rightPartImage = null;
    [SerializeField] private Image leftPartImage = null;
    [SerializeField] private Image verticalTopCutLine = null;
    [SerializeField] private Image verticalBottomCutLine = null;
    [SerializeField] private RectTransform topTargetPoint = null;
    [SerializeField] private RectTransform topDefaultPoint = null;
    [SerializeField] private RectTransform bottomTargetPoint = null;
    [SerializeField] private RectTransform bottomDefaultPoint = null;

    private bool isGrayscaleCompleted = false;
    private bool isCuttingCompleted = false;

    private void Initialize()
    {
        ResetHorizontalCutScreen();
        ResetVerticalCutScreen();
    }

    public void OnVerticalScreenshot(Action onComplete = null)
    {
        StartCoroutine(VerticalCutScreen(onComplete));
    }

    public void OnHorizontalScreenshot(Action onComplete = null)
    {
        StartCoroutine(HorizontalCutScreen(onComplete));
    }

    private IEnumerator HorizontalCutScreen(Action onComplete = null)
    {
        ResetHorizontalCutScreen();

        this.screenshotHandler.TakeScreenshot();

        yield return new WaitUntil(() => screenshotHandler.GetIsReadyToCut());

        if (screenshotHandler.GetImageToCut() == null)
        {
            Debug.Log("No captured image.");
        }
        else
        {
            Debug.Log("Screenshot button clicked horizontal.");

            HorizontalCutLineAnimation();

            this.firstPartGO.SetActive(true);
            this.secondPartGO.SetActive(true);

            this.firstPartImage.sprite = screenshotHandler.GetImageToCut();
            this.secondPartImage.sprite = screenshotHandler.GetImageToCut();

            StartCoroutine(GrayscaleCoroutine(this.firstPartImage, this.grayscaleDuration, true));
            StartCoroutine(GrayscaleCoroutine(this.secondPartImage, this.grayscaleDuration, true));

            yield return new WaitUntil(() => this.isCuttingCompleted && this.isGrayscaleCompleted);

            Vector3 _movingUpwardPosition = new Vector3(Screen.width * 0.5f, (Screen.height * 0.5f) + Screen.height, 0);
            Vector3 _movingDownwardPosition = new Vector3(Screen.width * 0.5f, (Screen.height * 0.5f) - Screen.height, 0);
            LeanTween.move(this.firstPartGO, _movingUpwardPosition, this.cutScreenMoveDuration).setOnComplete(() => {ResetHorizontalCutScreen(); onComplete?.Invoke(); });
            LeanTween.move(this.secondPartGO, _movingDownwardPosition, this.cutScreenMoveDuration).setOnComplete(() => { ResetHorizontalCutScreen(); onComplete.Invoke(); });
        }
    }

    private IEnumerator VerticalCutScreen(Action onComplete = null)
    {
        ResetVerticalCutScreen();

        this.screenshotHandler.TakeScreenshot();

        yield return new WaitUntil(() => screenshotHandler.GetIsReadyToCut());

        if (screenshotHandler.GetImageToCut() == null)
        {
            Debug.Log("No captured image.");
        }
        else
        {

            VerticalCutLineAnimation();

            this.rightPartGo.SetActive(true);
            this.leftPartGo.SetActive(true);

            this.verticalTopCutLine.gameObject.SetActive(true);
            this.verticalBottomCutLine.gameObject.SetActive(true);

            this.rightPartImage.sprite = screenshotHandler.GetImageToCut();
            this.leftPartImage.sprite = screenshotHandler.GetImageToCut();

            StartCoroutine(GrayscaleCoroutine(this.rightPartImage, this.grayscaleDuration, true));
            StartCoroutine(GrayscaleCoroutine(this.leftPartImage, this.grayscaleDuration, true));

            yield return new WaitUntil(() => this.isCuttingCompleted && this.isGrayscaleCompleted);

            Vector3 _movingRightPosition = new Vector3((Screen.width * 0.5f) + Screen.width, Screen.height * 0.5f, 0);
            Vector3 _movingLeftPosition = new Vector3((Screen.width * 0.5f) - Screen.width, Screen.height * 0.5f, 0);

            LeanTween.move(this.rightPartGo, _movingRightPosition, this.cutScreenMoveDuration).setOnComplete(() => { ResetVerticalCutScreen(); onComplete?.Invoke(); });
            LeanTween.move(this.leftPartGo, _movingLeftPosition, this.cutScreenMoveDuration).setOnComplete(() => { ResetVerticalCutScreen(); onComplete?.Invoke(); });
            LeanTween.move(this.verticalTopCutLine.gameObject, _movingRightPosition, this.arrowMoveDuration).setOnComplete(() => { ResetVerticalCutScreen(); onComplete?.Invoke(); });
            LeanTween.move(this.verticalBottomCutLine.gameObject, _movingLeftPosition, this.arrowMoveDuration).setOnComplete(() => { ResetVerticalCutScreen(); onComplete?.Invoke(); });
        }
    }

    private IEnumerator GrayscaleCoroutine(Image spriteRenderer, float duration, bool isGrayscale)
    {
        float _time = 0.0f;
        while (duration > _time)
        {
            float _durationFrame = Time.deltaTime;
            float _ratio = _time / duration;
            float _grayAmount = isGrayscale ? _ratio : 1 - _ratio;
            spriteRenderer.materialForRendering.SetFloat("_GrayscaleAmount", _grayAmount);
            _time += _durationFrame;

            yield return null;
        }

        this.isGrayscaleCompleted = true;
    }

    private void HorizontalCutLineAnimation()
    {
        ArrowRotationCalculation(defaultPoint, targetPoint, horizontalCutLine);
        LeanTween.move(horizontalCutLine.gameObject, targetPoint, this.arrowMoveForwardDuration)
            .setOnComplete(() =>
            {
                this.isCuttingCompleted = true;
            });
    }

    private void VerticalCutLineAnimation()
    {

        ArrowRotationCalculation(this.topDefaultPoint, topTargetPoint, verticalTopCutLine);
        ArrowRotationCalculation(this.bottomDefaultPoint, bottomTargetPoint, verticalBottomCutLine);

        LeanTween.move(verticalTopCutLine.gameObject, topTargetPoint, this.arrowMoveForwardDuration);
        LeanTween.value(verticalTopCutLine.gameObject, verticalTopCutLine.fillAmount, 1, this.arrowMoveForwardDuration)
            .setOnUpdate((float val) =>
            {
                verticalTopCutLine.fillAmount = val;
            });

        LeanTween.move(verticalBottomCutLine.gameObject, bottomTargetPoint, this.arrowMoveForwardDuration);
        LeanTween.value(verticalBottomCutLine.gameObject, verticalBottomCutLine.fillAmount, 1, this.arrowMoveForwardDuration)
            .setOnUpdate((float val) =>
            {
                verticalBottomCutLine.fillAmount = val;
            }).setOnComplete(() =>
            {
                this.isCuttingCompleted = true;
            });
    }

    private void ResetHorizontalCutScreen()
    {
        LeanTween.cancel(this.firstPartGO);
        LeanTween.cancel(this.secondPartGO);

        this.firstPartGO.SetActive(false);
        this.secondPartGO.SetActive(false);

        this.horizontalCutLine.transform.position = this.defaultPoint.transform.position;
      
        this.firstPartGO.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        this.secondPartGO.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        this.firstPartImage.sprite = null;
        this.secondPartImage.sprite = null;

        this.firstPartImage.materialForRendering.SetFloat("_GrayscaleAmount", 0);
        this.secondPartImage.materialForRendering.SetFloat("_GrayscaleAmount", 0);

        this.isGrayscaleCompleted = false;
        this.isCuttingCompleted = false;
    }


    private void ResetVerticalCutScreen()
    {
        LeanTween.cancel(this.rightPartGo);
        LeanTween.cancel(this.leftPartGo);

        this.rightPartGo.SetActive(false);
        this.leftPartGo.SetActive(false);

        this.verticalBottomCutLine.transform.position = this.bottomDefaultPoint.transform.position;
        this.verticalTopCutLine.transform.position = this.topDefaultPoint.transform.position;

        verticalTopCutLine.fillAmount = 0.4f;
        verticalBottomCutLine.fillAmount = 0.4f;

        this.leftPartGo.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);
        this.rightPartGo.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        this.rightPartImage.sprite = null;
        this.leftPartImage.sprite = null;

        this.rightPartImage.materialForRendering.SetFloat("_GrayscaleAmount", 0);
        this.leftPartImage.materialForRendering.SetFloat("_GrayscaleAmount", 0);

        this.isGrayscaleCompleted = false;
        this.isCuttingCompleted = false;

        this.verticalTopCutLine.gameObject.SetActive(false);
        this.verticalBottomCutLine.gameObject.SetActive(false);
    }

    private void ArrowRotationCalculation(RectTransform defaultPoint, RectTransform targetPoint, Image targetCutLine)
    {
        Vector2 startingPoint = new Vector2(defaultPoint.position.x, defaultPoint.position.y);
        Vector2 endingPoint = new Vector2(targetPoint.position.x, targetPoint.position.y);
        float angle = Mathf.Atan2(startingPoint.y - endingPoint.y, startingPoint.x - endingPoint.x) * Mathf.Rad2Deg;
        targetCutLine.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
