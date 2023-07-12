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
        this.skillSelectionListBox.Initialize( this );
    }

    private void Start()
    {
        this.skillInfoPanel.Hide();
    }

    public void Show( CharacterSkill[] characterSkills )
    {
        this.skillSelectionListBox.Show( characterSkills );
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        this.skillSelectionPanel.OnSkillSelected( skillSelectionBox );

        this.skillInfoPanel.Show(skillSelectionBox.GetCharacterSkill());
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        skillSelectionPanel.OnSkillDeselected( skillSelectionBox );

        this.skillInfoPanel.Show(skillSelectionBox.GetCharacterSkill());
    }

    public void HideSkillInfoPanel()
    {
        this.skillInfoPanel.Hide();
    }
}
