using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject preparationSection = null;
    [SerializeField] private GameObject battleSection = null;
    [SerializeField] private Button executeButton = null;
    [SerializeField] private Button repulseButton = null;
    [SerializeField] private Button defendButton = null;
    [SerializeField] private Button evadeButton = null;
    [SerializeField] private Button counterButton = null;

    private GameCharacter selectedGameCharacter = null;
    private Action onExecuteButtonClickedCallback = null;

    public void Initialize( Action onExecuteButtonClickedCallback )
    {
        this.onExecuteButtonClickedCallback = onExecuteButtonClickedCallback;
    }

    public void SetSelectedGameCharacter( GameCharacter selectedGameCharacter )
    {
        this.selectedGameCharacter = selectedGameCharacter;
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

    public void ClickOnRepulseButton()
    {
        DisableRepulseButton();
        selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Repulse );
    }

    public void ClickOnDefendButton()
    {
        DisableDefendButton();
        selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Defend );
    }

    public void ClickOnEvadeButton()
    {
        DisableEvadeButton();
        selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Evade );
    }

    public void ClickOnCounterButton()
    {
        DisableCounterButton();
        selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Counter );
    }

    public void ShowPreparationSection()
    {
        this.preparationSection.SetActive( true );
    }

    public void HidePreparationSection()
    {
        this.preparationSection.SetActive( false );
    }

    public void ShowBattleSection()
    {
        this.battleSection.SetActive( true );
    }

    public void HideBattleSection()
    {
        this.battleSection.SetActive( false );
    }

    public void EnableExecuteButton()
    {
        this.executeButton.interactable = true;
    }

    public void DisableExecuteButton()
    {
        this.executeButton.interactable = false;
    }

    public void EnableRepulseButton()
    {
        this.repulseButton.interactable = true;
    }

    public void DisableRepulseButton()
    {
        this.repulseButton.interactable = false;
    }

    public void EnableDefendButton()
    {
        this.defendButton.interactable = true;
    }

    public void DisableDefendButton()
    {
        this.defendButton.interactable = false;
    }

    public void EnableEvadeButton()
    {
        this.evadeButton.interactable = true;
    }

    public void DisableEvadeButton()
    {
        this.evadeButton.interactable = false;
    }

    public void EnableCounterButton()
    {
        this.counterButton.interactable = true;
    }

    public void DisableCounterButton()
    {
        this.counterButton.interactable = false;
    }
}
