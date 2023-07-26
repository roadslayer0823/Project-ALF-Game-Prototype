using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionPanel : MonoBehaviour
{
    [SerializeField] private Button executeButton = null;

    private Action onExecuteButtonClickedCallback = null;

    public void Initialize( Action onExecuteButtonClickedCallback )
    {
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;
    }

    public void ClickOnExecuteButton()
    {
        DisableExecuteButton();

        if (this.onExecuteButtonClickedCallback != null)
        {
            this.onExecuteButtonClickedCallback();
        }
        else
        {
            Debug.Log( "The value for 'onExecuteButtonClickedCallback' is not assigned." );
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
}
