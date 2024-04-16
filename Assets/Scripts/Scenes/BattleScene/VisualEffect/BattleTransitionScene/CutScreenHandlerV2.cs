using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutScreenHandlerV2 : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cutScreenMoveDuration = 1.5f;
    [SerializeField] private float grayscaleDuration = 1.0f;

    [Header("Outside References")]
    [SerializeField] private Button verticalScreenshotBtn = null;
    [SerializeField] private Button horizontalScreenshotBtn = null;
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
    [SerializeField] private RectTransform topDefaulttPoint = null;
    [SerializeField] private RectTransform bottomTargetPoint = null;
    [SerializeField] private RectTransform bottomDefaultPoint = null;

    private bool isGrayscaleCompleted = false;
    private bool isCuttingCompleted = false;

    private void Start()
    {
        this.verticalScreenshotBtn.onClick.AddListener(OnVerticalScreenshotClick);
        this.horizontalScreenshotBtn.onClick.AddListener(OnHorizontalScreenshotClick);

        ResetHorizontalCutScreen();
        ResetVerticalCutScreen();
    }

    private void OnVerticalScreenshotClick()
    {
        StartCoroutine(VerticalCutScreen());

        Debug.Log("Screenshot button clicked.");
        Debug.Log("Screen.width: " + Screen.width + "\n" + "Screen.height: " + Screen.height);
    }

    private void OnHorizontalScreenshotClick()
    {
        StartCoroutine(HorizontalCutScreen());

        
    }

    private IEnumerator HorizontalCutScreen()
    {
        Debug.Log("Screenshot button clicked.");
        Debug.Log("Screen.width: " + Screen.width + "\n" + "Screen.height: " + Screen.height); ResetHorizontalCutScreen();

        this.screenshotHandler.TakeScreenshot();

        yield return new WaitUntil(() => screenshotHandler.GetIsReadyToCut());

        if (screenshotHandler.GetImageToCut() == null)
        {
            Debug.Log("No captured image.");
        }
        else
        {
            this.horizontalScreenshotBtn.enabled = false;

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
            LeanTween.move(this.firstPartGO, _movingUpwardPosition, this.cutScreenMoveDuration).setOnComplete(ResetHorizontalCutScreen);
            LeanTween.move(this.secondPartGO, _movingDownwardPosition, this.cutScreenMoveDuration).setOnComplete(ResetHorizontalCutScreen);
        }
    }

    private IEnumerator VerticalCutScreen()
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
            this.verticalScreenshotBtn.enabled = false;

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

            LeanTween.move(this.rightPartGo, _movingRightPosition, this.cutScreenMoveDuration).setOnComplete(ResetVerticalCutScreen);
            LeanTween.move(this.leftPartGo, _movingLeftPosition, this.cutScreenMoveDuration).setOnComplete(ResetVerticalCutScreen);
            LeanTween.move(this.verticalTopCutLine.gameObject, _movingRightPosition, this.cutScreenMoveDuration).setOnComplete(ResetVerticalCutScreen);
            LeanTween.move(this.verticalBottomCutLine.gameObject, _movingLeftPosition, this.cutScreenMoveDuration).setOnComplete(ResetVerticalCutScreen);
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
        LeanTween.move(horizontalCutLine.gameObject, targetPoint, 0.5f)
            .setOnComplete(() =>
            {
                this.isCuttingCompleted = true;
            });
    }

    private void VerticalCutLineAnimation()
    {
        LeanTween.move(verticalTopCutLine.gameObject, topTargetPoint, 0.5f);
        LeanTween.value(verticalTopCutLine.gameObject, verticalTopCutLine.fillAmount, 1, 0.5f)
            .setOnUpdate((float val) =>
            {
                verticalTopCutLine.fillAmount = val;
            });

        LeanTween.move(verticalBottomCutLine.gameObject, bottomTargetPoint, 0.5f);
        LeanTween.value(verticalBottomCutLine.gameObject, verticalBottomCutLine.fillAmount, 1, 0.5f)
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

        this.horizontalScreenshotBtn.enabled = true;
    }


    private void ResetVerticalCutScreen()
    {
        LeanTween.cancel(this.rightPartGo);
        LeanTween.cancel(this.leftPartGo);

        this.rightPartGo.SetActive(false);
        this.leftPartGo.SetActive(false);

        this.verticalBottomCutLine.transform.position = this.bottomDefaultPoint.transform.position;
        this.verticalTopCutLine.transform.position = this.topDefaulttPoint.transform.position;

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

        this.verticalScreenshotBtn.enabled = true;

        this.verticalTopCutLine.gameObject.SetActive(false);
        this.verticalBottomCutLine.gameObject.SetActive(false);

        this.verticalScreenshotBtn.enabled = true;
    }
}
