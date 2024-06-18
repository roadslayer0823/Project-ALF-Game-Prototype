using UnityEngine;
using UnityEngine.UI;

public class ScreenSizeController : MonoBehaviour
{
    [SerializeField] private RectTransform uiContainer;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GameObject blackSidePanel;

    public void Initialize()
    {
        float canvasScalerHeight = this.canvasScaler.referenceResolution.y;
        if ((float)Screen.width / (float)Screen.height > 19.5f / 9.0f)
        {
            float width = (float)(canvasScalerHeight / 9.0f * 19.5f);
            this.uiContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            this.blackSidePanel.SetActive(true);
        }
        else
        {
            this.uiContainer.anchorMin = Vector2.zero;
            this.uiContainer.anchorMax = Vector2.one;
            this.uiContainer.sizeDelta = Vector2.zero;
            this.blackSidePanel.SetActive(false);
        }
        this.uiContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasScalerHeight);
    }
}
