using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUiManager : MonoBehaviour
{
    [SerializeField] private SkillSelectionPanel skillSelectionPanel;

    private BattleGameManager battleGameManager = null;

    public void Initialize( BattleGameManager battleGameManager )
    {
        SkillDatabase skillDatabase = battleGameManager.GetSkillDatabase();
        List<CharacterSkill> characterSkillList = new List<CharacterSkill>();

        for (int i = 1; i <= 6; i++)
        {
            characterSkillList.Add( new CharacterSkill( skillDatabase.GetSkillDataById( i ) ) );
        }

        skillSelectionPanel.Initialize( characterSkillList.ToArray(), OnSkillSelectedFromSkillSelectionPanel, OnSkillDeselectedFromSkillSelectionPanel );
    }

    public void ShowSkillSelectionPanel( GameCharacter gameCharacter )
    {
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
