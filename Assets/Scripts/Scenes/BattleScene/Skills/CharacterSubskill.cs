using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;

public class CharacterSubskill
{
    GameCharacter owner = null;
    Subskill subskill = null;
    CharacterSkill repulseSkill = null;
    CharacterSkill derivedSkill = null;
    CharacterSkill counterSkill = null;

    public CharacterSubskill(Subskill subskillData, GameCharacter owner)
    {
        this.subskill = subskillData;
        this.owner = owner;

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
            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int i = 0; i < _characterSkill.Length; i++)
            {
                CharacterSkill _repulseSkill = _characterSkill[i];

                if (_repulseSkillId == _repulseSkill.GetSkillData().Id)
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
            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int i = 0; i < _characterSkill.Length; i++)
            {
                CharacterSkill _derivedSkill = _characterSkill[i];

                if (_derivedSkillId == _derivedSkill.GetSkillData().Id)
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
            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int i = 0; i < _characterSkill.Length; i++)
            {
                CharacterSkill _counterSkill = _characterSkill[i];

                if (_counterSkillId == _counterSkill.GetSkillData().Id)
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

    public CharacterSkill GetRepulseSkill()
    {
        return this.repulseSkill;
    }

    public CharacterSkill GetDerivedSkill()
    {
        return this.derivedSkill;
    }

    public CharacterSkill GetCounterSkill()
    {
        return this.counterSkill;
    }
}
