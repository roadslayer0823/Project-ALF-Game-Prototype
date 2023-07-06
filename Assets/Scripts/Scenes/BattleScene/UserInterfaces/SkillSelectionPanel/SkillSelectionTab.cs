using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionTab : MonoBehaviour
{
    [SerializeField] private SkillInfoBox skillInfoPanel = null;
    [SerializeField] private SkillSelectionListBox skillSelectionListBox = null;

    private SkillSelectionPanel skillSelectionPanel = null;

    public void Initialize( SkillSelectionPanel skillSelectionPanel )
    {
        this.skillSelectionPanel = skillSelectionPanel;
        skillSelectionListBox.Initialize( this );
    }

    public void Show( CharacterSkill[] characterSkills )
    {
        skillSelectionListBox.Show( characterSkills );
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
