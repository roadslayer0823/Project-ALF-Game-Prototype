using System.Collections.Generic;
using UnityEngine;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;
using Skill = DatabaseManager.Skill;
using SkillType = DatabaseManager.Skill.SkillType;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using BattleResultData_GameCharacter = BattleResultData.BattleResultData_GameCharacter;

public class BattleLogicManagerV2
{
    public enum ActionType
    {
        None,
        Repulse,
        Defend,
        Evade
    }

    private static bool IsAttackingSkill( CharacterSkill skill )
    {
        return ( skill?.GetSkillData().skillType is SkillType.active or SkillType.derived or SkillType.counter );
    }

    public static bool IsInBreakStatus( GameCharacter gameCharacter )
    {
        BattleResultData_GameCharacter _temporaryBattleResultData = gameCharacter.GetTemporaryBattleResultData();

        if (_temporaryBattleResultData != null)
        {
            if (_temporaryBattleResultData.IsInBreakStatus())
            {
                return true;
            }
        }
        else if (gameCharacter.GetStateBreakStatusRemainingATLs() > 0 || gameCharacter.GetStressBreakStatusRemainingATLs() > 0)
        {
            return true;
        }

        return false;
    }

    public static bool IsAbleToUseAnySkill( GameCharacter gameCharacter )
    {
        if (IsInBreakStatus( gameCharacter ))
        {
            return false;
        }

        return true;
    }

    public static bool IsAbleToUseAttackingAndDefendingSkills( GameCharacter gameCharacter )
    {
        if (!IsAbleToUseAnySkill( gameCharacter ))
        {
            return false;
        }

        float _minimumCurrentStatePoint = GameConfiguration.Instance.GetBattleConfiguration().GetMinimumCurrentStatePoint();
        BattleResultData_GameCharacter _temporaryBattleResultData = gameCharacter.GetTemporaryBattleResultData();

        if (_temporaryBattleResultData != null)
        {
            if (_temporaryBattleResultData.currentStatePoint <= _minimumCurrentStatePoint)
            {
                return false;
            }
        }
        else if (gameCharacter.GetCurrentStatePoint() <= _minimumCurrentStatePoint)
        {
            return false;
        }

        return true;
    }

    public static bool IsAttackerCurrentSkillRecordedInObservingSkill( CharacterSkill observingSkill, GameCharacter attacker )
    {
        ObservedSkillRecord _observedSkillRecord = observingSkill.GetObservedSkillRecord();
        if (_observedSkillRecord != null
            && _observedSkillRecord.GetSubskillData().FeatureId == attacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().FeatureId)
        {
            return true;
        }

        return false;
    }

    public static bool ShouldPreparationSectionBeSkipped( GameCharacter gameCharacter )
    {
        if (IsInBreakStatus( gameCharacter ))
        {
            return true;
        }

        return false;
    }

    public static bool ShouldCombatCommandTimeBeSkipped( GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        CharacterSkill _gameCharacterOne_Skill = gameCharacterOne.GetAssignedSkill();
        CharacterSkill _gameCharacterTwo_Skill = gameCharacterTwo.GetAssignedSkill();

        if (_gameCharacterOne_Skill?.GetSkillData().skillType == SkillType.derived)
        {
            _gameCharacterTwo_Skill = gameCharacterTwo.ResetAssignedSkill();
        }
        else if (_gameCharacterTwo_Skill?.GetSkillData().skillType == SkillType.derived)
        {
            _gameCharacterOne_Skill = gameCharacterOne.ResetAssignedSkill();
        }

        return ( IsAttackingSkill( _gameCharacterOne_Skill ) || IsAttackingSkill( _gameCharacterTwo_Skill ) );
    }

    public static ( GameCharacter lead, GameCharacter improviser ) DetermineLeadAndImproviser( GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        CharacterSkill _gameCharacterOne_Skill = gameCharacterOne.GetAssignedSkill();
        CharacterSkill _gameCharacterTwo_Skill = gameCharacterTwo.GetAssignedSkill();

        bool _isGameCharacterOneUsingAttackingSkill = IsAttackingSkill( _gameCharacterOne_Skill );
        bool _isGameCharacterTwoUsingAttackingSkill = IsAttackingSkill( _gameCharacterTwo_Skill );

        Subskill _gameCharacterOne_Skill_SubskillData = null;
        Subskill _gameCharacterTwo_Skill_SubskillData = null;

        if (_isGameCharacterOneUsingAttackingSkill)
        {
            _gameCharacterOne_Skill_SubskillData = _gameCharacterOne_Skill.GetCharacterSubskillData().GetSubskillData();

            string _logOne = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>按下了的技能是"
                           + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterOne_Skill_SubskillData.DisplayName }</color>"
                           + $" （{ TerminologyManager.GetSkillInformationText( _gameCharacterOne_Skill ) }）。";

            BattleLog.Instance.AddOnScreenBattleLog( _logOne );
        }

        if (_isGameCharacterTwoUsingAttackingSkill)
        {
            _gameCharacterTwo_Skill_SubskillData = _gameCharacterTwo_Skill.GetCharacterSubskillData().GetSubskillData();

            string _logTwo = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>按下了的技能是"
                           + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterTwo_Skill_SubskillData.DisplayName }</color>"
                           + $" （{ TerminologyManager.GetSkillInformationText( _gameCharacterTwo_Skill ) }）。";

            BattleLog.Instance.AddOnScreenBattleLog( _logTwo );
        }

        // 如果雙方都有按下主動技能或反擊技能：
        if (_isGameCharacterOneUsingAttackingSkill && _isGameCharacterTwoUsingAttackingSkill)
        {
            BattleLog.Instance.AddOnScreenBattleLog( "雙方都有按下主動技能或反擊技能。" );

            int _gameCharacterOne_Skill_Speed = _gameCharacterOne_Skill_SubskillData.Speed + gameCharacterOne.GetCurrentSkillStatIncrement();
            int _gameCharacterTwo_Skill_Speed = _gameCharacterTwo_Skill_SubskillData.Speed + gameCharacterTwo.GetCurrentSkillStatIncrement();

            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>按下了的技能的速度是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetSpeedLevelText( _gameCharacterOne_Skill_Speed ) }</color>。" );
            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>按下了的技能的速度是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetSpeedLevelText( _gameCharacterTwo_Skill_Speed ) }</color>。" );

            // 對比雙方技能的速度。
            if (_gameCharacterOne_Skill_Speed > _gameCharacterTwo_Skill_Speed)
            {
                gameCharacterOne.SetCurrentCharacterIdentityType( CharacterIdentityType.Lead );
                gameCharacterTwo.SetCurrentCharacterIdentityType( CharacterIdentityType.Improviser );
            }
            else if (_gameCharacterOne_Skill_Speed < _gameCharacterTwo_Skill_Speed)
            {
                gameCharacterOne.SetCurrentCharacterIdentityType( CharacterIdentityType.Improviser );
                gameCharacterTwo.SetCurrentCharacterIdentityType( CharacterIdentityType.Lead );
            }
            // 如果雙方的技能速度相同：
            else
            {
                BattleLog.Instance.AddOnScreenBattleLog( "雙方按下了的技能速度相同。" );

                bool _isGameCharacterOneCounterAttacking = gameCharacterOne.GetIsCounterAttacking();
                bool _isGameCharacterTwoCounterAttacking = gameCharacterTwo.GetIsCounterAttacking();

                // 如果有一方在【 反擊指令時間 】階段按下主動技能或反擊技能：
                if (_isGameCharacterOneCounterAttacking || _isGameCharacterTwoCounterAttacking)
                {
                    gameCharacterOne.SetCurrentCharacterIdentityType( ( _isGameCharacterOneCounterAttacking ) ? CharacterIdentityType.Lead : CharacterIdentityType.Improviser );
                    gameCharacterTwo.SetCurrentCharacterIdentityType( ( _isGameCharacterTwoCounterAttacking ) ? CharacterIdentityType.Lead : CharacterIdentityType.Improviser );
                }
                // 否則：
                else
                {
                    float _minimumCurrentStatePoint = GameConfiguration.Instance.GetBattleConfiguration().GetMinimumCurrentStatePoint();
                    float _gameCharacterOne_StatePoint = gameCharacterOne.GetCurrentStatePoint() - _gameCharacterOne_Skill_SubskillData.StatePointCost;
                    float _gameCharacterTwo_StatePoint = gameCharacterTwo.GetCurrentStatePoint() - _gameCharacterTwo_Skill_SubskillData.StatePointCost;

                    if (_gameCharacterOne_StatePoint < _minimumCurrentStatePoint)
                    {
                        _gameCharacterOne_StatePoint = _minimumCurrentStatePoint;
                    }

                    if (_gameCharacterTwo_StatePoint < _minimumCurrentStatePoint)
                    {
                        _gameCharacterTwo_StatePoint = _minimumCurrentStatePoint;
                    }

                    BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>的當前以太值（扣除技能以太值消耗後）是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterOne_StatePoint }</color>。" );
                    BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>的當前以太值（扣除技能以太值消耗後）是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterTwo_StatePoint }</color>。" );

                    // 對比雙方的當前以太值（扣除了按下的技能的以太值消耗後的數值）：
                    if (_gameCharacterOne_StatePoint > _gameCharacterTwo_StatePoint)
                    {
                        gameCharacterOne.SetCurrentCharacterIdentityType( CharacterIdentityType.Lead );
                        gameCharacterTwo.SetCurrentCharacterIdentityType( CharacterIdentityType.Improviser );
                    }
                    else if (_gameCharacterOne_StatePoint < _gameCharacterTwo_StatePoint)
                    {
                        gameCharacterOne.SetCurrentCharacterIdentityType( CharacterIdentityType.Improviser );
                        gameCharacterTwo.SetCurrentCharacterIdentityType( CharacterIdentityType.Lead );
                    }
                    // 如果雙方的當前以太值（扣除了按下的技能的以太值消耗後的數值）也是相同:
                    else
                    {
                        BattleLog.Instance.AddOnScreenBattleLog( "雙方的當前以太值（扣除技能以太值消耗後）是相同。" );

                        if (Random.value < 0.5f)
                        {
                            gameCharacterOne.SetCurrentCharacterIdentityType( CharacterIdentityType.Lead );
                            gameCharacterTwo.SetCurrentCharacterIdentityType( CharacterIdentityType.Improviser );
                        }
                        else
                        {
                            gameCharacterOne.SetCurrentCharacterIdentityType( CharacterIdentityType.Improviser );
                            gameCharacterTwo.SetCurrentCharacterIdentityType( CharacterIdentityType.Lead );
                        }
                    }
                }
            }
        }
        // 如果只有一方按下主動技能或反擊技能：
        else if (_isGameCharacterOneUsingAttackingSkill || _isGameCharacterTwoUsingAttackingSkill)
        {
            BattleLog.Instance.AddOnScreenBattleLog( "只有一方按下主動技能或反擊技能。" );

            gameCharacterOne.SetCurrentCharacterIdentityType( ( _isGameCharacterOneUsingAttackingSkill ) ? CharacterIdentityType.Lead : CharacterIdentityType.Improviser );
            gameCharacterTwo.SetCurrentCharacterIdentityType( ( _isGameCharacterTwoUsingAttackingSkill ) ? CharacterIdentityType.Lead : CharacterIdentityType.Improviser );
        }
        // 否則：
        else
        {
            BattleLog.Instance.AddOnScreenBattleLog( "雙方都沒有按下主動技能或反擊技能。" );

            gameCharacterOne.SetCurrentCharacterIdentityType( CharacterIdentityType.None );
            gameCharacterTwo.SetCurrentCharacterIdentityType( CharacterIdentityType.None );
        }

        if (gameCharacterOne.GetCurrentCharacterIdentityType() == CharacterIdentityType.Lead
            && gameCharacterTwo.GetCurrentCharacterIdentityType() == CharacterIdentityType.Improviser)
        {
            return ( lead: gameCharacterOne, improviser: gameCharacterTwo );
        }

        if (gameCharacterOne.GetCurrentCharacterIdentityType() == CharacterIdentityType.Improviser
            && gameCharacterTwo.GetCurrentCharacterIdentityType() == CharacterIdentityType.Lead)
        {
            return ( lead: gameCharacterTwo, improviser: gameCharacterOne );
        }

        return ( lead: null, improviser: null );
    }

    public static void DeclareAssaulterAndRecipient( GameCharacter assaulter, GameCharacter recipient )
    {
        assaulter.SetCurrentCharacterIdentityType( CharacterIdentityType.Assaulter );
        recipient.SetCurrentCharacterIdentityType( CharacterIdentityType.Recipient );

        DetermineLightOrHeavyRecipient( recipient: recipient, assaulter: assaulter );
    }

    public static void DetermineLightOrHeavyRecipient( GameCharacter recipient, GameCharacter assaulter )
    {
        // TODO: Temporarily determine that the recipient is a heavy recipient.
        recipient.SetCurrentCharacterIdentityType( CharacterIdentityType.HeavyRecipient );
        assaulter.SetCurrentCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
    }

    public static void DetermineResultForPartA( GameCharacter lead, GameCharacter improviser )
    {
        CharacterSkill _leadCurrentSkill = lead.GetCurrentSkill();
        RangeType _leadCurrentSkillRangeType = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;

        // TODO: Temporarily determine that the lead's current skill's range type is melee if it is melee-or-ranged.
        if (_leadCurrentSkillRangeType == RangeType.melee_or_ranged)
        {
            _leadCurrentSkillRangeType = RangeType.melee;
        }

        lead.SetCurrentSkillRangeType( _leadCurrentSkillRangeType );
    }

    public static BattleResultData DetermineResultForPartB( GameCharacter lead, GameCharacter improviser, out GameCharacter winner, out GameCharacter loser, out List<string> resultLogList )
    {
        resultLogList = new List<string>();

        BattleResultData _battleResultData = new();
        winner = null;
        loser = null;

        CharacterSkill _leadCurrentSkill = lead.GetCurrentSkill();
        RangeType _leadRangeType = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;

        CharacterSkill _improviserCurrentSkill = improviser.GetCurrentSkill();
        RangeType _improviserRangeType = RangeType.none;

        if (_improviserCurrentSkill != null)
        {
            Skill _improviserSkillData = _improviserCurrentSkill.GetSkillData();
            Subskill _improviserSubskillData = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData();
            _improviserRangeType = _improviserSubskillData.Range;

            if (_improviserSkillData.skillType == Skill.SkillType.repulse)
            {
                float _stressValueDamageMultiplierOnRepulseForLoser = GameConfiguration.Instance.GetBattleConfiguration().GetStressValueDamageMultiplierOnRepulseForLoser();

                // 判定迎擊中途結果。
                CompareCharacterSkillAttributes( ActionType.Repulse, lead, improviser, out winner, out loser, ref resultLogList );

                // ----------------------------------------------------------------------------------------------------
                // Battle Log

                string _repulseResultLog = $"<color={ BattleLog.SPECIAL_COLOR_CODE }>判定迎擊中途結果</color>為";
                if (winner == lead)
                {
                    _repulseResultLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ lead.GetCharacterName() }</color>攻擊勝利。";
                }
                else if (winner == improviser)
                {
                    _repulseResultLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ improviser.GetCharacterName() }</color>迎擊勝利。";
                }
                else
                {
                    _repulseResultLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>雙方打平</color>。";
                }

                resultLogList.Add( _repulseResultLog );

                // ----------------------------------------------------------------------------------------------------

                if (_leadRangeType == RangeType.melee)
                {
                    if (_improviserRangeType == RangeType.melee)
                    {
                        if (winner == lead)
                        {
                            // 先手近戰攻擊，後手近戰迎擊，先手攻擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else if (winner == improviser)
                        {
                            // 先手近戰攻擊，後手近戰迎擊，後手迎擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: improviser, recipient: lead );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else
                        {
                            // 先手近戰攻擊，後手近戰迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true );
                        }
                    }
                    else if (_improviserRangeType == RangeType.ranged)
                    {
                        if (winner == lead)
                        {
                            // 先手近戰攻擊，後手遠程迎擊，先手攻擊勝利。
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else if (winner == improviser)
                        {
                            // 先手近戰攻擊，後手遠程迎擊，後手迎擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: improviser, recipient: lead );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                        }
                        else
                        {
                            // 先手近戰攻擊，後手遠程迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true );
                        }
                    }
                }
                else if (_leadRangeType == RangeType.ranged)
                {
                    if (_improviserRangeType == RangeType.melee)
                    {
                        if (winner == lead)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，先手攻擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                        }
                        else if (winner == improviser)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，後手迎擊勝利。
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else
                        {
                            // 先手遠程攻擊，後手近戰迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true );
                        }
                    }
                    else if (_improviserRangeType == RangeType.ranged)
                    {
                        if (winner == lead)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，先手攻擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true );
                        }
                        else if (winner == improviser)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，後手迎擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: improviser, recipient: lead );
                            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true );
                        }
                        else
                        {
                            // 先手遠程攻擊，後手近戰迎擊，雙方打平。
                        }
                    }
                }
            }
            else if (_improviserSubskillData.IsDefendingSkill || _improviserSubskillData.IsEvadingSkill)
            {
                if (_improviserSubskillData.IsDefendingSkill)
                {
                    // 判定防禦成敗。
                    CompareCharacterSkillAttributes( ActionType.Defend, lead, improviser, out winner, out loser, ref resultLogList );
                }
                else if (_improviserSubskillData.IsEvadingSkill)
                {
                    // 判定迴避成敗。
                    CompareCharacterSkillAttributes( ActionType.Evade, lead, improviser, out winner, out loser, ref resultLogList );
                }

                // ----------------------------------------------------------------------------------------------------
                // Battle Log

                string _resultLog = $"<color={ BattleLog.SPECIAL_COLOR_CODE }>判定結果</color>為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ improviser.GetCharacterName() }</color>";

                if (_improviserSubskillData.IsDefendingSkill)
                {
                    _resultLog += "防禦";
                }
                else if (_improviserSubskillData.IsEvadingSkill)
                {
                    _resultLog += "迴避";
                }

                _resultLog += $"{ ( ( winner == improviser ) ? "成功" : "失敗" ) }。";

                resultLogList.Add( _resultLog );

                // ----------------------------------------------------------------------------------------------------

                if (winner == lead)
                {
                    DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
                }
                else if (winner == improviser)
                {
                    lead.SetCurrentCharacterIdentityType( CharacterIdentityType.Deuce );

                    if (_improviserSubskillData.IsDefendingSkill)
                    {
                        improviser.SetCurrentCharacterIdentityType( CharacterIdentityType.SuccessfulDefender );
                    }
                    else if (_improviserSubskillData.IsEvadingSkill)
                    {
                        improviser.SetCurrentCharacterIdentityType( CharacterIdentityType.SuccessfulEvader );
                    }
                }

                ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasHealthPointDamage: winner == lead, hasStatePointDamage: true, hasStressValueDamage: true );
            }
        }
        else
        {
            DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
            ExecuteCasterSkillOnHit( ref _battleResultData, ref resultLogList, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true );

            winner = lead;
            loser = improviser;
        }

        return _battleResultData;
    }

    public static void ExecuteCasterSkillOnUse( ref BattleResultData battleResultData, GameCharacter caster, GameCharacter target )
    {
        CharacterSkill _casterSkill = caster.GetCurrentSkill();
        Subskill _casterSubskillData = _casterSkill.GetCharacterSubskillData().GetSubskillData();

        float _statePointCost = BattleCalculationManager.GetStatePointCost( _casterSubskillData );
        string _statePointCostLog = "";

        // 如果“後手方”已按下的技能有“已更新按下技能”的標記，那麼該技能的以太值消耗會增加基礎值的倍數。
        if (_casterSkill.GetHasSkillUpdateIndicator())
        {
            _casterSkill.SetHasSkillUpdateIndicator( false );
            _statePointCost *= GameConfiguration.Instance.GetBattleConfiguration().GetStatePointCostMultiplierOnSkillUpdate();
            _statePointCostLog = "（有“已更新按下技能”的標記";
        }

        if (_casterSubskillData.IsEvadingSkill)
        {
            Subskill _targetSubskillData = target.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
            float _evasionStress = _targetSubskillData.EvasionStress * ( ( caster.HasEnergyMarker() ) ? _targetSubskillData.EnergyMarkerEvasionStressRate : 1.0f );
            _statePointCost += _evasionStress;

            if (_statePointCostLog == "")
            {
                _statePointCostLog += "（";
            }
            else
            {
                _statePointCostLog += "、";
            }

            _statePointCostLog += $"迴避壓力：{ _evasionStress }";
        }

        if (_statePointCostLog != "")
        {
            _statePointCostLog += "）";
        }

        _statePointCost = BattleCalculationManager.AdjustAmount( _statePointCost );
        battleResultData.AddGameCharacterResultData_StatePointCost( caster, _statePointCost, out _ );

        float _maxStatePointUp = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetMaxStatePointUp( _casterSubskillData ) );
        battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease( caster, _maxStatePointUp, out _ );

        string _log = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ caster.GetCharacterName() }</color>使出了"
                    + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _casterSubskillData.DisplayName }</color>"
                    + $" （{ TerminologyManager.GetSkillInformationText( _casterSkill ) }）";

        string _extraLog = "";

        if (_statePointCost > 0)
        {
            _extraLog += $"，消耗了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _statePointCost }{ TerminologyManager.STATE_POINT }</color>";

            if (_statePointCostLog != "")
            {
                _extraLog += _statePointCostLog;
            }
        }

        if (_maxStatePointUp > 0)
        {
            if (_extraLog != "")
            {
                _extraLog += "和";
            }

            _extraLog += $"提升了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _maxStatePointUp }最大{ TerminologyManager.STATE_POINT }</color>";
        }

        if (_extraLog != "")
        {
            _extraLog += "。";
        }

        if (_extraLog == "")
        {
            _log += "。";
        }
        else
        {
            _log += _extraLog;
        }

        BattleLog.Instance.AddOnScreenBattleLog( _log );
    }

    private static void ExecuteCasterSkillOnHit( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter caster, GameCharacter target,
                                                 bool hasHealthPointDamage = false, bool hasStatePointDamage = false, bool hasStressValueDamage = false,
                                                 bool isBreakStatusAvailable = true, float stressValueDamageMultiplier = 1.0f )
    {
        string _resultLog = "";

        CharacterSkill _casterSkill = caster.GetCurrentSkill();
        Subskill _casterSubskillData = _casterSkill.GetCharacterSubskillData().GetSubskillData();
        BattleResultData_GameCharacter _casterBattleResultData = null;

        CharacterSkill _targetSkill = target.GetCurrentSkill();
        Subskill _targetSubskillData = _targetSkill?.GetCharacterSubskillData().GetSubskillData();
        BattleResultData_GameCharacter _targetBattleResultData = null;

        int _energyMarkerAtl = _casterSubskillData.EnergyMarkerATL;
        if (_energyMarkerAtl > 0)
        {
            // 更新“能量殘響”。
            battleResultData.AddGameCharacterResultData_RenewedEnergyMarkerATLs( target, _energyMarkerAtl, out _targetBattleResultData );

            _resultLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>的<color={ BattleLog.KEYWORD_COLOR_CODE }>【能量殘響】維持值</color>被更新為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _energyMarkerAtl }個ATL</color>。";
        }

        // 在發生迎擊時，如果攻擊或迎擊技能是近戰，而對方的迎擊或攻擊技能是遠程，就會得到額外的最大以太值提升。
        // 近戰攻擊 VS 遠程迎擊：近戰攻擊得到額外的最大以太值提升。
        // 近戰迎擊 VS 遠程攻擊：近戰迎擊得到額外的最大以太值提升。
        if (_targetSkill != null)
        {
            if (_casterSkill.GetSkillData().skillType == Skill.SkillType.repulse
                || _targetSkill.GetSkillData().skillType == Skill.SkillType.repulse)
            {
                if (_casterSubskillData.Range == Subskill.RangeType.melee
                    && _targetSubskillData?.Range == Subskill.RangeType.ranged)
                {
                    float _maxStatePointUp = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetMaxStatePointUp( _casterSubskillData ) );
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease( caster, _maxStatePointUp, out _casterBattleResultData );

                    _resultLog += $"在發生迎擊時，<color={ BattleLog.KEYWORD_COLOR_CODE }>{ caster.GetCharacterName() }</color>得到額外的<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _maxStatePointUp }最大{ TerminologyManager.STATE_POINT }</color>。";
                }
            }
        }

        bool _isTargetHavingEnergyMarker = false;
        bool _isTargetInBreakStatus = false;
        string _extraLog = "";

        // 以太傷害
        if (hasStatePointDamage)
        {
            _isTargetHavingEnergyMarker = ( _targetBattleResultData != null ) ? _targetBattleResultData.HasEnergyMarker() : target.HasEnergyMarker();

            float _statePointDamage = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetStatePointDamage( _casterSubskillData ) * ( ( _isTargetHavingEnergyMarker ) ? _casterSubskillData.EnergyMarkerStateDamageRate : 1.0f ) );
            battleResultData.AddGameCharacterResultData_StatePointDamage( target, _statePointDamage, isBreakStatusAvailable, out _targetBattleResultData );

            if (_statePointDamage > 0)
            {
                _extraLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _statePointDamage }{ TerminologyManager.STATE_POINT }傷害</color>{ ( ( _isTargetHavingEnergyMarker ) ? "（帶有能量殘響）" : "" ) }";
            }
        }

        // 負荷傷害
        if (hasStressValueDamage)
        {
            _isTargetHavingEnergyMarker = ( _targetBattleResultData != null ) ? _targetBattleResultData.HasEnergyMarker() : target.HasEnergyMarker();

            float _stressValueDamage = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetStressValueDamage( _casterSubskillData ) * ( ( _isTargetHavingEnergyMarker ) ? _casterSubskillData.EnergyMarkerStressDamageRate : 1.0f ) * stressValueDamageMultiplier );
            battleResultData.AddGameCharacterResultData_StressValueDamage( target, _stressValueDamage, isBreakStatusAvailable, out _targetBattleResultData );

            if (_stressValueDamage > 0)
            {
                _extraLog += $"{ ( ( _extraLog != "" ) ? "、" : "" ) }<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _stressValueDamage }%負荷值傷害</color>{ ( ( _isTargetHavingEnergyMarker ) ? "（帶有能量殘響）" : "" ) }";
            }
        }

        // HP 傷害
        if (hasHealthPointDamage)
        {
            _isTargetHavingEnergyMarker = ( _targetBattleResultData != null ) ? _targetBattleResultData.HasEnergyMarker() : target.HasEnergyMarker();
            _isTargetInBreakStatus = ( _targetBattleResultData != null ) ? _targetBattleResultData.IsInBreakStatus() : target.IsInBreakStatus();

            float _healthPointDamage = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetCurrentAttackDamage( _casterSkill, caster, target, _isTargetHavingEnergyMarker, _isTargetInBreakStatus ) );
            if (_healthPointDamage > 0)
            {
                // TODO: 參考 Flowchart 裡的“虛傷實傷解說”。
                // 當角色受到直擊傷害時：
                // 如果該角色是“輕受擊方”，該直擊傷害為“虛傷”。
                // 如果該角色是“重受擊方”，該直擊傷害為“實傷”。

                bool _isActualDamage = ( target.GetCurrentCharacterIdentityType() == CharacterIdentityType.HeavyRecipient );

                /*
                // When the target takes damage, if the target is not using the Repulse skill
                // and Defending skill, the target will take the actual damage.
                if (_targetSkill != null)
                {
                    if (_targetSkill.GetSkillData().skillType != Skill.SkillType.repulse
                        && _targetSubskillData?.IsDefendingSkill == false)
                    {
                        _isActualDamage = true;
                    }
                }
                else
                {
                    _isActualDamage = true;
                }
                */

                if (_isActualDamage)
                {
                    // 實傷
                    battleResultData.AddGameCharacterResultData_ActualHealthPointDamage( target, _healthPointDamage, out _targetBattleResultData );
                }
                else
                {
                    // 虛傷
                    battleResultData.AddGameCharacterResultData_VirtualHealthPointDamage( target, _healthPointDamage, out _targetBattleResultData );
                }

                if (_healthPointDamage > 0)
                {
                    _extraLog += $"{ ( ( _extraLog != "" ) ? "、" : "" ) }<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _healthPointDamage }HP值傷害</color>（{ ( ( _isActualDamage ) ? "實傷" : "虛傷" ) }{ ( ( _isTargetHavingEnergyMarker ) ? "、帶有能量殘響" : "" ) }{ ( ( _isTargetInBreakStatus ) ? "、處於崩潰狀態" : "" ) }）";
                }
            }
        }

        if (_extraLog != "")
        {
            _resultLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>受到了{ _extraLog }。";
        }

        if (_targetBattleResultData.IsInBreakStatus() && hasHealthPointDamage)
        {
            // 尚未回復的虛傷部分全數轉化為實傷。
            battleResultData.AddGameCharacterResultData_ConvertAllVirtualDamageToActualDamage( target, out _targetBattleResultData );

            _resultLog += $"由於在崩潰狀態時受到了HP值傷害，<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>的HP值裡尚未回復的虛傷值</color>全數轉化為<color={ BattleLog.KEYWORD_COLOR_CODE }>實傷值</color>。";
        }

        if (_casterSubskillData.WillRemoveEnergyMarker)
        {
            // 消去“能量殘響”。
            battleResultData.AddGameCharacterResultData_RemoveEnergyMarker( target, out _targetBattleResultData );

            _resultLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>帶有的<color={ BattleLog.KEYWORD_COLOR_CODE }>【能量殘響】</color>被消去了。";
        }

        if (_resultLog != "")
        {
            resultLogList.Add( _resultLog );
        }
    }

    public static void ExecuteObservingSkill( GameCharacter caster, GameCharacter target, out string resultLog )
    {
        resultLog = "";

        CharacterSkill _casterObservingSkill = caster.GetCurrentObservingSkill();

        // 如果新的看破 ID 與舊的看破 ID 不一致，更新看破技能中鎖定的技能和增加看破技能的保護值。
        if (_casterObservingSkill != null)
        {
            CharacterSkill _targetSkill = target.GetCurrentSkill();
            Subskill _targetSkillSubskillData = _targetSkill.GetCharacterSubskillData().GetSubskillData();

            if (_casterObservingSkill.GetObservedSkillRecord() == null
                || _targetSkillSubskillData.FeatureId != _casterObservingSkill.GetObservedSkillRecord().GetSubskillData().FeatureId)
            {
                _casterObservingSkill.SetObservedSkillRecord( new ObservedSkillRecord( _targetSkillSubskillData ) );
                _casterObservingSkill.SetObservingSkillProtectionValue( _casterObservingSkill.GetObservingSkillProtectionValue() + 1 );
            }

            _casterObservingSkill.GetObservedSkillRecord().SetIsRecording( true );

            resultLog = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ caster.GetCharacterName() }</color>使用<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _casterObservingSkill.GetCharacterSubskillData().GetSubskillData().DisplayName }</color>（看破技能）來看破"
                      + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>使用的<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _targetSkill.GetCharacterSubskillData().GetSubskillData().DisplayName }</color>。";
        }
    }

    private static int GetSkillStatIncrement( GameCharacter observer, CharacterSkill targetSkill )
    {
        int _targetSkillFeatureId = targetSkill.GetCharacterSubskillData().GetSubskillData().FeatureId;

        List<CharacterSkill> _selectedBackendSkillList = observer.GetSelectedBackendSkillList();
        for (int i = 0; i < _selectedBackendSkillList.Count; i++)
        {
            CharacterSkill _selectedBackendSkill = _selectedBackendSkillList[ i ];
            if (_selectedBackendSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
            {
                ObservedSkillRecord _observedSkillRecord = _selectedBackendSkill.GetObservedSkillRecord();
                if (_observedSkillRecord != null)
                {
                    if (_targetSkillFeatureId == _observedSkillRecord.GetSubskillData().FeatureId)
                    {
                        return Mathf.FloorToInt( _observedSkillRecord.GetCurrentObservedRate() );
                    }
                }
            }
        }

        return 0;
    }

    private static void CompareCharacterSkillAttributes( ActionType actionType, GameCharacter lead, GameCharacter improviser,
                                                         out GameCharacter winner, out GameCharacter loser, ref List<string> resultLogList )
    {
        winner = null;
        loser = null;

        CharacterSkill _leadCurrentSkill = lead.GetCurrentSkill();
        CharacterSkill _improviserCurrentSkill = improviser.GetCurrentSkill();
        string _resultLog = "";

        int _leadSkillStatIncrement = GetSkillStatIncrement( lead, _improviserCurrentSkill );
        Subskill _leadSubskillData = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData();
        int _leadSkillSpeed = _leadSubskillData.Speed + _leadSkillStatIncrement;
        int _leadSkillStrength = _leadSubskillData.Strength + _leadSkillStatIncrement;
        int _leadSkillEffectType = ( int )_leadSubskillData.EffectType;

        int _improviserSkillStatIncrement = GetSkillStatIncrement( improviser, _leadCurrentSkill );
        Subskill _improviserSubskillData = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData();
        int _improviserSkillSpeed = _improviserSubskillData.Speed + _improviserSkillStatIncrement;
        int _improviserSkillStrength = _improviserSubskillData.Strength + _improviserSkillStatIncrement;
        int _improviserSkillEffectType = ( int )_improviserSubskillData.EffectType;

        if (_leadSkillStatIncrement > 0)
        {
            _resultLog = $"因為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ lead.GetCharacterName() }</color>已看破<color={ BattleLog.KEYWORD_COLOR_CODE }>{ improviser.GetCharacterName() }</color>使用的"
                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _improviserSubskillData.DisplayName }</color>，所以<color={ BattleLog.KEYWORD_COLOR_CODE }>{ lead.GetCharacterName() }</color>使用的"
                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _leadSubskillData.DisplayName }</color>的強度和速度得到<color={ BattleLog.KEYWORD_COLOR_CODE }>+{ _leadSkillStatIncrement }</color>。";

            resultLogList.Add( _resultLog );
        }

        if (_improviserSkillStatIncrement > 0)
        {
            _resultLog = $"因為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ improviser.GetCharacterName() }</color>已看破<color={ BattleLog.KEYWORD_COLOR_CODE }>{ lead.GetCharacterName() }</color>使用的"
                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _leadSubskillData.DisplayName }</color>，所以<color={ BattleLog.KEYWORD_COLOR_CODE }>{ improviser.GetCharacterName() }</color>使用的"
                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _improviserSubskillData.DisplayName }</color>的強度和速度得到<color={ BattleLog.KEYWORD_COLOR_CODE }>+{ _improviserSkillStatIncrement }</color>。";

            resultLogList.Add( _resultLog );
        }

        switch ( actionType )
        {
            case ActionType.Repulse:

                if (_leadSkillSpeed > _improviserSkillSpeed)
                {
                    winner = lead;
                }
                else
                {
                    if (_leadSkillStrength > _improviserSkillStrength)
                    {
                        winner = lead;
                    }
                    else if (_leadSkillStrength < _improviserSkillStrength)
                    {
                        winner = improviser;
                    }
                }

                break;

            case ActionType.Defend:

                if (_leadSkillSpeed > _improviserSkillSpeed)
                {
                    winner = lead;
                }
                else
                {
                    if (_leadSkillEffectType > _improviserSkillEffectType)
                    {
                        winner = lead;
                    }
                    else
                    {
                        if (_leadSkillStrength > _improviserSkillStrength)
                        {
                            winner = lead;
                        }
                        else
                        {
                            winner = improviser;
                        }
                    }
                }

                break;

            case ActionType.Evade:

                if (_leadSkillSpeed > _improviserSkillSpeed)
                {
                    winner = lead;
                }
                else
                {
                    winner = improviser;
                }

                break;
        }

        if (winner == lead)
        {
            loser = improviser;
        }
        else if (winner == improviser)
        {
            loser = lead;
        }
    }

    public static void OnTheStartOfRound( GameCharacter[] gameCharacters )
    {

    }

    public static BattleResultData OnTheStartOfATL( GameCharacter[] gameCharacters )
    {
        BattleResultData _battleResultData = new();

        for (int i = 0; i < gameCharacters.Length; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];
            _gameCharacter.TriggerEvent( BattleAnimationManager.AnimationEvent.SetCharacter );

            // 以太崩潰狀態
            int _stateBreakStatusRemainingATLs = _gameCharacter.GetStateBreakStatusRemainingATLs();
            float _maximumStatePoint = _gameCharacter.GetMaximumStatePoint();
            float _currentStatePoint = _gameCharacter.GetCurrentStatePoint();
            if (_stateBreakStatusRemainingATLs > 0)
            {
                _stateBreakStatusRemainingATLs--;

                if (_stateBreakStatusRemainingATLs <= 0)
                {
                    _maximumStatePoint = _gameCharacter.GetOriginalStatePoint();
                    _currentStatePoint = _maximumStatePoint;
                }
            }

            // 負荷崩潰狀態
            int _stressBreakStatusRemainingATLs = _gameCharacter.GetStressBreakStatusRemainingATLs();
            float _currentStressValue = _gameCharacter.GetCurrentStressValue();
            if (_stressBreakStatusRemainingATLs > 0)
            {
                _stressBreakStatusRemainingATLs--;

                if (_stressBreakStatusRemainingATLs <= 0)
                {
                    _currentStressValue = 0.0f;
                }
            }

            _battleResultData.AddGameCharacterResultData( _gameCharacter,
                stateBreakStatusRemainingATLs: _stateBreakStatusRemainingATLs, maximumStatePoint: _maximumStatePoint, currentStatePoint: _currentStatePoint,
                stressBreakStatusRemainingATLs: _stressBreakStatusRemainingATLs, currentStressValue: _currentStressValue );
        }

        return _battleResultData;
    }

    public static void OnTheEndOfPartB( GameCharacter[] gameCharacters, out List<string> resultLogList )
    {
        resultLogList = new List<string>();

        for (int i = 0; i < gameCharacters.Length; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];

            // 看破技能記錄裡鎖定的看破 ID 的儲蓄值的加算。
            CharacterSkill _observingSkill = _gameCharacter.GetCurrentObservingSkill();
            if (_observingSkill != null)
            {
                ObservedSkillRecord _observedSkillRecord = _observingSkill.GetObservedSkillRecord();
                if (_observedSkillRecord.GetIsRecording())
                {
                    float _observationRate = _observingSkill.GetCharacterSubskillData().GetSubskillData().ObservationRate;

                    _observedSkillRecord.SetCurrentObservedRate( Mathf.Clamp( _observedSkillRecord.GetCurrentObservedRate() + _observationRate, 0.0f, GameConfiguration.Instance.GetBattleConfiguration().GetMaximumObservedRate() ) );

                    string _resultLog = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacter.GetCharacterName() }</color>使用的<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observingSkill.GetCharacterSubskillData().GetSubskillData().DisplayName }</color>（看破技能）對"
                                      + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observedSkillRecord.GetSubskillData().DisplayName }</color>的<color={ BattleLog.KEYWORD_COLOR_CODE }>看破儲蓄值</color>增加"
                                      + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observationRate.ConvertToIntegerInPercentage() }%</color>至<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observedSkillRecord.GetCurrentObservedRate().ConvertToIntegerInPercentage() }%</color>。";

                    resultLogList.Add( _resultLog );
                }
            }

            _gameCharacter.ResetCurrentObservingSkill();

            if (_gameCharacter.GetIsDead())
            {
                _gameCharacter.TriggerEvent( BattleAnimationManager.AnimationEvent.OnDeath );
            }
        }
    }

    public static void OnTheEndOfATL( GameCharacter[] gameCharacters )
    {
        for (int i = 0; i < gameCharacters.Length; i++)
        {
            gameCharacters[ i ].TriggerEvent( BattleAnimationManager.AnimationEvent.OnAtlEnded );
        }
    }

    public static BattleResultData OnTheEndOfRound( GameCharacter[] gameCharacters, out List<string> resultLogList )
    {
        BattleResultData _battleResultData = new();
        resultLogList = new List<string>();

        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();
        int _stressValueDecreaseOnRoundStart = _battleConfiguration.GetStressValueDecreaseOnRoundStart();
        float _healthPointRegenerationRateOnRoundStart = _battleConfiguration.GetHealthPointRegenerationRateOnRoundStart();
        int _maximumStatePointIncreaseOnRoundStart = _battleConfiguration.GetMaximumStatePointIncreaseOnRoundStart();

        for (int i = 0; i < gameCharacters.Length; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];
            float _virtualHealthPointDamageRecovered = _gameCharacter.GetMaximumHealthPoint() * _healthPointRegenerationRateOnRoundStart;
            float _currentStatePoint = _gameCharacter.GetCurrentStatePoint();
            float _maximumStatePointDecrease = ( _currentStatePoint < 0 ) ? Mathf.Abs( _currentStatePoint ) : 0.0f;
            float _maximumStatePointIncrease = _maximumStatePointIncreaseOnRoundStart;
            float _stressValueDamageRecovered = _stressValueDecreaseOnRoundStart;

            // 回復受到的“虛傷”，回復值為最大生命(HP)值的指定巴仙率。
            _battleResultData.AddGameCharacterResultData_VirtualHealthPointDamageRecovered( _gameCharacter, _virtualHealthPointDamageRecovered, out _ )

                // 如果透支以太值至負數去消費，最大以太值將減去負數值。
                .AddGameCharacterResultData_MaximumStatePointDecrease( _gameCharacter, _maximumStatePointDecrease, out _ )

                // 提升最大以太值。
                .AddGameCharacterResultData_MaximumStatePointIncrease( _gameCharacter, _maximumStatePointIncrease, out _ )

                // 降低當前負荷值。
                .AddGameCharacterResultData_StressValueDamageRecovered( _gameCharacter, _stressValueDamageRecovered, out _ )

                // 當前以太值回復至最大以太值的100%。
                .AddGameCharacterResultData_FullyRestoreCurrentStatePoint( _gameCharacter, out _ );

            BattleResultData_GameCharacter _gameCharacterResultData = _battleResultData.GetGameCharacterResultData( _gameCharacter );

            string _resultLog = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacter.GetCharacterName() }</color>"
                              + ( ( _virtualHealthPointDamageRecovered > 0 ) ? $"回復了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _virtualHealthPointDamageRecovered }虛傷值</color>，" : "" );

            if (_maximumStatePointDecrease > 0)
            {
                _resultLog += $"因<color={ BattleLog.KEYWORD_COLOR_CODE }>當前{ TerminologyManager.STATE_POINT }</color>為負數而導致"
                            + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>最大{ TerminologyManager.STATE_POINT }</color>"
                            + $"減少了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _maximumStatePointDecrease }</color>，";
            }

            if (_maximumStatePointIncrease > 0)
            {
                _resultLog += $"提升了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _maximumStatePointIncrease }最大{ TerminologyManager.STATE_POINT }</color>，";
            }

            _resultLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>最大{ TerminologyManager.STATE_POINT }</color>現為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterResultData.maximumStatePoint }</color>，"
                        + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>當前{ TerminologyManager.STATE_POINT }</color>已回復至最大值"
                        + ( ( _stressValueDamageRecovered > 0 ) ? $"，減少了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _stressValueDamageRecovered }%負荷值</color>" : "" )
                        + "。";

            resultLogList.Add( _resultLog );

            // 看破技能記錄裡鎖定的看破 ID 的儲蓄值的減算。
            CharacterSkill[] _skills = _gameCharacter.GetSkills();
            for (int j = 0; j < _skills.Length; j++)
            {
                CharacterSkill _skill = _skills[ j ];
                if (_skill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
                {
                    int _observingSkillProtectionValue = _skill.GetObservingSkillProtectionValue();
                    if (_observingSkillProtectionValue > 0)
                    {
                        _skill.SetObservingSkillProtectionValue( _observingSkillProtectionValue - 1 );
                    }
                    else
                    {
                        ObservedSkillRecord _observedSkillRecord = _skill.GetObservedSkillRecord();
                        if (_observedSkillRecord != null)
                        {
                            float _observationRateDeductionPerRound = GameConfiguration.Instance.GetBattleConfiguration().GetObservationRateDeductionPerRound();
                            Subskill _subskillData = _observedSkillRecord.GetSubskillData();

                            _observedSkillRecord.SetCurrentObservedRate( _observedSkillRecord.GetCurrentObservedRate() - _observationRateDeductionPerRound );

                            _resultLog = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacter.GetCharacterName() }</color>的<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _skill.GetCharacterSubskillData().GetSubskillData().DisplayName }</color>（看破技能）的記錄裡對"
                                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _subskillData.DisplayName }</color>的<color={ BattleLog.KEYWORD_COLOR_CODE }>看破儲蓄值</color>減少"
                                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observationRateDeductionPerRound.ConvertToIntegerInPercentage() }%</color>至<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observedSkillRecord.GetCurrentObservedRate().ConvertToIntegerInPercentage() }%</color>";

                            if (_observedSkillRecord.GetSubskillData() == null)
                            {
                                _skill.ResetObservedSkillRecord();
                                _resultLog += "和該記錄被刪除了";
                            }

                            _resultLog += "。";

                            resultLogList.Add( _resultLog );
                        }
                    }
                }
            }
        }

        return _battleResultData;
    }

    public static void OnGameCharacterBeingInBreakStatus( GameCharacter gameCharacter )
    {
        if (gameCharacter.GetCurrentCharacterIdentityType() is CharacterIdentityType.Recipient
                                                            or CharacterIdentityType.LightRecipient
                                                            or CharacterIdentityType.SuccessfulResister
                                                            or CharacterIdentityType.SuccessfulDefender
                                                            or CharacterIdentityType.SuccessfulEvader)
        {
            gameCharacter.SetCurrentCharacterIdentityType( CharacterIdentityType.HeavyRecipient );
            gameCharacter.GetCurrentAttacker().SetCurrentCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
        }
    }
}
