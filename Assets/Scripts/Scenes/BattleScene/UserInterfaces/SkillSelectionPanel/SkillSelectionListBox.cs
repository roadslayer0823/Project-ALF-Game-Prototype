using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionListBox : MonoBehaviour
{
    private SkillSelectionTab skillSelectionTab = null;
    private List<SkillSelectionBox> skillSelectionBoxList = null;

    public void Initialize( SkillSelectionTab skillSelectionTab, CharacterSkill[] characterSkills )
    {
        for (int i = 0; i < characterSkills.Length; i++)
        {
        }
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionTab.OnSkillSelected( skillSelectionBox );
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionTab.OnSkillDeselected( skillSelectionBox );
    }
}
