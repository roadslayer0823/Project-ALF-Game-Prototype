using UnityEngine;
using UnityEngine.UI;

public class GameObjectPositionHandler : MonoBehaviour
{
    [SerializeField] private float AspectRatioX = 0.0f;
    [SerializeField] private float AspectRatioY = 0.0f;
    [SerializeField] private Transform BattleFieldPosition;
    [SerializeField] private RectTransform UiMiddleFrame;
    [SerializeField] private RectTransform PreparationSection;
    [SerializeField] private RectTransform BattleLog;
    [SerializeField] private RectTransform PassiveSkillSelectionList;
    [SerializeField] private RectTransform ActiveSkillSlotList;
    [SerializeField] private RectTransform BackendSkillSlotList;
    [SerializeField] private RectTransform SkillSelectionPanelList;
    [SerializeField] private RectTransform DebugContainer;
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private Camera mainCamera;

    private const float maximumAspectRatio = 19.5f / 9.0f;

    public void Initialize()
    {
        float _aspectRatio = (float)Screen.width / (float)Screen.height;
        float _targetAspectRatio = AspectRatioX / AspectRatioY;

       
        this.canvasScaler.matchWidthOrHeight = (_aspectRatio < _targetAspectRatio) ? 0.0f :
                                               (_aspectRatio <= maximumAspectRatio) ? 1.0f - (((_aspectRatio - _targetAspectRatio) / (maximumAspectRatio - _targetAspectRatio)) * 0.25f) :
                                               0.75f + ((((_aspectRatio - _targetAspectRatio) / (maximumAspectRatio - _targetAspectRatio)) - 1.0f) * 0.065f);

        if(_aspectRatio > _targetAspectRatio)
        {
            //anchor position
            this.PassiveSkillSelectionList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2300);
            this.BattleLog.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2350);
            this.UiMiddleFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2300);
            this.DebugContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2200);
            this.PreparationSection.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2350);
            this.ActiveSkillSlotList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2300);
            this.BackendSkillSlotList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2400);
            this.SkillSelectionPanelList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2300);

            //position
            Vector3 _battleFieldPosition = BattleFieldPosition.position;
            _battleFieldPosition.y = -0.5f;
            BattleFieldPosition.position = _battleFieldPosition;
        }
        this.mainCamera.orthographicSize = (_aspectRatio < 1.6f) ? 8.0f : (_aspectRatio < 1.9f) ? 6.2f : 5.6f;
    }
}
