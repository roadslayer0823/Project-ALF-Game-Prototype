using UnityEngine;
using TMPro;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private TMP_Text buttonLabel = null;

    private bool isPaused = false;

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
    }
}
