using System.Collections.Generic;
using UnityEngine;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;
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
        CharacterIdentityType _characterIdentityType = gameCharacter.GetCurrentCharacterIdentityType();
        CharacterSkill _currentSkill = gameCharacter.GetCurrentSkill();
        List<SkillType> _skillTypeList = new();

        switch ( battlePhaseType )
        {
            case BattlePhaseType.CombatCommandTime_Before:

                if (_characterIdentityType is CharacterIdentityType.None)
                {
                    _skillTypeList = new List<SkillType> { SkillType.Active, SkillType.Defend, SkillType.Evade };
                }

                break;

            case BattlePhaseType.Part_A:

                if (_characterIdentityType is CharacterIdentityType.Lead)
                {
                    _skillTypeList = new List<SkillType>();
                }

                break;

            case BattlePhaseType.RepulseCommandTime:

                if (_characterIdentityType is CharacterIdentityType.Improviser)
                {
                    _skillTypeList = new List<SkillType> { SkillType.Repulse, SkillType.Defend, SkillType.Evade };
                }

                break;

            case BattlePhaseType.CombatCommandTime_After:

                if (atlNumber <= 4)
                {
                    if (_characterIdentityType is CharacterIdentityType.Lead
                                               or CharacterIdentityType.LightAssaulter
                                               or CharacterIdentityType.Improviser
                                               or CharacterIdentityType.LightRecipient
                                               or CharacterIdentityType.HeavyAssaulter
                                               or CharacterIdentityType.HeavyRecipient
                                               or CharacterIdentityType.Deuce)
                    {
                        _skillTypeList = new List<SkillType> { SkillType.Active, SkillType.Defend, SkillType.Evade };
                    }
                    else if (_characterIdentityType is CharacterIdentityType.HeavyRecipient)
                    {
                        _skillTypeList = new List<SkillType> { SkillType.Defend, SkillType.Evade };
                    }
                }
                else
                {
                    if (_characterIdentityType is CharacterIdentityType.Lead
                                               or CharacterIdentityType.LightAssaulter
                                               or CharacterIdentityType.Improviser
                                               or CharacterIdentityType.LightRecipient
                                               or CharacterIdentityType.HeavyAssaulter)
                    {
                        _skillTypeList = new List<SkillType>();
                    }
                }

                if (_characterIdentityType is CharacterIdentityType.HeavyAssaulter)
                {
                    if (_currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill() != null)
                    {
                        _skillTypeList.Add( SkillType.Derive );
                    }
                }

                break;

            case BattlePhaseType.CounterAttackCommandTime:

                if (_characterIdentityType is CharacterIdentityType.SuccessfulDefender
                                           or CharacterIdentityType.SuccessfulEvader)
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
            CharacterSkill _attackerCurrentSkill = attacker.GetCurrentSkill();
            Skill.SkillType _skillType = _attackerCurrentSkill.GetSkillData().skillType;

            if (_skillType is Skill.SkillType.active or Skill.SkillType.repulse or Skill.SkillType.counter)
            {
                if (gameCharacter.GetCurrentObservedSkill() != _attackerCurrentSkill)
                {
                    _skillTypeList.Add( SkillType.Observe );
                }
            }
        }

        return _skillTypeList;
    }
}
