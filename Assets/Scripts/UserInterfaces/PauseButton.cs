using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonLabel = null;
    [SerializeField] private Slider timeSpeedSlider = null;
    [SerializeField] private TMP_Text sliderValue = null;

    private bool isPaused = false;
    public static float currentTimeScale = 1.0f;

    public void ClickToToggle()
    {
        this.isPaused = !( this.isPaused );

        if (isPaused)
        {
            Time.timeScale = 0.0f;
            this.buttonLabel.text = "Resume";
        }
        else
        {
            Time.timeScale = 1.0f;
            this.buttonLabel.text = "Pause";
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
