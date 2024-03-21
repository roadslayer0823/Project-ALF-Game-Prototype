using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDashboard : MonoBehaviour
{
    [SerializeField] private Button executeButton = null;

    private Action onExecuteButtonClickedCallback = null;
    private const string AUDIO_ID_EXECUTE = "execute";

    public void Initialize(Action onExecuteButtonClickedCallback)
    {
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;
    }

    public void ClickOnExecuteButton()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_EXECUTE);

        DisableExecuteButton();

        if (this.onExecuteButtonClickedCallback != null)
        {
            this.onExecuteButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onExecuteButtonClickedCallback' is not assigned.");
        }
    }

    public void EnableExecuteButton()
    {
        this.executeButton.interactable = true;
    }

    public void DisableExecuteButton()
    {
        this.executeButton.interactable = false;
    }

    public void ShowExecuteButton()
    {
        this.executeButton.gameObject.SetActive(true);
    }

    public void HideExecuteButton()
    {
        this.executeButton.gameObject.SetActive(false);
    }
}
