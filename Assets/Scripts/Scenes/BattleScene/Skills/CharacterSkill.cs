using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using SkillAnimation = DatabaseManager.SkillAnimation;
using static DatabaseManager;

public class CharacterSkill
{
    private Skill skillData = null;
    private List<Subskill> subskillList = null;

    public CharacterSkill( Skill skillData)
    {
        this.skillData = skillData;
        SetupSubskillList(skillData.GetId());
    }

    private void SetupSubskillList(string skillId)
    {
        List<Subskill> _subskillList = DatabaseManager.Instance.GetSubskillList();
        this.subskillList = new List<Subskill>();

        foreach (Subskill subskill in _subskillList)
        {
            if (subskill.GetSkillId() == skillId)
            {
                this.subskillList.Add(subskill);
            }
        }
    }

    public Skill GetSkillData()
    {
        return this.skillData;
    }

    public List<Subskill> GetSubskillList()
    {
        return this.subskillList;
    }

    public Subskill GetSubskillByLevel(int level)
    {
        int _maxLevel = this.subskillList.Count;

        if (level > _maxLevel)
        {
            return this.subskillList[_maxLevel - 1];
        }
        else
        {
            foreach (Subskill subskill in this.subskillList)
            {
                if (subskill.GetLevel() == level)
                {
                    return subskill;
                }
            }
        }

        return null;
    }

    public SkillAnimation GetSkillAnimation(string subskillId)
    {
        List<SkillAnimation> _skillAniationList = DatabaseManager.Instance.GetSkillAnimationList();

        foreach (SkillAnimation skillAnimation in _skillAniationList)
        {
            if (skillAnimation.GetSubskillId() == subskillId)
            {
                return skillAnimation;
            }
        }

        return null;
    }
}
