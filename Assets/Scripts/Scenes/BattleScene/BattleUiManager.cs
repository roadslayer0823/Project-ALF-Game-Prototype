using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUiManager : MonoBehaviour
{
    [SerializeField] private SkillSelectionPanel skillSelectionPanel = null;
    [SerializeField] private SkillSlotListPanel skillSlotListPanel = null;
    [SerializeField] private ATLSlotListPanel atlSlotListPanel = null;
    [SerializeField] private PlayerActionPanel playerActionPanel = null;

    private BattleGameManager battleGameManager = null;
    private GameCharacter selectedGameCharacter = null;

    public void Initialize( BattleGameManager battleGameManager )
    {
        this.battleGameManager = battleGameManager;
        this.skillSelectionPanel.Initialize( OnSkillSelectedFromSkillSelectionPanel, OnSkillDeselectedFromSkillSelectionPanel );
        this.playerActionPanel.Initialize( OnExecuteButtonClicked );
        this.skillSlotListPanel.Initialize(OnSkillSlotSwiped);
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
        this.selectedGameCharacter.AddSelectedSkill( skillSelectionBox.GetCharacterSkill() );
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
        this.skillSlotListPanel.Show();
    }

    public void HideSkillSlotListPanel()
    {
        this.skillSlotListPanel.Hide();
    }

    public void UpdateSkillSlotListPanel( GameCharacter gameCharacter )
    {
        this.skillSlotListPanel.Show( gameCharacter );
    }

    public SkillSlotListPanel GetSkillSlotListPanel()
    {
        return this.skillSlotListPanel;
    }

    public void OnSkillSlotSwiped()
    {
        this.atlSlotListPanel.OnSkillSlotUpdated();
    }

#endregion

#region ATL Slot List Panel

    public void ShowATLSlotListPanel( BattleFlowATL[] flowATLs )
    {
        this.atlSlotListPanel.Show( flowATLs, OnSkillSlotSwiped, OnATLSlotExecuted);
    }

    public void HideATLSlotListPanel()
    {
        this.atlSlotListPanel.Hide();
    }

    public void OnATLSlotExecuted()
    {
        this.skillSlotListPanel.SwipeLeft();
    }

#endregion

    #region Player Action Panel

    public void OnExecuteButtonClicked()
    {
        this.battleGameManager.StartExecution();
    }

#endregion
}
