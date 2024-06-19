using UnityEngine;
using UnityEngine.UI;

public class ScreenSizeController : MonoBehaviour
{
    [SerializeField] private RectTransform uiContainer;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GameObject blackSidePanel;
    [SerializeField] private Camera mainCamera;

    private const float ASPECT_RATIO_A = 2f; // 19 / 9
    private const float ASPECT_RATIO_B = 1.9f; // 16/ 9
    private const float ASPECT_RATIO_C = 1.6f; // 4 / 3

    public void Initialize()
    {
        float _canvasScalerHeight = this.canvasScaler.referenceResolution.y;
        if (((float)Screen.width / (float)Screen.height) > ASPECT_RATIO_A)
        {
            this.mainCamera.orthographicSize = 5.6f;
            this.canvasScaler.matchWidthOrHeight = 1;
            this.uiContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _canvasScalerHeight * ASPECT_RATIO_A);
            this.blackSidePanel.SetActive(true);
        }
        else
        {
            if (((float)Screen.width / (float)Screen.height) < ASPECT_RATIO_B)
            {
                this.canvasScaler.matchWidthOrHeight = 0;
                if (((float)Screen.width / (float)Screen.height) < ASPECT_RATIO_C)
                {
                    this.mainCamera.orthographicSize = 8.0f;
                }
                else
                {
                    this.mainCamera.orthographicSize = 6.2f;
                }
            }
            else
            {
                this.canvasScaler.matchWidthOrHeight = 1;
            }
            this.uiContainer.anchorMin = Vector2.zero;
            this.uiContainer.anchorMax = Vector2.one;
            this.uiContainer.sizeDelta = Vector2.zero;
            this.blackSidePanel.SetActive(false);
        }
        this.uiContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _canvasScalerHeight);
    }
}
