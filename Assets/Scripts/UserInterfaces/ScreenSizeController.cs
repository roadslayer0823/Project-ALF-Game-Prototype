using UnityEngine;
using UnityEngine.UI;

public class ScreenSizeController : MonoBehaviour
{
    [SerializeField] private RectTransform uiContainer;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GameObject blackSidePanel;

    private const float MAXIMUM_ASPECT_RATIO = 19.5f / 9.0f;

    public void Initialize()
    {
        float _canvasScalerHeight = this.canvasScaler.referenceResolution.y;
        if (( ( float )Screen.width / ( float )Screen.height ) > MAXIMUM_ASPECT_RATIO)
        {
            this.uiContainer.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, _canvasScalerHeight * MAXIMUM_ASPECT_RATIO );
            this.blackSidePanel.SetActive( true );
        }
        else
        {
            this.uiContainer.anchorMin = Vector2.zero;
            this.uiContainer.anchorMax = Vector2.one;
            this.uiContainer.sizeDelta = Vector2.zero;
            this.blackSidePanel.SetActive( false );
        }
        this.uiContainer.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, _canvasScalerHeight );
    }
}
