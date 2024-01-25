using System.Collections.Generic;
using UnityEngine;
using Skill = DatabaseManager.Skill;

public class BattleSkillManager : MonoBehaviour
{
    public enum BattlePhaseType
    {
        CombatCommandTime_Before,
        Part_A,
        RepulseCommandTime,
        CombatCommandTime_After,
        CounterAttackCommandTime
    }

    public enum SkillType
    {
        None,
        Active,
        Repulse,
        Defend,
        Evade,
        Derive,
        Counter,
        Observe
    }

    public static List<SkillType> GetSkillTypeList( GameCharacter gameCharacter, BattlePhaseType battlePhaseType, int atlNumber, GameCharacter attacker = null )
    {
        GameCharacter.CharacterIdentityType _characterIdentityType = gameCharacter.GetCurrentCharacterIdentityType();
        CharacterSkill _currentSkill = gameCharacter.GetCurrentSkill();
        List<SkillType> _skillTypeList = new List<SkillType>();

        switch ( battlePhaseType )
        {
            case BattlePhaseType.CombatCommandTime_Before:

                if (_characterIdentityType == GameCharacter.CharacterIdentityType.None)
                {
                    _skillTypeList = new List<SkillType> { SkillType.Active, SkillType.Defend, SkillType.Evade };
                }

                break;

            case BattlePhaseType.Part_A:

                if (_characterIdentityType == GameCharacter.CharacterIdentityType.Lead)
                {
                    _skillTypeList = new List<SkillType>();
                }

                break;

            case BattlePhaseType.RepulseCommandTime:

                if (_characterIdentityType == GameCharacter.CharacterIdentityType.Improviser)
                {
                    _skillTypeList = new List<SkillType> { SkillType.Repulse, SkillType.Defend, SkillType.Evade };
                }

                break;

            case BattlePhaseType.CombatCommandTime_After:

                if (atlNumber <= 4)
                {
                    if (_characterIdentityType == GameCharacter.CharacterIdentityType.Lead
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.LightAssaulter
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.Improviser
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.LightRecipient
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.HeavyAssaulter
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.Deuce)
                    {
                        _skillTypeList = new List<SkillType> { SkillType.Active, SkillType.Defend, SkillType.Evade };
                    }
                    else if (_characterIdentityType == GameCharacter.CharacterIdentityType.HeavyRecipient)
                    {
                        _skillTypeList = new List<SkillType> { SkillType.Defend, SkillType.Evade };
                    }
                }
                else
                {
                    if (_characterIdentityType == GameCharacter.CharacterIdentityType.Lead
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.LightAssaulter
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.Improviser
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.LightRecipient
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.HeavyAssaulter
                        || _characterIdentityType == GameCharacter.CharacterIdentityType.HeavyRecipient)
                    {
                        _skillTypeList = new List<SkillType>();
                    }
                }

                if (_characterIdentityType == GameCharacter.CharacterIdentityType.HeavyAssaulter)
                {
                    if (_currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill() != null)
                    {
                        _skillTypeList.Add( SkillType.Derive );
                    }
                }

                break;

            case BattlePhaseType.CounterAttackCommandTime:

                if (_characterIdentityType == GameCharacter.CharacterIdentityType.SuccessfulDefender
                    || _characterIdentityType == GameCharacter.CharacterIdentityType.SuccessfulEvader)
                {
                    if (atlNumber <= 4)
                    {
                        _skillTypeList = new List<SkillType> { SkillType.Active, SkillType.Defend, SkillType.Evade };

                        if (_currentSkill != null)
                        {
                            if (_currentSkill.GetCharacterSubskillData().GetSelectedCounterSkill() != null)
                            {
                                _skillTypeList.Add( SkillType.Counter );
                            }
                        }
                    }
                    else
                    {
                        _skillTypeList = new List<SkillType>();
                    }
                }

                break;
        }

        if (attacker != null)
        {
            Skill.SkillType _skillType = attacker.GetCurrentSkill().GetSkillData().skillType;
            if (_skillType == Skill.SkillType.active
                || _skillType == Skill.SkillType.repulse
                || _skillType == Skill.SkillType.counter)
            {
                _skillTypeList.Add( SkillType.Observe );
            }
        }

        return _skillTypeList;
    }
}
