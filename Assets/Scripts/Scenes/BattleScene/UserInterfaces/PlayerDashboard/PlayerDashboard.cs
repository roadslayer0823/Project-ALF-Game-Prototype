using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDashboard : MonoBehaviour
{
    [SerializeField] private ATLSlotListPanelV3 atlSlotListPanelV3 = null;
    [SerializeField] private CharacterInfoPanelV2 characterInfoPanelV2 = null;
    [SerializeField] private GameObject executeButtonContainer = null;
    [SerializeField] private Button executeButton = null;

    private Action onExecuteButtonClickedCallback = null;
    private const string AUDIO_ID_EXECUTE = "execute";

    public void Initialize(Action onExecuteButtonClickedCallback)
    {
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;
        this.atlSlotListPanelV3.Initialize();
    }

    public void ClickOnExecuteButton()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_EXECUTE);

        HideExecuteButtonContainer();

        if (this.onExecuteButtonClickedCallback != null)
        {
            this.onExecuteButtonClickedCallback();
        }
        else
        {
            Debug.Log("The value for 'onExecuteButtonClickedCallback' is not assigned.");
        }
    }

    public void ShowExecuteButtonContainer()
    {
        this.executeButtonContainer.SetActive( true );
    }

    public void HideExecuteButtonContainer()
    {
        this.executeButtonContainer.SetActive( false );
    }

    public void EnableExecuteButton()
    {
        this.executeButton.interactable = true;
    }

    public void DisableExecuteButton()
    {
        this.executeButton.interactable = false;
    }

    public ATLSlotListPanelV3 GetAtlSlotListPanelV3()
    {
        return this.atlSlotListPanelV3;
    }

    public CharacterInfoPanelV2 GetCharacterInfoPanelV2()
    {
        return this.characterInfoPanelV2;
    }
}
