using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;

public class CharacterSubskill
{
    Subskill subskill = null;
    Subskill repulseSkill = null;
    Subskill derivedSkill = null;
    Subskill counterSkill = null;

    public CharacterSubskill(Subskill subskillData)
    {
        this.subskill = subskillData;

        SetRepulseSkill();
        SetDerivedSkill();
        SetCounterSkill();
    }

    private void SetRepulseSkill()
    {
        string _repulseSkillId = this.subskill.RepulseSkillId;

        if (_repulseSkillId == "-")
        {
            this.repulseSkill = null;
            return;
        }
        else
        {
            List<Subskill> _subskillList = DatabaseManager.Instance.GetSubskillList();
            for (int i = 0; i < _subskillList.Count; i++)
            {
                Subskill _repulseSkill = _subskillList[i];

                if (_repulseSkillId == _repulseSkill.SkillId)
                {
                    this.repulseSkill = _repulseSkill;
                    break;
                }
            }
        }
    }

    private void SetDerivedSkill()
    {
        string _derivedSkillId = this.subskill.DerivedSkillId;

        if (_derivedSkillId == "-")
        {
            this.derivedSkill = null;
            return;
        }
        else
        {
            List<Subskill> _subskillList = DatabaseManager.Instance.GetSubskillList();
            for (int i = 0; i < _subskillList.Count; i++)
            {
                Subskill _derivedSkill = _subskillList[i];

                if (_derivedSkillId == _derivedSkill.SkillId)
                {
                    this.derivedSkill = _derivedSkill;
                    break;
                }
            }
        }
    }

    private void SetCounterSkill()
    {
        string _counterSkillId = this.subskill.CounterSkillId;

        if (_counterSkillId == "-")
        {
            this.counterSkill = null;
            return;
        }
        else
        {
            List<Subskill> _subskillList = DatabaseManager.Instance.GetSubskillList();
            for (int i = 0; i < _subskillList.Count; i++)
            {
                Subskill _counterSkill = _subskillList[i];

                if (_counterSkillId == _counterSkill.SkillId)
                {
                    this.counterSkill = _counterSkill;
                    break;
                }
            }
        }
    }

    public Subskill GetSubskillData()
    {
        return this.subskill;
    }

    public Subskill GetRepulseSkill()
    {
        return this.repulseSkill;
    }

    public Subskill GetDerivedSkill()
    {
        return this.derivedSkill;
    }

    public Subskill GetCounterSkill()
    {
        return this.counterSkill;
    }
}
