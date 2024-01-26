using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SkillType = BattleSkillManager.SkillType;

public class BattleUiManager : MonoBehaviour
{
    [SerializeField] private SkillSelectionPanel skillSelectionPanel = null;
    [System.Obsolete][SerializeField] private SkillSlotListPanel skillSlotListPanel = null;
    [SerializeField] private SkillSlotListPanelV2 skillSlotListPanelV2 = null;
    [System.Obsolete][SerializeField] private ATLSlotListPanel atlSlotListPanel = null;
    [SerializeField] private ATLSlotListPanelV2 atlSlotListPanelV2 = null;
    [SerializeField] private PlayerActionPanel playerActionPanel = null;
    [SerializeField] private CharacterInfoPanel characterInfoPanel = null;
    [SerializeField] private BattleResultPanel battleResultPanel = null;
    [SerializeField] private TMP_Text instructionLabel = null;

    private BattleGameManager battleGameManager = null;
    private GameCharacter selectedGameCharacter = null;

    public void Initialize( BattleGameManager battleGameManager )
    {
        this.battleGameManager = battleGameManager;
        this.skillSelectionPanel.Initialize( OnSkillSelectedFromSkillSelectionPanel, OnSkillDeselectedFromSkillSelectionPanel );
        this.playerActionPanel.Initialize( OnExecuteButtonClicked, ShowActiveSkillSelectionPanel, ShowBackendSkillSelectionPanel);

        if (this.skillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Initialize( OnSkillSlotSwiped );
        }
        else
        {
            this.skillSlotListPanel.SetIsSkillSlotListScrollable( false );
            this.skillSlotListPanelV2.Initialize();
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
            this.playerActionPanel.EnableExecuteButton();
        }
        else
        {
            this.playerActionPanel.DisableExecuteButton();
        }
    }

    public void SetAllActive( bool value )
    {
        this.skillSelectionPanel.gameObject.SetActive( value );

        if (this.skillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.gameObject.SetActive( value );
        }
        else
        {
            this.skillSlotListPanelV2.gameObject.SetActive( value );
        }

        if (this.atlSlotListPanelV2 == null)
        {
            this.atlSlotListPanel.gameObject.SetActive( value );
        }
        else
        {
            this.atlSlotListPanelV2.gameObject.SetActive( value );
        }

        this.playerActionPanel.gameObject.SetActive( value );
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

#region Skill Selection Panel

    public void ShowSkillSelectionPanel()
    {
        this.skillSelectionPanel.Show( this.selectedGameCharacter );
    }

    public void HideSkillSelectionPanel()
    {
        this.skillSelectionPanel.Hide();
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

    public void OnSkillDeselectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
        this.selectedGameCharacter.RemoveSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel( this.selectedGameCharacter );
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
        if (this.skillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Show();
        }
        else
        {
            this.skillSlotListPanelV2.Show();
        }
    }

    public void HideSkillSlotListPanel()
    {
        if (this.skillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.Hide();
        }
        else
        {
            this.skillSlotListPanelV2.Hide();
        }
    }

    public void UpdateSkillSlotListPanel( GameCharacter gameCharacter )
    {
        if (this.skillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.UpdateSkillSlots( gameCharacter );
        }
        else
        {
            this.skillSlotListPanelV2.UpdateSkillSlots( gameCharacter );
        }
    }

    public SkillSlotListPanel GetSkillSlotListPanel()
    {
        return this.skillSlotListPanel;
    }

    public SkillSlotListPanelV2 GetSkillSlotListPanelV2()
    {
        return this.skillSlotListPanelV2;
    }

    public void OnSkillSlotSwiped()
    {
        this.atlSlotListPanel.OnSkillSlotUpdated();
    }

    public bool IsUsingSkillSlotListPanelV2()
    {
        return ( this.skillSlotListPanelV2 != null );
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
        if (this.skillSlotListPanelV2 == null)
        {
            this.skillSlotListPanel.SwipeLeft();
        }
        else
        {
            this.skillSlotListPanelV2.ClickBottom();
        }
    }

    public ATLSlotListPanelV2 GetATLSlotListPanelV2()
    {
        return this.atlSlotListPanelV2;
    }

#endregion

#region Player Action Panel

    public void ShowPreparationSection()
    {
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
        this.skillSelectionPanel.ShowActiveSkillSelectionPanel();
    }

    private void ShowBackendSkillSelectionPanel()
    {
        this.skillSelectionPanel.ShowBackendSkillSelectionPanel();
    }

    public PlayerActionPanel GetPlayerActionPanel()
    {
        return this.playerActionPanel;
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
            this.skillSlotListPanelV2.ChangeToRepulseMode( this.selectedGameCharacter );

            CharacterSkill _currentSkill = this.selectedGameCharacter.GetCurrentSkill();
            if (_currentSkill != null)
            {
                this.selectedGameCharacter.SetCurrentSkill( _currentSkill.GetCharacterSubskillData().GetSelectedRepulseSkill() );
            }
        }
        else if (skillTypeList.Contains( SkillType.Derive ))
        {
            Debug.Log( "SkillType.Derive" );
            this.skillSlotListPanelV2.ChangeToDerivedMode( this.selectedGameCharacter );

            if (skillTypeList.Contains( SkillType.Active ))
            {
                this.skillSlotListPanelV2.EnableInteraction();
            }
            else
            {
                this.skillSlotListPanelV2.DisableInteraction();
            }
        }
        else
        {
            Debug.Log( "Default" );
            this.skillSlotListPanelV2.ChangeToDefaultMode( this.selectedGameCharacter );
        }

        this.playerActionPanel.ShowSkillActionButtons( this.selectedGameCharacter.GetSelectedBackendSkillList().ToArray() );
        this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Defense, skillTypeList.Contains( SkillType.Defend ) );
        this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Evasion, skillTypeList.Contains( SkillType.Evade ) );
        this.playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Observation, skillTypeList.Contains( SkillType.Observe ) );
    }

#endregion
}
