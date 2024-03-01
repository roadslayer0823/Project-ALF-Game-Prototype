using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SkillType = BattleSkillManager.SkillType;

public class BattleUiManager : MonoBehaviour
{
    [System.Obsolete][SerializeField] private PlayerActionPanel playerActionPanel = null;
    [System.Obsolete][SerializeField] private SkillSelectionPanel skillSelectionPanel = null;
    [System.Obsolete][SerializeField] private SkillSlotListPanel skillSlotListPanel = null;
    [System.Obsolete][SerializeField] private ATLSlotListPanel atlSlotListPanel = null;
    [SerializeField] private CharacterInfoPanel characterInfoPanel = null;
    [SerializeField] private BattleResultPanel battleResultPanel = null;
    [SerializeField] private TMP_Text instructionLabel = null;

    [Header( "Version 2" )]
    [SerializeField] private PreparationSection preparationSection = null;
    [SerializeField] private SkillSelectionPanelV2 skillSelectionPanelV2 = null;
    [SerializeField] private SkillInfoPanel skillInfoPanel = null;
    [SerializeField] private ActiveSkillSlotListPanelV2 activeSkillSlotListPanelV2 = null;
    [SerializeField] private BackendSkillSlotListPanel backendSkillSlotListPanel = null;
    [SerializeField] private ATLSlotListPanelV2 atlSlotListPanelV2 = null;

    private BattleGameManager battleGameManager = null;
    private GameCharacter selectedGameCharacter = null;

    public void Initialize( BattleGameManager battleGameManager )
    {
        this.battleGameManager = battleGameManager;

        if (this.preparationSection == null)
        {
            this.playerActionPanel.Initialize( OnExecuteButtonClicked, ShowActiveSkillSelectionPanel, ShowBackendSkillSelectionPanel );
        }
        else
        {
            this.preparationSection.Initialize( OnExecuteButtonClicked, ShowActiveSkillSelectionPanel, ShowBackendSkillSelectionPanel );
        }

        if (this.skillSelectionPanelV2 == null)
        {
            this.skillSelectionPanel.Initialize( OnSkillSelectedFromSkillSelectionPanel, OnSkillDeselectedFromSkillSelectionPanel );
        }
        else
        {
            this.skillSelectionPanelV2.Initialize( OnSkillSelectedFromSkillSelectionPanelV2, OnSkillDeselectedFromSkillSelectionPanelV2, ReturnToSkillMenu );
        }

        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Initialize( OnSkillSlotSwiped );
        }
        else
        {
            this.skillSlotListPanel.SetIsSkillSlotListScrollable( false );
            this.activeSkillSlotListPanelV2.Initialize();
            this.backendSkillSlotListPanel.Initialize();
        }

        this.atlSlotListPanelV2.Initialize();
    }

    public void SetSelectedGameCharacter( GameCharacter gameCharacter )
    {
        this.selectedGameCharacter = gameCharacter;
    }

    public void CheckWhetherToEnableExecuteButton()
    {
        if (this.selectedGameCharacter.GetSelectedActiveSkillList().Count > 0)
        {
            if (this.preparationSection == null)
            {
                this.playerActionPanel.EnableExecuteButton();
            }
            else
            {
                this.preparationSection.EnableExecuteButton();
            }
        }
        else
        {
            if (this.preparationSection == null)
            {
                this.playerActionPanel.DisableExecuteButton();
            }
            else
            {
                this.preparationSection.DisableExecuteButton();
            }
        }
    }

    public void SetAllActive( bool value )
    {
        if (this.preparationSection == null)
        {
            this.playerActionPanel.gameObject.SetActive( value );
        }
        else
        {
            this.preparationSection.gameObject.SetActive( value );
        }

        if (this.skillSelectionPanelV2 == null)
        {
            this.skillSelectionPanel.gameObject.SetActive( value );
        }
        else
        {
            this.skillSelectionPanelV2.gameObject.SetActive( value );
        }

        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.gameObject.SetActive( value );
        }
        else
        {
            this.activeSkillSlotListPanelV2.gameObject.SetActive( value );
            this.backendSkillSlotListPanel.gameObject.SetActive( value );
        }

        if (this.atlSlotListPanelV2 == null)
        {
            this.atlSlotListPanel.gameObject.SetActive( value );
        }
        else
        {
            this.atlSlotListPanelV2.gameObject.SetActive( value );
        }

        this.characterInfoPanel.gameObject.SetActive( value );
        this.battleResultPanel.gameObject.SetActive( value );
    }

    public void PlayInstructionAnimation()
    {
        this.instructionLabel.transform.localScale = Vector3.zero;
        LeanTween.scale( this.instructionLabel.gameObject, Vector3.one, 0.9f ).setEaseOutCubic().setLoopPingPong( 1 ).setOnComplete( () =>
        {
            this.instructionLabel.gameObject.SetActive( false );
        } );
    }

    public void ShowBattleSection()
    {
        this.skillInfoPanel.Hide();
        this.preparationSection.Hide();
    }

#region Player Action Panel

    public void ShowPreparationSection()
    {
        this.playerActionPanel.gameObject.SetActive( true );
        this.playerActionPanel.ShowPreparationSection();
        this.playerActionPanel.ShowActiveSkillSelectionPanel();
        this.playerActionPanel.HideBattleSection();
    }

    public void ShowBattleSection( GameCharacter gameCharacter )
    {
        DisablePlayerActionPanelButtons();
        this.playerActionPanel.ShowSkillActionButtons( gameCharacter.GetSelectedBackendSkillList().ToArray() );
        this.playerActionPanel.ShowBattleSection();
        this.playerActionPanel.HidePreparationSection();
    }

    public void UpdatePlayerActionPanelCharacter( GameCharacter gameCharacter )
    {
        this.playerActionPanel.SetSelectedGameCharacter( gameCharacter );
    }

    public void UpdatePlayerActionPanelButtons( CharacterSkill qteSkill, bool canDefend, bool canEvade, bool canObserve, float countdownTime )
    {
        this.playerActionPanel.ShowQTEActionButton( qteSkill, countdownTime );
        this.playerActionPanel.UpdateSkillActionButtons( canDefend, canEvade, canObserve, countdownTime );
    }

    public void DisablePlayerActionPanelButtons()
    {
        UpdatePlayerActionPanelButtons( null, false, false, false, 0.0f );
    }

    public void OnExecuteButtonClicked()
    {
        this.skillSelectionPanel.CheckForNecessarySkill();
        this.battleGameManager.StartExecution();
    }

    private void ShowActiveSkillSelectionPanel()
    {
        if (this.preparationSection == null)
        {
            this.skillSelectionPanel.ShowActiveSkillSelectionPanel();
        }
        else
        {
            this.preparationSection.HideSkillMenu();
            ShowSkillSelectionPanel();
        }
    }

    private void ShowBackendSkillSelectionPanel()
    {
        if (this.preparationSection == null)
        {
            this.skillSelectionPanel.ShowBackendSkillSelectionPanel();
        }
        else
        {
            this.preparationSection.HideSkillMenu();
            ShowSkillSelectionPanel();
        }
    }

    public PlayerActionPanel GetPlayerActionPanel()
    {
        return this.playerActionPanel;
    }

#endregion

#region Preparation Section

    public void ReturnToSkillMenu()
    {
        HideSkillSelectionPanel();
        this.skillInfoPanel.Hide();
        this.preparationSection.ShowSkillMenu();
    }

    public PreparationSection GetPreparationSection()
    {
        return this.preparationSection;
    }

#endregion

#region Skill Selection Panel

    public void ShowSkillSelectionPanel()
    {
        if (this.skillSelectionPanelV2 == null)
        {
            this.skillSelectionPanel.Show( this.selectedGameCharacter );
        }
        else
        {
            this.skillSelectionPanelV2.Show( this.selectedGameCharacter );
        }
    }

    public void HideSkillSelectionPanel()
    {
        if (this.skillSelectionPanelV2 == null)
        {
            this.skillSelectionPanel.Hide();
        }
        else
        {
            this.skillSelectionPanelV2.HideSkillSelectionPanel();
        }
    }

    public void OnSkillSelectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
        if (!skillSelectionBox.CheckIsSkillLevelChanged())
        {
            this.selectedGameCharacter.AddSelectedSkill(skillSelectionBox.GetCharacterSkill());
        }
        
        UpdateSkillSlotListPanel( this.selectedGameCharacter );
        CheckWhetherToEnableExecuteButton();
    }

    public void OnSkillSelectedFromSkillSelectionPanelV2( SkillSelectionBoxV2 skillSelectionBox )
    {
        this.selectedGameCharacter.AddSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel(this.selectedGameCharacter);
        CheckWhetherToEnableExecuteButton();
    }

    public void OnSkillDeselectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
        this.selectedGameCharacter.RemoveSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel( this.selectedGameCharacter );
        CheckWhetherToEnableExecuteButton();
    }

    public void OnSkillDeselectedFromSkillSelectionPanelV2( SkillSelectionBoxV2 skillSelectionBox )
    {
        this.selectedGameCharacter.RemoveSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel(this.selectedGameCharacter);
        CheckWhetherToEnableExecuteButton();
    }

    public SkillSelectionPanel GetSkillSelectionPanel()
    {
        return this.skillSelectionPanel;
    }

#endregion

#region Skill Slot List Panel

    public void ShowSkillSlotListPanel()
    {
        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Show();
        }
        else
        {
            this.activeSkillSlotListPanelV2.Show();
            this.backendSkillSlotListPanel.Show();
        }
    }

    public void HideSkillSlotListPanel()
    {
        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Hide();
        }
        else
        {
            this.activeSkillSlotListPanelV2.Hide();
            this.backendSkillSlotListPanel.Hide();
        }
    }

    public void UpdateSkillSlotListPanel( GameCharacter gameCharacter )
    {
        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.UpdateSkillSlots( gameCharacter );
        }
        else
        {
            //this.skillSlotListPanelV2.UpdateSkillSlots( gameCharacter );
        }
    }

    public SkillSlotListPanel GetSkillSlotListPanel()
    {
        return this.skillSlotListPanel;
    }

    public ActiveSkillSlotListPanelV2 GetActiveSkillSlotListPanelV2()
    {
        return this.activeSkillSlotListPanelV2;
    }

    public BackendSkillSlotListPanel GetBackendSkillSlotListPanel()
    {
        return this.backendSkillSlotListPanel;
    }

    public void OnSkillSlotSwiped()
    {
        this.atlSlotListPanel.OnSkillSlotUpdated();
    }

    public bool IsUsingSkillSlotListPanelV2()
    {
        return ( this.activeSkillSlotListPanelV2 != null );
    }

#endregion

#region ATL Slot List Panel

    public void ShowATLSlotListPanel( BattleFlowATL[] flowATLs )
    {
        this.atlSlotListPanel.Show( flowATLs, OnSkillSlotSwiped, OnATLSlotExecuted );

        if (this.atlSlotListPanelV2 != null)
        {
            this.atlSlotListPanel.gameObject.SetActive( false );
            this.atlSlotListPanelV2.SetUp( flowATLs );
            this.atlSlotListPanelV2.gameObject.SetActive( true );
        }
    }

    public void ShowATLSlotListPanel( BattleFlowATL_V2[] flowATLs )
    {
        this.atlSlotListPanelV2.SetUp( flowATLs );
        this.atlSlotListPanelV2.gameObject.SetActive( true );
    }

    public void HideATLSlotListPanel()
    {
        this.atlSlotListPanel.Hide();

        if (this.atlSlotListPanelV2 != null)
        {
            this.atlSlotListPanelV2.gameObject.SetActive( false );
        }
    }

    public void OnATLSlotExecuted()
    {
        if (this.activeSkillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.SwipeLeft();
        }
        else
        {
            this.activeSkillSlotListPanelV2.ClickBottom();
        }
    }

    public ATLSlotListPanelV2 GetATLSlotListPanelV2()
    {
        return this.atlSlotListPanelV2;
    }

#endregion

#region Character Info Panel

    public CharacterInfoPanel GetCharacterInfoPanel()
    {
        return this.characterInfoPanel;
    }

#endregion

#region Battle Result Panel

    public void ShowVictoryResult()
    {
        this.battleResultPanel.ShowVictory();
    }

    public void ShowDefeatResult()
    {
        this.battleResultPanel.ShowDefeat();
    }

#endregion

#region Battle Skill Manager

    public void UpdateSkillButtons( List<SkillType> skillTypeList )
    {
        Debug.Log( "UpdateSkillButtons" );
        if (skillTypeList.Contains( SkillType.Repulse ))
        {
            Debug.Log( "SkillType.Repulse" );
            this.activeSkillSlotListPanelV2.ChangeToRepulseMode( this.selectedGameCharacter );

            CharacterSkill _currentSkill = this.selectedGameCharacter.GetCurrentSkill();
            if (_currentSkill != null)
            {
                if (_currentSkill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
                {
                    CharacterSkill _repulseSkill = _currentSkill.GetCharacterSubskillData().GetSelectedRepulseSkill();
                    this.selectedGameCharacter.SetAssignedSkill( _repulseSkill );
                    this.activeSkillSlotListPanelV2.GetSkillSlot( _repulseSkill ).SelectSkill();
                }
            }
        }
        else if (skillTypeList.Contains( SkillType.Derive ))
        {
            Debug.Log( "SkillType.Derive" );
            this.activeSkillSlotListPanelV2.ChangeToDerivedMode( this.selectedGameCharacter );

            if (skillTypeList.Contains( SkillType.Active ))
            {
                this.activeSkillSlotListPanelV2.EnableInteraction();
            }
            else
            {
                this.activeSkillSlotListPanelV2.DisableInteraction();
            }
        }
        else
        {
            Debug.Log( "Default" );
            this.activeSkillSlotListPanelV2.ChangeToDefaultMode( this.selectedGameCharacter, ( skillTypeList.Contains( SkillType.Active ) ) ? SkillSlotV2.StateType.Enabled : SkillSlotV2.StateType.Disabled );
        }

        if (this.backendSkillSlotListPanel == null)
        {
            this.playerActionPanel.ShowSkillActionButtons( this.selectedGameCharacter.GetSelectedBackendSkillList().ToArray() );
            this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Defense, skillTypeList.Contains( SkillType.Defend ) );
            this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Evasion, skillTypeList.Contains( SkillType.Evade ) );
            this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Observation, skillTypeList.Contains( SkillType.Observe ) );
        }
        else
        {
            if (this.preparationSection == null)
            {
                this.playerActionPanel.gameObject.SetActive( false );
            }

            this.backendSkillSlotListPanel.UpdateBackendSkillSlots( this.selectedGameCharacter, skillTypeList );
        }
    }

    public void OnSkillBeingUsed()
    {
        CharacterSkill _currentSkill = this.selectedGameCharacter.GetCurrentSkill();
        SkillSlotV2 _skillSlot = this.activeSkillSlotListPanelV2.GetSkillSlot( _currentSkill );

        if (_skillSlot == null)
        {
            _skillSlot = this.backendSkillSlotListPanel.GetSkillSlot( _currentSkill );
        }

        if (_skillSlot != null)
        {
            _skillSlot.SetCurrentStateType( SkillSlotV2.StateType.Activated );
        }
    }

#endregion
}
