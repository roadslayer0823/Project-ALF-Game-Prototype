using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LongPressDetector : MonoBehaviour
{
    [SerializeField] private UnityEvent onLongPressedCallBack = null;
    [SerializeField] private float longPressDuration;
    private bool isLongPress;

    public void SetIsLongPress(bool currentLongPress)
    {
        this.isLongPress = currentLongPress;
    }

    public bool GetIsLongPress()
    {
        return this.isLongPress;
    }

    public void SkillSelectionLongPress(float startPressTime, float longPressDuration, bool currentLongPress)
    {
        if (Time.time - startPressTime >= longPressDuration)
        {
            isLongPress = currentLongPress;
            onLongPressedCallBack.Invoke();
        }
    }
}
