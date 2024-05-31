using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonLabel = null;
    [SerializeField] private Slider timeSpeedSlider = null;

    private bool isPaused = false;
    public static float currentTimeScale = 1.0f;

    public void ClickToToggle()
    {
        isPaused = !( isPaused );

        if (isPaused)
        {
            Time.timeScale = 0.0f;
            buttonLabel.text = "Resume";
        }
        else
        {
            Time.timeScale = 1.0f;
            buttonLabel.text = "Pause";
        }

        currentTimeScale = Time.timeScale;
    }

    public void SliderToggle()
    {
        currentTimeScale = timeSpeedSlider.normalizedValue;
    }
}
