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

    public void Initialize()
    {
        float _aspectRatio = (float)Screen.width / (float)Screen.height;
        float _targetAspectRatio = AspectRatioX / AspectRatioY;
        float _18_divede_9 = 18.0f / 9.0f;
        float _22_divide_9 = 22.0f / 9.0f;
        if(_aspectRatio > _targetAspectRatio + 0.01f)
        {
            //anchor position
            this.PassiveSkillSelectionList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2350);
            this.BattleLog.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2500);
            this.UiMiddleFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2350);
            this.DebugContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2200);
            this.PreparationSection.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2400);
            this.ActiveSkillSlotList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2250);
            this.BackendSkillSlotList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2350);
            this.SkillSelectionPanelList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2350);

            if (_aspectRatio >= _18_divede_9 + 0.3f || _aspectRatio <= _18_divede_9 + 0.3f)
            {
                this.BattleLog.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2600);
            }
            else if (_aspectRatio > _22_divide_9)
            {
                this.DebugContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2300);
                this.BattleLog.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3000);
                this.PreparationSection.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2650);
                this.ActiveSkillSlotList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2350);
                this.SkillSelectionPanelList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2400);
                this.UiMiddleFrame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2450);
                this.PassiveSkillSelectionList.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2450);
            }
            Debug.Log("aspect ratio:" +_aspectRatio);
            //position
            Vector3 _battleFieldPosition = BattleFieldPosition.position;
            _battleFieldPosition.y = -0.5f;
            BattleFieldPosition.position = _battleFieldPosition;
        }
    }
}
