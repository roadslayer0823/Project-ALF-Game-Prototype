using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DoubleTapDetector : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private UnityEvent onDoubleTapCallBack = null;
    [SerializeField] private float tapDelay = 0.2f;

    private bool isPointerDown = false;
    private bool isDoubleTap = false;
    private bool isWaitingSecondTap = true;
    private int tapCount = 0;
    private float lastTapTime = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        this.isPointerDown = true;
    }

    public void StartTap()
    {
        if(this.isPointerDown)
        {
            if (this.lastTapTime > 0)
            {
                this.isWaitingSecondTap = Time.time - this.lastTapTime <= this.tapDelay;
                if (!this.isWaitingSecondTap)
                {
                    this.tapCount = 0;
                    this.lastTapTime = 0;
                }
            }
            else
            {
                this.isWaitingSecondTap = true;
            }
            this.tapCount += 1;

            if (this.tapCount > 1)
            {
                this.isDoubleTap = true;
            }
            else
            {
                this.isDoubleTap = false;
            }

            onDoubleTapCallBack.Invoke();
            this.lastTapTime = Time.time;

        }
    }

    public bool IsDoubleTap()
    {
        return this.isDoubleTap;
    }

    public void ResetDoubleTapValue()
    {
        this.tapCount = 0;
        this.isWaitingSecondTap = false;
        this.isDoubleTap = false;
    }
}
