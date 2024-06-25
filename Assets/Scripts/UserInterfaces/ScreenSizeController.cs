using UnityEngine;
using UnityEngine.UI;

public class ScreenSizeController : MonoBehaviour
{
    [SerializeField] private RectTransform uiContainer;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private GameObject blackSidePanel;
    [SerializeField] private Camera mainCamera;

    private const float MAXIMUM_ASPECT_RATIO = 19.5f / 9.0f;
    private const float TARGET_ASPECT_RATIO = 16.0f / 9.0f;

    public void Initialize()
    {
        float _aspectRatio = ( float )Screen.width / ( float )Screen.height;
        float _canvasScalerHeight = this.canvasScaler.referenceResolution.y;

        this.canvasScaler.matchWidthOrHeight = ( _aspectRatio < TARGET_ASPECT_RATIO ) ? 0.0f :
                                               ( _aspectRatio <= MAXIMUM_ASPECT_RATIO ) ? 1.0f - ( ( ( _aspectRatio - TARGET_ASPECT_RATIO ) / ( MAXIMUM_ASPECT_RATIO - TARGET_ASPECT_RATIO ) ) * 0.25f ) :
                                               0.75f + ( ( ( ( _aspectRatio - TARGET_ASPECT_RATIO ) / ( MAXIMUM_ASPECT_RATIO - TARGET_ASPECT_RATIO ) ) - 1.0f ) * 0.065f );

        this.uiContainer.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, _canvasScalerHeight );

        if (_aspectRatio > MAXIMUM_ASPECT_RATIO)
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

        this.mainCamera.orthographicSize = ( _aspectRatio < 1.6f ) ? 8.0f :
                                           ( _aspectRatio < 1.9f ) ? 6.2f :
                                                                     5.6f;
    }
}
