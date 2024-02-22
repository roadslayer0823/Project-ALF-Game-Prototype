using System.Collections;
using System.Collections.Generic;
using Subskill = DatabaseManager.Subskill;

public class CharacterSubskill
{
    private GameCharacter owner = null;
    private Subskill subskill = null;

    private List<CharacterSkill> repulseSkillList = new List<CharacterSkill>();
    private List<CharacterSkill> derivedSkillList = new List<CharacterSkill>();
    private List<CharacterSkill> counterSkillList = new List<CharacterSkill>();

    private CharacterSkill selectedRepulseSkill = null;
    private CharacterSkill selectedDerivedSkill = null;
    private CharacterSkill selectedCounterSkill = null;

    public CharacterSubskill(Subskill subskillData, GameCharacter owner)
    {
        this.subskill = subskillData;
        this.owner = owner;

        if (subskillData.IsAttackingSkill)
        {
            SetRepulseSkillList();
            SetDerivedSkillList();
        }
        else if (subskillData.IsDefendingSkill || subskillData.IsEvadingSkill)
        {
            SetCounterSkillList();
        }
    }

    public Subskill GetSubskillData()
    {
        return this.subskill;
    }

    private void SetRepulseSkillList()
    {
        if (this.subskill.RepulseSkillIds == null)
        {
            return;
        }

        string[] _repulseSkillIds = this.subskill.RepulseSkillIds;

        for (int i = 0; i < _repulseSkillIds.Length; i++)
        {
            string _repulseSkillId = _repulseSkillIds[i];

            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int j = 0; j < _characterSkill.Length; j++)
            {
                CharacterSkill _repulseSkill = _characterSkill[j];

                if (_repulseSkillId == _repulseSkill.GetSkillData().Id)
                {
                    this.repulseSkillList.Add(_repulseSkill);
                    break;
                }
            }
        }

        if (this.repulseSkillList.Count > 0)
        {
            SetSelectedRepulseSkill(this.repulseSkillList[0]);
        }
    }

    private void SetDerivedSkillList()
    {
        if (this.subskill.DerivedSkillIds == null)
        {
            return;
        }

        string[] _derivedSkillIds = this.subskill.DerivedSkillIds;

        for (int i = 0; i < _derivedSkillIds.Length; i++)
        {
            string _derivedSkillId = _derivedSkillIds[i];

            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int j = 0; j < _characterSkill.Length; j++)
            {
                CharacterSkill _derivedSkill = _characterSkill[j];

                if (_derivedSkillId == _derivedSkill.GetSkillData().Id)
                {
                    this.derivedSkillList.Add(_derivedSkill);
                    break;
                }
            }
        }

        if (this.derivedSkillList.Count > 0)
        {
            SetSelectedDerivedSkill(this.derivedSkillList[0]);
        }
    }

    private void SetCounterSkillList()
    {
        if (this.subskill.CounterSkillIds == null)
        {
            return;
        }

        string[] _counterSkillIds = this.subskill.CounterSkillIds;

        for (int i = 0; i < _counterSkillIds.Length; i++)
        {
            string _counterSkillId = _counterSkillIds[i];

            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int j = 0; j < _characterSkill.Length; j++)
            {
                CharacterSkill _counterSkill = _characterSkill[j];

                if (_counterSkillId == _counterSkill.GetSkillData().Id)
                {
                    this.counterSkillList.Add(_counterSkill);
                    break;
                }
            }
        }

        if (this.counterSkillList.Count > 0)
        {
            SetSelectedCounterSkill(this.counterSkillList[0]);
        }
    }

    public void SetSelectedRepulseSkill(CharacterSkill selectedRepulseSkill)
    {
        this.selectedRepulseSkill = selectedRepulseSkill;
    }

    public void SetSelectedDerivedSkill(CharacterSkill selectedDerivedSkill)
    {
        this.selectedDerivedSkill = selectedDerivedSkill;
    }

    public void SetSelectedCounterSkill(CharacterSkill selectedCounterSkill)
    {
        this.selectedCounterSkill = selectedCounterSkill;
    }

    public List<CharacterSkill> GetRepulseSkillList()
    {
        return this.repulseSkillList;
    }

    public List<CharacterSkill> GetDerivedSkillList()
    {
        return this.derivedSkillList;
    }

    public List<CharacterSkill> GetCounterSkillList()
    {
        return this.counterSkillList;
    }

    public CharacterSkill GetSelectedRepulseSkill()
    {
        return this.selectedRepulseSkill;
    }

    public CharacterSkill GetSelectedDerivedSkill()
    {
        return this.selectedDerivedSkill;
    }

    public CharacterSkill GetSelectedCounterSkill()
    {
        return this.selectedCounterSkill;
    }
}
