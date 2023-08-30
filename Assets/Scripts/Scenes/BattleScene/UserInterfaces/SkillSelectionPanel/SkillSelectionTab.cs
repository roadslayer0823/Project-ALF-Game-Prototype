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

    public void ShowCharacterSkillList(CharacterSkill[] characterSkills)
    {
        this.skillSelectionListBox.ShowCharacterSkillList(characterSkills);
    }

    public void ShowRepulseSkillList()
    {
        this.skillSelectionListBox.ShowRepulseSkillList();
    }

    public void ShowDerivedSkillList()
    {
        this.skillSelectionListBox.ShowDerivedSkillList();
    }

    public void ShowCounterSkillList()
    {
        this.skillSelectionListBox.ShowCounterSkillList();
    }

    public void OnSkillSelected( SkillSelectionBox skillSelectionBox )
    {
        this.skillSelectionPanel.OnSkillSelected( skillSelectionBox );

        ShowSelectedSkillInfo(skillSelectionBox.GetCharacterSkill());
    }

    public void OnSkillDeselected( SkillSelectionBox skillSelectionBox )
    {
        this.skillSelectionPanel.OnSkillDeselected( skillSelectionBox );

        ShowSelectedSkillInfo(skillSelectionBox.GetCharacterSkill());
    }

    public void ShowSelectedSkillInfo(CharacterSkill characterSkill)
    {
        if (characterSkill == null)
        {
            this.skillInfoPanel.Show(null);
            return;
        }

        CharacterSubskill _characterSubskill = characterSkill.GetCharacterSubskillData();
        this.skillInfoPanel.Show(_characterSubskill);
    }

    public void HideSkillInfoPanel()
    {
        this.skillInfoPanel.Hide();
    }

    public SkillSelectionPanel.SkillInfoTab GetSkillInfoTab()
    {
        return this.skillSelectionPanel.GetSkillInfoTab();
    }

    public CharacterSkill GetLastSelectedCharacterSkill()
    {
        return this.skillSelectionListBox.GetLastSelectedCharacterSkill();
    }

    public CharacterSkill GetLastSelectedRepulseSkill()
    {
        return this.skillSelectionListBox.GetLastSelectedRepulseSkill();
    }

    public CharacterSkill GetLastSelectedDerivedSkill()
    {
        return this.skillSelectionListBox.GetLastSelectedDerivedSkill();
    }

    public CharacterSkill GetLastSelectedCounterSkill()
    {
        return this.skillSelectionListBox.GetLastSelectedCounterSkill();
    }

    public void SetSkillListTitle(string skillListTitle)
    {
        this.skillSelectionListBox.SetSkillListTitle(skillListTitle);
    }
}
