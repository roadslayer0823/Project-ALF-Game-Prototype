using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private Image timerImage = null;

    private float countdownTime = 0.0f;
    private float startTime = 0.0f;
    private Coroutine timerCoroutine = null;

    public void StartCountdownTimer( float countdownTime )
    {
        this.countdownTime = countdownTime;
        this.startTime = Time.time;
        UpdateTimerImage();

        StopCountdownTimer();
        this.timerCoroutine = StartCoroutine( RunCountdownTimer() );
    }

    public void StopCountdownTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine( timerCoroutine );
            timerCoroutine = null;
        }

        this.timerImage.fillAmount = 1.0f;
    }

    private IEnumerator RunCountdownTimer()
    {
        this.timerImage.fillAmount = 0.0f;

        while (Time.time - this.startTime < this.countdownTime)
        {
            yield return null;
            UpdateTimerImage();
        }

        this.timerImage.fillAmount = 1.0f;
        this.timerCoroutine = null;
    }

    private void UpdateTimerImage()
    {
        this.timerImage.fillAmount = ( Time.time - this.startTime ) / this.countdownTime;
    }
}
