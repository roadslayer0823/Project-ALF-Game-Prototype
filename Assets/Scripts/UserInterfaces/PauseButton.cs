using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private Slider timeSpeedSlider = null;
    [SerializeField] private TMP_Text sliderValue = null;
    [SerializeField] private Image buttonImage = null;
    [SerializeField] private Sprite pauseButton = null;
    [SerializeField] private Sprite resumeButton = null;

    private bool isPaused = false;
    public static float currentTimeScale = 1.0f;

    public void ClickToToggle()
    {
        this.isPaused = !( this.isPaused );

        if (isPaused)
        {
            Time.timeScale = 0.0f;
            this.buttonImage.sprite = resumeButton;
        }
        else
        {
            Time.timeScale = 1.0f;
            this.buttonImage.sprite = pauseButton;
        }

        currentTimeScale = Time.timeScale;
    }

    public void SliderToggle()
    {
        currentTimeScale = this.timeSpeedSlider.normalizedValue;
    }

    public void SetSliderValue()
    {
        sliderValue.text = timeSpeedSlider.value.ToString("0.##");
    }
}
