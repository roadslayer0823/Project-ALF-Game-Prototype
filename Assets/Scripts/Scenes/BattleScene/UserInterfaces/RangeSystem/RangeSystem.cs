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

    public void UpdateCurrentRangePosition(int currentRange)
    {
        if(currentRange == 1)
        {
            SetToMeleePosition();
        }
        else if(currentRange == 2)
        {
            SetToMiddlePosition();
        }
        else if(currentRange == 3)
        {
            SetToLongPosition();
        }
    }

    public void SetToMeleePosition()
    {
        pointer1.rectTransform.position = meleePosition1.position;
        pointer2.rectTransform.position = meleePosition2.position;
    }

    public void SetToMiddlePosition()
    {
        pointer1.rectTransform.position = middlePosition1.position;
        pointer2.rectTransform.position = middlePosition2.position;
    }

    public void SetToLongPosition()
    {
        pointer1.rectTransform.position = longPosition1.position;
        pointer2.rectTransform.position = longPosition2.position;
    }
}
