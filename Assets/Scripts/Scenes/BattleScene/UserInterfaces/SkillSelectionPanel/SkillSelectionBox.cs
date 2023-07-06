using System;
using UnityEngine;

public class SkillSelectionBox : MonoBehaviour
{
    private SkillSelectionListBox skillSelectionListBox = null;
    private CharacterSkill characterSkill = null;
    private bool isSelected = false;

    public void Initialize( SkillSelectionListBox skillSelectionListBox, CharacterSkill characterSkill )
    {
        this.characterSkill = characterSkill;
        this.isSelected = false;
    }

    public void ClickToToggle()
    {
        if (this.isSelected == true)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    public void Select()
    {
        this.isSelected = true;
        this.skillSelectionListBox.OnSkillSelected( this );
    }

    public void Deselect()
    {
        this.isSelected = false;
        this.skillSelectionListBox.OnSkillDeselected( this );
    }

    public CharacterSkill GetCharacterSkill()
    {
        return this.characterSkill;
    }
}
