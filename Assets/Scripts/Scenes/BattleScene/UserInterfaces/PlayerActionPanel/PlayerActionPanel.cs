using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject preparationSection = null;
    [SerializeField] private GameObject battleSection = null;
    [SerializeField] private Button executeButton = null;
    [SerializeField] private Button repulseButton = null;
    [SerializeField] private Button deriveButton = null;
    [SerializeField] private Button counterButton = null;
    [SerializeField] private SkillActionButton[] skillActionButtons = new SkillActionButton[ 0 ];

    private GameCharacter selectedGameCharacter = null;
    private Action onExecuteButtonClickedCallback = null;

    public enum QTEActionType
    {
        None,
        Repulse,
        Derive,
        Counter
    }

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
        HideQTEActionButton();
        this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Repulse );
    }

    public void ClickOnDeriveButton()
    {
        HideQTEActionButton();
        this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Derive );
    }

    public void ClickOnCounterButton()
    {
        HideQTEActionButton();
        this.selectedGameCharacter.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.Counter );
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

    public void ShowQTEActionButton( QTEActionType actionType )
    {
        HideQTEActionButton();

        switch ( actionType )
        {
            case QTEActionType.Repulse:

                this.repulseButton.gameObject.SetActive( true );

                break;

            case QTEActionType.Derive:

                this.deriveButton.gameObject.SetActive( true );

                break;

            case QTEActionType.Counter:

                this.counterButton.gameObject.SetActive( true );

                break;
        }
    }

    public void HideQTEActionButton()
    {
        this.repulseButton.gameObject.SetActive( false );
        this.deriveButton.gameObject.SetActive( false );
        this.counterButton.gameObject.SetActive( false );
    }

    public void ShowSkillActionButtons( CharacterSkill[] skills )
    {
        HideSkillActionButtons();

        for (int i = 0; i < skills.Length; i++)
        {
            if (i < this.skillActionButtons.Length)
            {
                SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
                _skillActionButton.SetSelectedSkill( skills[ i ] );
                _skillActionButton.gameObject.SetActive( true );
            }
        }
    }

    public void HideSkillActionButtons()
    {
        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            this.skillActionButtons[ i ].gameObject.SetActive( false );
        }
    }

    public void UpdateSkillActionButtons( bool canDefend, bool canEvade )
    {
        for (int i = 0; i < this.skillActionButtons.Length; i++)
        {
            SkillActionButton _skillActionButton = this.skillActionButtons[ i ];
            CharacterSkill _skill = _skillActionButton.GetSelectedSkill();
            if (_skill != null && _skill.IsSkillAvailable( canDefend, canEvade ))
            {
                _skillActionButton.EnableActionButton();
            }
            else
            {
                _skillActionButton.DisableActionButton();
            }
        }
    }
}
