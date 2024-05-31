using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private UnityEvent onLongPressedCallBack = null;
    [SerializeField] private float longPressDuration;

    private float pressTime = 2f;
    private float timePressStarted;
    private bool isLongPress;
    private bool isPointerDown = false;

    public void Update()
    {
        if (GetIsPointerDown() && !GetIsLongPress())
        {
            SkillSelectionLongPress(this.timePressStarted, this.pressTime, true);
        }
    }

    public void SkillSelectionLongPress(float startPressTime, float longPressDuration, bool currentLongPress)
    {
        if (Time.time - startPressTime >= longPressDuration)
        {
            isLongPress = currentLongPress;
            onLongPressedCallBack.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetIsPointerDown(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.timePressStarted = Time.time;
        SetIsPointerDown(true);
        SetIsLongPress(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetIsLongPress(true);
    }

    public void SetIsLongPress(bool currentLongPress)
    {
        this.isLongPress = currentLongPress;
    }

    public void SetIsPointerDown(bool currentPointerDown)
    {
        this.isPointerDown = currentPointerDown;
    }

    public bool GetIsLongPress()
    {
        return this.isLongPress;
    }

    public bool GetIsPointerDown()
    {
        return this.isPointerDown;
    }
}
