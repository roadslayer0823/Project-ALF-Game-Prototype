using UnityEngine;
using UnityEngine.UI;

public class RangeSystem : MonoBehaviour
{
    [SerializeField] private Image pointer1;
    [SerializeField] private Image pointer2;
    [SerializeField] private RectTransform meleePosition1;
    [SerializeField] private RectTransform meleePosition2;
    [SerializeField] private RectTransform middlePosition1;
    [SerializeField] private RectTransform middlePosition2;
    [SerializeField] private RectTransform longPosition1;
    [SerializeField] private RectTransform longPosition2;
    [SerializeField] private Range currentRange = Range.none; 

    public enum Range
    {
        none,
        meleeRange,
        middleRange,
        longRange    
    }

    public void UpdateCurrentRangePosition(Range currentRange)
    {
        if(currentRange == Range.meleeRange)
        {
            SetToMeleePosition();
        }
        else if(currentRange == Range.middleRange)
        {
            SetToMiddlePosition();
        }
        else if(currentRange == Range.longRange)
        {
            SetToLongPosition();
        }
    }

    public void SetToMeleePosition()
    {
        SetCurrentRange(Range.meleeRange);
        pointer1.rectTransform.position = meleePosition1.position;
        pointer2.rectTransform.position = meleePosition2.position;
    }

    public void SetToMiddlePosition()
    {
        SetCurrentRange(Range.middleRange);
        pointer1.rectTransform.position = middlePosition1.position;
        pointer2.rectTransform.position = middlePosition2.position;
    }

    public void SetToLongPosition()
    {
        SetCurrentRange(Range.longRange);
        pointer1.rectTransform.position = longPosition1.position;
        pointer2.rectTransform.position = longPosition2.position;
    }

    public void SetCurrentRange(Range currentRangeStatus)
    {
        currentRange = currentRangeStatus;
    }

    public Range GetCurrentRange()
    {
        return currentRange;
    }
}
