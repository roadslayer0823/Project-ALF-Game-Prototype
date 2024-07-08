using UnityEngine;
using UnityEngine.UI;
using DistanceType = BattleDistanceManager.DistanceType;

public class BattleDistancePanel : MonoBehaviour
{
    [SerializeField] private Image pointerLeft;
    [SerializeField] private Image pointerRight;
    [SerializeField] private RectTransform nearDistanceLeft;
    [SerializeField] private RectTransform nearDistanceRight;
    [SerializeField] private RectTransform normalDistanceLeft;
    [SerializeField] private RectTransform normalDistanceRight;
    [SerializeField] private RectTransform farDistanceLeft;
    [SerializeField] private RectTransform farDistanceRight;

    public void UpdatBattleDistanceType( DistanceType currentDistanceType )
    {
        switch ( currentDistanceType )
        {
            case DistanceType.Near:

                CurrentPointerPosition( nearDistanceLeft, nearDistanceRight );

                break;

            case DistanceType.Normal:

                CurrentPointerPosition( normalDistanceLeft, normalDistanceRight );

                break;

            case DistanceType.Far:

                CurrentPointerPosition( farDistanceLeft, farDistanceRight );

                break;
        }
    }

    public void CurrentPointerPosition(RectTransform DistanceLeftPosition, RectTransform DistanceRightPosition)
    {
        this.pointerLeft.rectTransform.position = DistanceLeftPosition.position;
        this.pointerRight.rectTransform.position = DistanceRightPosition.position;
    }
}
