using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class CharacterSkill
{
    private GameCharacter owner = null;
    private Skill skillData = null;
    private List<CharacterSubskill> characterSubskillList = null;
    private int selectedSkillLevel = 1;

    public CharacterSkill( Skill skillData, GameCharacter owner)
    {
        this.skillData = skillData;
        this.owner = owner;
    }

    public void SetupCharacterSubskillList()
    {
        List<Subskill> _subskillList = DatabaseManager.Instance.GetSubskillList();
        this.characterSubskillList = new List<CharacterSubskill>();

        foreach (Subskill subskill in _subskillList)
        {
            if (subskill.SkillId == this.skillData.Id)
            {
                this.characterSubskillList.Add(new CharacterSubskill(subskill, owner));
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

    public int GetMaximumSkillLevel()
    {
        return this.characterSubskillList.Count;
    }

    public bool IsSkillAvailable( bool canDefend, bool canEvade )
    {
        Subskill _subskill = GetCharacterSubskillData().GetSubskillData();

        return ( ( canDefend && _subskill.IsDefendingSkill )
               || ( canEvade && _subskill.IsEvadingSkill ) );
    }
}
