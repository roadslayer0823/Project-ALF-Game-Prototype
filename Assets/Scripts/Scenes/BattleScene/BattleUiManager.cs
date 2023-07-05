using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUiManager : MonoBehaviour
{
    [SerializeField] private SkillSelectionPanel skillSelectionPanel;

    public void Initialize()
    {
    }

    public void ShowSkillSelectionPanel( GameCharacter gameCharacter )
    {
        skillSelectionPanel.Initialize( gameCharacter.GetSkills(), OnSkillSelectedFromSkillSelectionPanel, OnSkillDeselectedFromSkillSelectionPanel );
    }

    public void OnSkillSelectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
    }

    public void OnSkillDeselectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
    }
}
