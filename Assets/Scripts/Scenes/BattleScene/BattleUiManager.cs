using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUiManager : MonoBehaviour
{
    [SerializeField] private SkillSelectionPanel skillSelectionPanel = null;
    [SerializeField] private SkillSlotListPanel skillSlotListPanel = null;

    private BattleGameManager battleGameManager = null;
    private GameCharacter selectedGameCharacter = null;

    public void Initialize( BattleGameManager battleGameManager )
    {
        this.skillSelectionPanel.Initialize( OnSkillSelectedFromSkillSelectionPanel, OnSkillDeselectedFromSkillSelectionPanel );
    }

    public void SetSelectedGameCharacter( GameCharacter gameCharacter )
    {
        this.selectedGameCharacter = gameCharacter;
    }

#region Skill Selection Panel

    public void ShowSkillSelectionPanel()
    {
        this.skillSelectionPanel.Show( this.selectedGameCharacter.GetSkills() );
    }

    public void OnSkillSelectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
        this.selectedGameCharacter.AddSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel( this.selectedGameCharacter );
    }

    public void OnSkillDeselectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
        this.selectedGameCharacter.RemoveSelectedSkill( skillSelectionBox.GetCharacterSkill() );
        UpdateSkillSlotListPanel( this.selectedGameCharacter );
    }

#endregion

#region Skill Slot List Panel

    public void UpdateSkillSlotListPanel( GameCharacter gameCharacter )
    {
        this.skillSlotListPanel.Show( gameCharacter );
    }

#endregion

    public SkillSelectionPanel GetSkillSelectionPanel()
    {
        return this.skillSelectionPanel;
    }
}
