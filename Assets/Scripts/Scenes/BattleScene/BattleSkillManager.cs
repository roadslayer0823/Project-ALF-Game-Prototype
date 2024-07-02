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
        List<SkillType> _skillTypeList = new();

        BattleResultData.BattleResultData_GameCharacter _temporaryBattleResultData = gameCharacter.GetTemporaryBattleResultData();
        if (_temporaryBattleResultData != null)
        {
            if (_temporaryBattleResultData.IsInBreakStatus() || _temporaryBattleResultData.isDead)
            {
                return _skillTypeList;
            }
        }
        else
        {
            if (gameCharacter.IsInBreakStatus() || gameCharacter.GetIsDead())
            {
                return _skillTypeList;
            }
        }

        CharacterSkill _currentSkill = gameCharacter.GetCurrentSkill();

        switch ( battlePhaseType )
        {
            case BattlePhaseType.CombatCommandTime_Before:

                if (gameCharacter.IsCharacterIdentityTypeListEmpty())
                {
                    _skillTypeList = new List<SkillType> { SkillType.Active, SkillType.Defend, SkillType.Evade };
                }

                break;

            case BattlePhaseType.Part_A:

                if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.Lead ))
                {
                    _skillTypeList = new List<SkillType>();
                }

                break;

            case BattlePhaseType.RepulseCommandTime:

                if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
                {
                    _skillTypeList = new List<SkillType> { SkillType.Repulse, SkillType.Defend, SkillType.Evade };
                }

                break;

            case BattlePhaseType.CombatCommandTime_After:

                if (atlNumber <= 4)
                {
                    if (gameCharacter.HasCharacterIdentityTypes( new CharacterIdentityType[]
                                                                 {
                                                                     CharacterIdentityType.Lead,
                                                                     CharacterIdentityType.LightAssaulter,
                                                                     CharacterIdentityType.Improviser,
                                                                     CharacterIdentityType.LightRecipient,
                                                                     CharacterIdentityType.HeavyAssaulter,
                                                                     CharacterIdentityType.Deuce
                                                                 }
                                                                 ))
                    {
                        _skillTypeList = new List<SkillType> { SkillType.Active, SkillType.Defend, SkillType.Evade };
                    }
                    else if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.HeavyRecipient ))
                    {
                        _skillTypeList = new List<SkillType> { SkillType.Defend, SkillType.Evade };
                    }
                }
                else
                {
                    if (gameCharacter.HasCharacterIdentityTypes( new CharacterIdentityType[]
                                                                 {
                                                                     CharacterIdentityType.Lead,
                                                                     CharacterIdentityType.LightAssaulter,
                                                                     CharacterIdentityType.Improviser,
                                                                     CharacterIdentityType.LightRecipient,
                                                                     CharacterIdentityType.HeavyAssaulter
                                                                 }
                                                                 ))
                    {
                        _skillTypeList = new List<SkillType>();
                    }
                }

                if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.HeavyAssaulter ))
                {
                    if (_currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill() != null)
                    {
                        _skillTypeList.Add( SkillType.Derive );
                    }
                }

                break;

            case BattlePhaseType.CounterAttackCommandTime:

                if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.SuccessfulResister ))
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
