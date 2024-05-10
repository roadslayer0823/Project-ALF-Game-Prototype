using UnityEngine;
using UnityEngine.Events;

public class SwipeDetector : MonoBehaviour
{
    [SerializeField] private float minSwipeDistance = 30f;
    [SerializeField] private UnityEvent onSwipeLeftCallBack = null;
    [SerializeField] private UnityEvent onSwipeRightCallBack = null;
    [SerializeField] private UnityEvent onSwipeUpCallBack = null;
    [SerializeField] private UnityEvent onSwipeDownCallBack = null;

    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    public void SetTouchStartPos(Vector2 startPos)
    {
        this.touchStartPos = startPos;
    }

    public void SetTouchEndPos(Vector2 endPos)
    {
        this.touchEndPos = endPos;
    }

    public void DetectSwipe()
    {
        float swipeDistance = Vector2.Distance(this.touchEndPos, this.touchStartPos);

        if(swipeDistance < minSwipeDistance)
        {
            return;
        }

        Vector2 swipeDirection = touchEndPos - touchStartPos;

        float swipeDirectionX = Mathf.Abs(swipeDirection.x);
        float swipeDirectionY = Mathf.Abs(swipeDirection.y);

        if (swipeDirectionY > swipeDirectionX && swipeDirectionY > 30)
        {
            if(swipeDirection.y > 0)
            {
                onSwipeUpCallBack.Invoke();
            }
            else
            {
                onSwipeDownCallBack.Invoke();
            }
        }

        else if(swipeDirectionX > swipeDirectionY && swipeDirectionX > 30)
        {
            if(swipeDirection.x < 0)
            {
                onSwipeLeftCallBack.Invoke();
            }
            else
            {
                onSwipeRightCallBack.Invoke();
            }
        }
    }
}

