using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUiManager : MonoBehaviour
{
    [SerializeField] private SkillSelectionPanel skillSelectionPanel = null;

    private BattleGameManager battleGameManager = null;
    private List<SkillSelectionBox> selectedActiveSkillList = new List<SkillSelectionBox>();
    private List<SkillSelectionBox> selectedPassiveSkillList = new List<SkillSelectionBox>();

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
        if (skillSelectionBox.GetCharacterSkill().GetSkillData().GetSkillType() == SkillDatabase.SkillData.SkillType.Active)
        {
            if (this.selectedActiveSkillList.Count < 3)
            {
                this.selectedActiveSkillList.Add(skillSelectionBox);

                UpdateSelectedSkillSequence();
            }
        }
        else if (skillSelectionBox.GetCharacterSkill().GetSkillData().GetSkillType() == SkillDatabase.SkillData.SkillType.Backend)
        {
            if (this.selectedPassiveSkillList.Count < 3)
            {
                this.selectedPassiveSkillList.Add(skillSelectionBox);

                skillSelectionBox.SetSkillSelectionText("ON");
            }
        }
        
    }

    public void OnSkillDeselectedFromSkillSelectionPanel( SkillSelectionBox skillSelectionBox )
    {
        if (this.selectedActiveSkillList.Contains(skillSelectionBox))
        {
            this.selectedActiveSkillList.Remove(skillSelectionBox);

            skillSelectionBox.SetSkillSelectionSequenceNumber(0);

            UpdateSelectedSkillSequence();
        }
        else if (this.selectedPassiveSkillList.Contains(skillSelectionBox))
        {
            this.selectedPassiveSkillList.Remove(skillSelectionBox);

            skillSelectionBox.SetSkillSelectionText("");
        }
    }

    public SkillSelectionPanel GetSkillSelectionPanel()
    {
        return skillSelectionPanel;
    }

    private void UpdateSelectedSkillSequence()
    {
        int skillSelectionCounter = 0;

        foreach (SkillSelectionBox skill in selectedActiveSkillList)
        {
            skillSelectionCounter++;
            skill.SetSkillSelectionSequenceNumber(skillSelectionCounter);
        }
    }
}
