using System;
using UnityEngine;
using UnityEngine.UI;

public class DistanceLevelSystem : MonoBehaviour
{
    [SerializeField] private Image pointerLeft;
    [SerializeField] private Image pointerRight;
    [SerializeField] private RectTransform nearDistanceLeft;
    [SerializeField] private RectTransform nearDistanceRight;
    [SerializeField] private RectTransform normalDistanceLeft;
    [SerializeField] private RectTransform normalDistanceRight;
    [SerializeField] private RectTransform farDistanceLeft;
    [SerializeField] private RectTransform farDistanceRight;
    [SerializeField] private DistanceLevel currentDistanceLevel = DistanceLevel.None; 

    public enum DistanceLevel
    {
        None,
        Near,
        Normal,
        Far    
    }

    public void SetCurrentDistanceLevel(DistanceLevel currentDistanceLevel)
    {
        this.currentDistanceLevel = currentDistanceLevel;
        (Vector2 distanceLeftPoint, Vector2 distanceRightPoint) = this.currentDistanceLevel switch
        {
            DistanceLevel.Near => (this.nearDistanceLeft.position, this.nearDistanceRight.position),
            DistanceLevel.Normal => (this.normalDistanceLeft.position, this.normalDistanceRight.position),
            DistanceLevel.Far => (this.farDistanceLeft.position, this.farDistanceRight.position),
            _ => throw new NotImplementedException()
        };
        this.pointerLeft.rectTransform.position = distanceLeftPoint;
        this.pointerRight.rectTransform.position = distanceRightPoint;
    }

    public DistanceLevel GetCurrentDistanceLevel()
    {
        return currentDistanceLevel;
    }
}
