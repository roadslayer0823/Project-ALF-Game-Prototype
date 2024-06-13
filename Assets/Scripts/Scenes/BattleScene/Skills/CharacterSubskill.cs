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
        if (this.subskill.RepulseSubskillIds == null)
        {
            return;
        }

        string[] _repulseSubskillIds = this.subskill.RepulseSubskillIds;

        for (int i = 0; i < _repulseSubskillIds.Length; i++)
        {
            string[] _repulseSubskillIdStrings = _repulseSubskillIds[ i ].Split( '_' );
            string _repulseSkillId = _repulseSubskillIdStrings[ 0 ];
            int _repulseSkillLevel = int.Parse( _repulseSubskillIdStrings[ 1 ] );

            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int j = 0; j < _characterSkill.Length; j++)
            {
                CharacterSkill _repulseSkill = _characterSkill[ j ].GetClone();

                if (_repulseSkillId == _repulseSkill.GetSkillData().Id)
                {
                    _repulseSkill.SetupCharacterSubskillList();
                    _repulseSkill.SetSelectedSkillLevel( _repulseSkillLevel );
                    this.repulseSkillList.Add(_repulseSkill);

                    this.owner.AddToAllSkills( _repulseSkill );

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
        if (this.subskill.DerivedSubskillIds == null)
        {
            return;
        }

        string[] _derivedSubskillIds = this.subskill.DerivedSubskillIds;

        for (int i = 0; i < _derivedSubskillIds.Length; i++)
        {
            string[] _derivedSubskillIdStrings = _derivedSubskillIds[ i ].Split( '_' );
            string _derivedSkillId = _derivedSubskillIdStrings[ 0 ];
            int _derivedSkillLevel = int.Parse( _derivedSubskillIdStrings[ 1 ] );

            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int j = 0; j < _characterSkill.Length; j++)
            {
                CharacterSkill _derivedSkill = _characterSkill[ j ].GetClone();

                if (_derivedSkillId == _derivedSkill.GetSkillData().Id)
                {
                    _derivedSkill.SetupCharacterSubskillList();
                    _derivedSkill.SetSelectedSkillLevel( _derivedSkillLevel );
                    this.derivedSkillList.Add(_derivedSkill);

                    this.owner.AddToAllSkills( _derivedSkill );

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
        if (this.subskill.CounterSubskillIds == null)
        {
            return;
        }

        string[] _counterSubskillIds = this.subskill.CounterSubskillIds;

        for (int i = 0; i < _counterSubskillIds.Length; i++)
        {
            string[] _counterSubskillIdStrings = _counterSubskillIds[ i ].Split( '_' );
            string _counterSkillId = _counterSubskillIdStrings[ 0 ];
            int _counterSkillLevel = int.Parse( _counterSubskillIdStrings[ 1 ] );

            CharacterSkill[] _characterSkill = owner.GetSkills();
            for (int j = 0; j < _characterSkill.Length; j++)
            {
                CharacterSkill _counterSkill = _characterSkill[ j ].GetClone();

                if (_counterSkillId == _counterSkill.GetSkillData().Id)
                {
                    _counterSkill.SetupCharacterSubskillList();
                    _counterSkill.SetSelectedSkillLevel( _counterSkillLevel );
                    this.counterSkillList.Add(_counterSkill);

                    this.owner.AddToAllSkills( _counterSkill );

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
