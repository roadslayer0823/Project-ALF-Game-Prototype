using System;
using UnityEngine;
using UnityEngine.UI;

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

    private BattleDistanceManager battleDistanceManager = null;

    public void UpdatBattleeDistanceTypeUI(BattleDistanceManager.DistanceType currentDistanceType)
    {
        if (currentDistanceType == BattleDistanceManager.DistanceType.Near)
        {
            CurrentPointerPosition(nearDistanceLeft, nearDistanceRight);
        }
        else if (currentDistanceType == BattleDistanceManager.DistanceType.Normal)
        {
            CurrentPointerPosition(normalDistanceLeft, normalDistanceRight);
        }
        else if (currentDistanceType == BattleDistanceManager.DistanceType.Far)
        {
            CurrentPointerPosition(farDistanceLeft, farDistanceRight);
        }
    }

    public void CurrentPointerPosition(RectTransform DistanceLeftPosition, RectTransform DistanceRightPosition)
    {
        this.pointerLeft.rectTransform.position = DistanceLeftPosition.position;
        this.pointerRight.rectTransform.position = DistanceRightPosition.position;
    }
}
