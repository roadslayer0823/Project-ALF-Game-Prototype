using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class CharacterSkill
{
    private Skill skillData = null;
    private List<CharacterSubskill> characterSubskillList = null;
    private int selectedSkillLevel = 1;

    public CharacterSkill( Skill skillData)
    {
        this.skillData = skillData;
        SetupCharacterSubskillList(skillData.Id);
    }

    private void SetupCharacterSubskillList(string skillId)
    {
        List<Subskill> _subskillList = DatabaseManager.Instance.GetSubskillList();
        this.characterSubskillList = new List<CharacterSubskill>();

        foreach (Subskill subskill in _subskillList)
        {
            if (subskill.SkillId == skillId)
            {
                this.characterSubskillList.Add(new CharacterSubskill(subskill));
            }
        }
    }

    public Skill GetSkillData()
    {
        return this.skillData;
    }

    public List<CharacterSubskill> GetCharacterSubskillList()
    {
        return this.characterSubskillList;
    }

    public CharacterSubskill GetCharacterSubskillData()
    {
        int _maxLevel = this.characterSubskillList.Count;

        if (this.selectedSkillLevel > _maxLevel)
        {
            return this.characterSubskillList[_maxLevel - 1];
        }
        else
        {
            foreach (CharacterSubskill characterSubskill in this.characterSubskillList)
            {
                if (characterSubskill.GetSubskillData().Level == this.selectedSkillLevel)
                {
                    return characterSubskill;
                }
            }
        }

        return null;
    }

    public void SetSelectedSkillLevel(int selectedSkillLevel)
    {
        this.selectedSkillLevel = selectedSkillLevel;
    }
}
