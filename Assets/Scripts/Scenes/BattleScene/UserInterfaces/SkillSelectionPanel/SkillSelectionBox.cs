using System;
using UnityEngine;

public class SkillSelectionBox : MonoBehaviour
{
    private SkillSelectionListBox skillSelectionListBox = null;
    private CharacterSkill characterSkill = null;
    private bool isSelected = false;
    private Action onSelected = null;

    public void Initialize( SkillSelectionListBox skillSelectionListBox, CharacterSkill characterSkill, Action onSelected )
    {
        this.characterSkill = characterSkill;
        this.onSelected = onSelected;
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
