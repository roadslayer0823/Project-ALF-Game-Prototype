using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDashboard : MonoBehaviour
{
    [SerializeField] private ATLSlotListPanelV3 atlSlotListPanelV3 = null;
    [SerializeField] private CharacterInfoPanelV2 characterInfoPanelV2 = null;
    [SerializeField] private GameObject executeButtonContainer = null;
    [SerializeField] private Button executeButton = null;
    [SerializeField] private Animator characterInfoPanelAnimator;

    private Action onExecuteButtonClickedCallback = null;
    private const string AUDIO_ID_EXECUTE = "execute";
    private const string ANIMATION_ID_CHARACTER_INFO_SETUP = "CharacterInfoPanelSetup";

    public void Initialize(Action onExecuteButtonClickedCallback)
    {
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;
        this.atlSlotListPanelV3.Initialize();
    }

    public void ClickOnExecuteButton()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_EXECUTE );
        ProcessExecuteButton();
    }

    public void ProcessExecuteButton()
    {
        HideExecuteButtonContainer();

        if (this.onExecuteButtonClickedCallback != null)
        {
            this.onExecuteButtonClickedCallback();
        }
        else
        {
            Debug.Log( "The value for 'onExecuteButtonClickedCallback' is not assigned." );
        }
    }

    public void PlayCharacterInfoPanelSetupAnimation()
    {
        this.characterInfoPanelAnimator.Play(ANIMATION_ID_CHARACTER_INFO_SETUP, 0, 0);
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
