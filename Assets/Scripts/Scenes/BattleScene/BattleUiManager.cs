using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUiManager : MonoBehaviour
{
    [SerializeField] private SkillSelectionPanel skillSelectionPanel = null;

    private BattleGameManager battleGameManager = null;

    public void Initialize( BattleGameManager battleGameManager )
    {
        skillSelectionPanel.Initialize( OnSkillSelectedFromSkillSelectionPanel, OnSkillDeselectedFromSkillSelectionPanel );
    }

    public void ShowSkillSelectionPanel( GameCharacter gameCharacter )
    {
        skillSelectionPanel.Show( gameCharacter.GetSkills() );
    }

    public void OnSkillSelectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
    }

    public void OnSkillDeselectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
    }

    public SkillSelectionPanel GetSkillSelectionPanel()
    {
        return skillSelectionPanel;
    }
}
