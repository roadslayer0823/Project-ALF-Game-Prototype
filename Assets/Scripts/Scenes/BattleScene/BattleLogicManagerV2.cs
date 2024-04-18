using UnityEngine;
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
        if (skill != null)
        {
            Skill _skillData = skill.GetSkillData();
            SkillType _skillType = _skillData.skillType;

            if (_skillType == SkillType.active
                || _skillType == SkillType.derived
                || _skillType == SkillType.counter)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsAbleToUseAnySkill( GameCharacter gameCharacter )
    {
        if (gameCharacter.GetIsInBreakStatus())
        {
            return false;
        }

        return true;
    }

    public static bool IsAbleToUseAttackingAndDefendingSkills( GameCharacter gameCharacter )
    {
        if (IsAbleToUseAnySkill( gameCharacter ))
        {
            if (gameCharacter.GetCurrentStatePoint() > GameConfiguration.Instance.GetBattleConfiguration().GetMinimumCurrentStatePoint())
            {
                return true;
            }
        }

        return false;
    }

    public static bool ShouldCombatCommandTimeBeSkipped( GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        CharacterSkill _gameCharacterOne_Skill = gameCharacterOne.GetAssignedSkill();
        CharacterSkill _gameCharacterTwo_Skill = gameCharacterTwo.GetAssignedSkill();

        if (_gameCharacterOne_Skill != null && _gameCharacterOne_Skill.GetSkillData().skillType == SkillType.derived)
        {
            _gameCharacterTwo_Skill = gameCharacterTwo.ResetAssignedSkill();
        }
        else if (_gameCharacterTwo_Skill != null && _gameCharacterTwo_Skill.GetSkillData().skillType == SkillType.derived)
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
                gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
            }
            else if (_gameCharacterOne_Skill_Speed < _gameCharacterTwo_Skill_Speed)
            {
                gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
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
                    gameCharacterOne.SetCurrentCharacterIdentityType( ( _isGameCharacterOneCounterAttacking ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
                    gameCharacterTwo.SetCurrentCharacterIdentityType( ( _isGameCharacterTwoCounterAttacking ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
                }
                // 否則：
                else
                {
                    float _gameCharacterOne_StatePoint = gameCharacterOne.GetCurrentStatePoint() - _gameCharacterOne_Skill_SubskillData.StatePointCost;
                    float _gameCharacterTwo_StatePoint = gameCharacterTwo.GetCurrentStatePoint() - _gameCharacterTwo_Skill_SubskillData.StatePointCost;

                    BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>的當前以太值（扣除技能以太值消耗後）是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterOne_StatePoint }</color>。" );
                    BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>的當前以太值（扣除技能以太值消耗後）是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterTwo_StatePoint }</color>。" );

                    // 對比雙方的當前以太值（扣除了按下的技能的以太值消耗後的數值）：
                    if (_gameCharacterOne_StatePoint > _gameCharacterTwo_StatePoint)
                    {
                        gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                        gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                    }
                    else if (_gameCharacterOne_StatePoint < _gameCharacterTwo_StatePoint)
                    {
                        gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                        gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                    }
                    // 如果雙方的當前以太值（扣除了按下的技能的以太值消耗後的數值）也是相同:
                    else
                    {
                        BattleLog.Instance.AddOnScreenBattleLog( "雙方的當前以太值（扣除技能以太值消耗後）是相同。" );

                        if (Random.value < 0.5f)
                        {
                            gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                            gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                        }
                        else
                        {
                            gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                            gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                        }
                    }
                }
            }
        }
        // 如果只有一方按下主動技能或反擊技能：
        else if (_isGameCharacterOneUsingAttackingSkill || _isGameCharacterTwoUsingAttackingSkill)
        {
            BattleLog.Instance.AddOnScreenBattleLog( "只有一方按下主動技能或反擊技能。" );

            gameCharacterOne.SetCurrentCharacterIdentityType( ( _isGameCharacterOneUsingAttackingSkill ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
            gameCharacterTwo.SetCurrentCharacterIdentityType( ( _isGameCharacterTwoUsingAttackingSkill ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
        }
        // 否則：
        else
        {
            BattleLog.Instance.AddOnScreenBattleLog( "雙方都沒有按下主動技能或反擊技能。" );

            gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.None );
            gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.None );
        }

        if (gameCharacterOne.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Lead
            && gameCharacterTwo.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Improviser)
        {
            return ( lead: gameCharacterOne, improviser: gameCharacterTwo );
        }

        if (gameCharacterOne.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Improviser
            && gameCharacterTwo.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Lead)
        {
            return ( lead: gameCharacterTwo, improviser: gameCharacterOne );
        }

        return ( lead: null, improviser: null );
    }

    public static BattleResultData DetermineResultForPartB( GameCharacter lead, GameCharacter improviser, out GameCharacter winner, out GameCharacter loser )
    {
        BattleResultData _battleResultData = new();
        winner = null;
        loser = null;

        CharacterSkill _leadCurrentSkill = lead.GetCurrentSkill();
        RangeType _leadRangeType = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;

        // TODO: Temporarily hardcoded the player character's and enemy character's attacking skills' range types because of the attacking skill animations that they have.
        if (lead.GetIsPlayer())
        {
            _leadRangeType = RangeType.ranged;
        }
        else
        {
            _leadRangeType = RangeType.melee;
        }

        CharacterSkill _improviserCurrentSkill = improviser.GetCurrentSkill();
        RangeType _improviserRangeType = RangeType.none;

        if (_improviserCurrentSkill != null)
        {
            ExecuteCasterSkillOnUse( ref _battleResultData, improviser, lead );

            Skill _improviserSkillData = _improviserCurrentSkill.GetSkillData();
            Subskill _improviserSubskillData = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData();
            _improviserRangeType = _improviserSubskillData.Range;

            if (_improviserSkillData.skillType == Skill.SkillType.repulse)
            {
                // TODO: Temporarily hardcoded that the enemy characeter will always use melee repulse skills.
                if (!improviser.GetIsPlayer())
                {
                    _improviserRangeType = RangeType.melee;
                }

                float _stressValueDamageMultiplierOnRepulseForLoser = GameConfiguration.Instance.GetBattleConfiguration().GetStressValueDamageMultiplierOnRepulseForLoser();

                // 判定迎擊中途結果。
                CompareCharacterSkillAttributes( ActionType.Repulse, lead, improviser, out winner, out loser );

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

                BattleLog.Instance.AddOnScreenBattleLog( _repulseResultLog );

                if (_leadRangeType == RangeType.melee)
                {
                    if (_improviserRangeType == RangeType.melee)
                    {
                        if (winner == lead)
                        {
                            // 先手近戰攻擊，後手近戰迎擊，先手攻擊勝利。
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else if (winner == improviser)
                        {
                            // 先手近戰攻擊，後手近戰迎擊，後手迎擊勝利。
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else
                        {
                            // 先手近戰攻擊，後手近戰迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true );
                        }
                    }
                    else if (_improviserRangeType == RangeType.ranged)
                    {
                        if (winner == lead)
                        {
                            // 先手近戰攻擊，後手遠程迎擊，先手攻擊勝利。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else if (winner == improviser)
                        {
                            // 先手近戰攻擊，後手遠程迎擊，後手迎擊勝利。
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                        }
                        else
                        {
                            // 先手近戰攻擊，後手遠程迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true );
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
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                        }
                        else if (winner == improviser)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，後手迎擊勝利。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else
                        {
                            // 先手遠程攻擊，後手近戰迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true );
                        }
                    }
                    else if (_improviserRangeType == RangeType.ranged)
                    {
                        if (winner == lead)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，先手攻擊勝利。
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true );
                        }
                        else if (winner == improviser)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，後手迎擊勝利。
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true );
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
                    CompareCharacterSkillAttributes( ActionType.Defend, lead, improviser, out winner, out loser );
                }
                else if (_improviserSubskillData.IsEvadingSkill)
                {
                    // 判定迴避成敗。
                    CompareCharacterSkillAttributes( ActionType.Evade, lead, improviser, out winner, out loser );
                }

                ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: winner == lead, hasStatePointDamage: true, hasStressValueDamage: true );

                if (winner == lead)
                {
                    lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                    improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );
                }
                else if (winner == improviser)
                {
                    lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Deuce );

                    if (_improviserSubskillData.IsDefendingSkill)
                    {
                        improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.SuccessfulDefender );
                    }
                    else if (_improviserSubskillData.IsEvadingSkill)
                    {
                        improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.SuccessfulEvader );
                    }
                }
            }
        }
        else
        {
            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true );

            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );
        }

        // 判定輕重受擊方。
        if (winner != null && loser != null)
        {
            if (winner.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Assaulter)
            {
                winner.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.HeavyAssaulter );
                loser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.HeavyRecipient );
            }
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
            Subskill _targetSubskillData = _casterSkill.GetCharacterSubskillData().GetSubskillData();
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
        battleResultData.AddGameCharacterResultData( gameCharacter: caster, statePointCost: _statePointCost );

        float _maxStatePointUp = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetMaxStatePointUp( _casterSubskillData ) );
        battleResultData.AddGameCharacterResultData( gameCharacter: caster, maximumStatePointIncrease: _maxStatePointUp );

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

    private static void ExecuteCasterSkillOnHit( ref BattleResultData battleResultData, GameCharacter caster, GameCharacter target,
                                                 bool hasHealthPointDamage = false, bool hasStatePointDamage = false, bool hasStressValueDamage = false, bool isBreakStatusAvailable = true,
                                                 float stressValueDamageMultiplier = 1.0f )
    {
        CharacterSkill _casterSkill = caster.GetCurrentSkill();
        Subskill _casterSubskillData = _casterSkill.GetCharacterSubskillData().GetSubskillData();
        CharacterSkill _targetSkill = target.GetCurrentSkill();
        Subskill _targetSubskillData = _targetSkill?.GetCharacterSubskillData().GetSubskillData();

        int _energyMarkerAtl = _casterSubskillData.EnergyMarkerATL;
        if (_energyMarkerAtl > 0)
        {
            // 更新“能量殘響”。
            battleResultData.AddGameCharacterResultData( target, hasEnergyMarker: true, energyMarkerRemainingATLs: _energyMarkerAtl );
        }

        if (hasHealthPointDamage)
        {
            float _healthPointDamage = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetCurrentAttackDamage( _casterSkill, caster, target ) );
            if (_healthPointDamage > 0)
            {
                bool _isActualDamage = false;

                // When the target takes damage, if the target is not using the Repulse skill
                // and Defending skill, the target will take the actual damage.
                if (_targetSkill != null)
                {
                    if (_targetSkill.GetSkillData().skillType != Skill.SkillType.repulse)
                    {
                        if (_targetSubskillData != null)
                        {
                            if (!_targetSubskillData.IsDefendingSkill)
                            {
                                _isActualDamage = true;
                            }
                        }
                    }
                }
                else
                {
                    _isActualDamage = true;
                }

                if (_isActualDamage)
                {
                    battleResultData.AddGameCharacterResultData( gameCharacter: target, actualHealthPointDamage: _healthPointDamage );
                }
                else
                {
                    battleResultData.AddGameCharacterResultData( gameCharacter: target, virtualHealthPointDamage: _healthPointDamage );
                }
            }
        }

        if (hasStatePointDamage)
        {
            float _statePointDamage = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetStatePointDamage( _casterSubskillData ) * ( ( target.HasEnergyMarker() ) ? _casterSubskillData.EnergyMarkerStateDamageRate : 1.0f ) );
            battleResultData.AddGameCharacterResultData( gameCharacter: target, statePointDamage: _statePointDamage, isBreakStatusAvailable: isBreakStatusAvailable );
        }

        if (hasStressValueDamage)
        {
            float _stressValueDamage = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetStressValueDamage( _casterSubskillData ) * ( ( target.HasEnergyMarker() ) ? _casterSubskillData.EnergyMarkerStressDamageRate : 1.0f ) * stressValueDamageMultiplier );
            battleResultData.AddGameCharacterResultData( gameCharacter: target, stressValueDamage: _stressValueDamage, isBreakStatusAvailable: isBreakStatusAvailable );
        }

        if (_targetSkill != null)
        {
            if (_casterSkill.GetSkillData().skillType == Skill.SkillType.repulse
                || _targetSkill.GetSkillData().skillType == Skill.SkillType.repulse)
            {
                if (_casterSubskillData.Range == Subskill.RangeType.melee)
                {
                    if (_targetSubskillData != null)
                    {
                        if (_targetSubskillData.Range == Subskill.RangeType.ranged)
                        {
                            float _maxStatePointUp = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetMaxStatePointUp( _casterSubskillData ) );
                            battleResultData.AddGameCharacterResultData( gameCharacter: caster, maximumStatePointIncrease: _maxStatePointUp );
                        }
                    }
                }
            }
        }

        BattleResultData_GameCharacter _targetResultData = battleResultData.GetGameCharacterResultData( target );
        if (target.GetIsInBreakStatus() || _targetResultData.isEnteringIntoBreakStatus)
        {
            if (hasHealthPointDamage)
            {
                // 尚未回復的虛傷部分全數轉化為實傷。
                _targetResultData.virtualHealthPoint = _targetResultData.currentHealthPoint;
            }
        }

        if (_casterSubskillData.WillRemoveEnergyMarker)
        {
            // 消去“能量殘響”。
            battleResultData.AddGameCharacterResultData( gameCharacter: target, hasEnergyMarker: false );
        }
    }

    private static void CompareCharacterSkillAttributes( ActionType actionType, GameCharacter lead, GameCharacter improviser,
                                                         out GameCharacter winner, out GameCharacter loser )
    {
        winner = null;
        loser = null;

        int _leadSkillStatIncrement = lead.GetCurrentSkillStatIncrement();
        Subskill _leadSubskillData = lead.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        int _leadSkillSpeed = _leadSubskillData.Speed + _leadSkillStatIncrement;
        int _leadSkillStrength = _leadSubskillData.Strength + _leadSkillStatIncrement;
        int _leadSkillEffectType = ( int )_leadSubskillData.EffectType;

        int _improviserSkillStatIncrement = improviser.GetCurrentSkillStatIncrement();
        Subskill _improviserSubskillData = improviser.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        int _improviserSkillSpeed = _improviserSubskillData.Speed + _improviserSkillStatIncrement;
        int _improviserSkillStrength = _improviserSubskillData.Strength + _improviserSkillStatIncrement;
        int _improviserSkillEffectType = ( int )_improviserSubskillData.EffectType;

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
}
