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

        switch (this.currentDistanceLevel)
        {
            case DistanceLevel.Near:
                this.pointerLeft.rectTransform.position = this.nearDistanceLeft.position;
                this.pointerRight.rectTransform.position = this.nearDistanceRight.position;
                break;

            case DistanceLevel.Normal:
                this.pointerLeft.rectTransform.position = this.normalDistanceLeft.position;
                this.pointerRight.rectTransform.position = this.normalDistanceRight.position;
                break;

            case DistanceLevel.Far:
                this.pointerLeft.rectTransform.position = this.farDistanceLeft.position;
                this.pointerRight.rectTransform.position = this.farDistanceRight.position;
                break;
        }
    }

    public DistanceLevel GetCurrentDistanceLevel()
    {
        return currentDistanceLevel;
    }
}
