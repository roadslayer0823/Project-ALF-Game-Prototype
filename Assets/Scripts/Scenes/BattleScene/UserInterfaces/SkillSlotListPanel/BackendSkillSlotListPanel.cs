using System.Collections.Generic;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;
using Skill = DatabaseManager.Skill;

public class BackendSkillSlotListPanel : MonoBehaviour
{
    [SerializeField] private SkillSlotV2[] backendSkillSlot = new SkillSlotV2[0];
    [SerializeField] private SkillSlotV2 qteSkillSlot;
    [SerializeField] private GameObject qteButton;

    private CharacterSkill qteSkill;
    private GameCharacter selectedCharacter;
    private List<CharacterSkill> selectedSkills = new List<CharacterSkill>();

    public void Initialize()
    {
        for (int i = 0; i < this.backendSkillSlot.Length; i++)
        {
            this.backendSkillSlot[i].InitializeBackendSkillSlot(this);
        }
    }

    public void HideQTEButton()
    {
        this.qteButton.SetActive(false);
    }

    public void ShowQTEButton()
    {
        this.qteButton.SetActive(true);
    }

    public void UpdateBackendSkillSlots(GameCharacter gameCharacter)
    {
        if (gameCharacter != null)
        {
            this.selectedCharacter = gameCharacter;

            this.selectedSkills = new List<CharacterSkill>(gameCharacter.GetSelectedBackendSkillList());
            if (this.selectedSkills.Count > backendSkillSlot.Length)
            {
                return;
            }

            InsertIntoBackendSkillSlot(this.selectedSkills);
        }
    }

    private void InsertIntoBackendSkillSlot(List<CharacterSkill> selectedSkills)
    {
        for (int i = 0; i < selectedSkills.Count; i++)
        {
            Subskill _subskillData = selectedSkills[i].GetCharacterSubskillData().GetSubskillData();
            switch (backendSkillSlot[i].backendSkillType)
            {
                case SkillSlotV2.BackendSkillType.Defense:
                    if (_subskillData.IsDefendingSkill)
                    {
                        backendSkillSlot[i].SetSelectedSkill(selectedSkills[i]);
                    }
                    break;

                case SkillSlotV2.BackendSkillType.Evasion:
                    if (_subskillData.IsEvadingSkill)
                    {
                        backendSkillSlot[i].SetSelectedSkill(selectedSkills[i]);
                    }
                    break;

                case SkillSlotV2.BackendSkillType.Generic:
                    if (_subskillData.IsObservingSkill || _subskillData.IsDefendingSkill || _subskillData.IsEvadingSkill)
                    {
                        backendSkillSlot[i].SetSelectedSkill(selectedSkills[i]);
                    }
                    break;
            }
        }
    }

    public void ShowQteSkillSlot(GameCharacter gameCharacter)
    {
        this.selectedCharacter = gameCharacter;

        CharacterSkill _currentSkill = this.selectedCharacter.GetCurrentSkill();
        for (int i = 0; i < this.selectedSkills.Count; i++)
        {
            CharacterSkill _selectedSkill = this.selectedSkills[i];
            if (_selectedSkill == _currentSkill)
            {
                this.selectedSkills[i] = _currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill();
            }
            SetQTEActionButton(this.selectedSkills[i]);
        }
    }

    public void SetQTEActionButton(CharacterSkill qteSkill)
    {
        this.qteSkill = qteSkill;

        if (qteSkill != null)
        {
            qteSkillSlot.SetSelectedSkill(qteSkill);
        }
    }
}
