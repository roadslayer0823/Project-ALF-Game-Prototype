using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionTab : MonoBehaviour
{
    [SerializeField] private SkillInfoBox skillInfoPanel = null;
    [SerializeField] private SkillSelectionListBox skillSelectionListBox = null;

    private SkillSelectionPanel skillSelectionPanel = null;

    public void Initialize( SkillSelectionPanel skillSelectionPanel, CharacterSkill[] characterSkills )
    {
        skillSelectionListBox.Initialize( this, characterSkills );
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionPanel.OnSkillSelected( skillSelectionBox );
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionPanel.OnSkillDeselected( skillSelectionBox );
    }
}
