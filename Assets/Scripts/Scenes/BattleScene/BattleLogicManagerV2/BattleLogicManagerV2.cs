using System.Collections.Generic;
using UnityEngine;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;
using Skill = DatabaseManager.Skill;
using SkillType = DatabaseManager.Skill.SkillType;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using BattleResultData_GameCharacter = BattleResultData.BattleResultData_GameCharacter;

public partial class BattleLogicManagerV2
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
            return _temporaryBattleResultData.IsInBreakStatus();
        }

        return gameCharacter.IsInBreakStatus();
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

    public static GameCharacter GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType characterIdentityType, GameCharacter[] gameCharacters )
    {
        for (int i = 0; i < gameCharacters.Length; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];
            if (_gameCharacter.HasCharacterIdentityType( characterIdentityType ))
            {
                return _gameCharacter;
            }
        }

        return null;
    }

    public static GameCharacter GetGameCharacterThatMatchesOneOfCharacterIdentityTypes( CharacterIdentityType[] characterIdentityTypes, GameCharacter[] gameCharacters )
    {
        for (int i = 0; i < characterIdentityTypes.Length; i++)
        {
            GameCharacter _gameCharacter = GetGameCharacterThatMatchesCharacterIdentityType( characterIdentityTypes[ i ], gameCharacters );
            if (_gameCharacter != null)
            {
                return _gameCharacter;
            }
        }

        return null;
    }

    // 頁面：狀態更新
    private static void UpdateGameCharacterStatus( ref BattleResultData battleResultData, GameCharacter gameCharacter )
    {
        // 是否處於負荷崩潰狀態或以太崩潰狀態?
        // YES
        if (gameCharacter.IsInBreakStatus())
        {
            // 負荷崩潰狀態
            int _stressBreakStatusRemainingATLs = gameCharacter.GetStressBreakStatusRemainingATLs();
            float _currentStressValue = gameCharacter.GetCurrentStressValue();
            if (_stressBreakStatusRemainingATLs > 0)
            {
                // 負荷崩潰狀態維持值減1
                _stressBreakStatusRemainingATLs--;

                // 負荷崩潰狀態維持值是否為0?
                if (_stressBreakStatusRemainingATLs <= 0)
                {
                    // 解除負荷崩潰狀態
                    // 負荷值回復至0%
                    _currentStressValue = 0.0f;
                }
            }

            // 以太崩潰狀態
            int _stateBreakStatusRemainingATLs = gameCharacter.GetStateBreakStatusRemainingATLs();
            float _maximumStatePoint = gameCharacter.GetMaximumStatePoint();
            float _currentStatePoint = gameCharacter.GetCurrentStatePoint();
            if (_stateBreakStatusRemainingATLs > 0)
            {
                // 以太崩潰狀態維持值減1
                _stateBreakStatusRemainingATLs--;

                // 以太崩潰狀態維持值是否為0?
                // YES
                if (_stateBreakStatusRemainingATLs <= 0)
                {
                    // 解除以太崩潰狀態

                    float _originalStatePoint = gameCharacter.GetOriginalStatePoint();

                    // 最大以太值是否<起始最大以太值?
                    // YES
                    if (_maximumStatePoint < _originalStatePoint)
                    {
                        // 最大以太值回復至起始最大以太值
                        _maximumStatePoint = _originalStatePoint;
                    }

                    float _maximumStatePointThreshold = _maximumStatePoint * 0.5f;

                    // 當前以太值是否少於最大以太值的50%?
                    // YES
                    if (_currentStatePoint < _maximumStatePointThreshold)
                    {
                        _currentStatePoint = _maximumStatePointThreshold;
                    }
                    // NO
                    else
                    {
                        // 當前以太值回復至崩潰前一刻的數值
                        // 正常情況下，以太崩潰前提一定是低於0的。
                        // 所以正常情況下一定是回復至50%的。
                        // 但為了今後可能會創作有一些技能的特殊效果是可以在以太在正常值下，強制進入崩潰。
                        // 例如尚擁有120的情況下，被特殊效果進入以太崩潰。
                        // 當崩潰回復時，以太值會返回崩潰前的120。

                        _currentStatePoint = gameCharacter.GetStatePointBeforeBreakStatus();
                    }
                }
            }

            battleResultData.AddGameCharacterResultData( gameCharacter,
                stateBreakStatusRemainingATLs: _stateBreakStatusRemainingATLs, maximumStatePoint: _maximumStatePoint, currentStatePoint: _currentStatePoint,
                stressBreakStatusRemainingATLs: _stressBreakStatusRemainingATLs, currentStressValue: _currentStressValue );

            // "己方"當前流向是否"負荷流"?
            // YES
            if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.Stress)
            {
                // 負荷流借力結算
                CategorizedPassiveSkillManager.StressPassiveSkillTypeJieLiCalculation( ref battleResultData, gameCharacter );
            }
        }
    }

    // 頁面：技能持續效果更新
    private static void UpdateSkillContinuousEffects( ref BattleResultData battleResultData, GameCharacter gameCharacter )
    {
        BattleResultData_GameCharacter _gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData( gameCharacter );

        if (_gameCharacter_BattleResultData.HasEnergyMarker())
        {
            // 技能持續效果維持值減1 (例如:能量殘響)
            // 技能持續效果的維持值為0時解除該技能效果
            battleResultData.AddGameCharacterResultData_ModifyEnergyMarkerATL( gameCharacter, -1, out _ );
        }
    }

    // 頁面：判定先後手方
    public static ( GameCharacter lead, GameCharacter improviser ) DetermineLeadAndImproviser( BattleGameManager battleGameManager, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo, out List<string> resultLogList )
    {
        resultLogList = new List<string>();

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

            resultLogList.Add( _logOne );
        }

        if (_isGameCharacterTwoUsingAttackingSkill)
        {
            _gameCharacterTwo_Skill_SubskillData = _gameCharacterTwo_Skill.GetCharacterSubskillData().GetSubskillData();

            string _logTwo = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>按下了的技能是"
                           + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterTwo_Skill_SubskillData.DisplayName }</color>"
                           + $" （{ TerminologyManager.GetSkillInformationText( _gameCharacterTwo_Skill ) }）。";

            resultLogList.Add( _logTwo );
        }

        // 如果雙方都有按下主動技能或反擊技能：
        if (_isGameCharacterOneUsingAttackingSkill && _isGameCharacterTwoUsingAttackingSkill)
        {
            resultLogList.Add( "雙方都有按下主動技能或反擊技能。" );

            int _gameCharacterOne_Skill_Speed = _gameCharacterOne_Skill_SubskillData.Speed + gameCharacterOne.GetCurrentSkillStatIncrement();
            int _gameCharacterTwo_Skill_Speed = _gameCharacterTwo_Skill_SubskillData.Speed + gameCharacterTwo.GetCurrentSkillStatIncrement();

            // 當前的距離是否為“遠距離”？
            // YES
            if (battleGameManager.GetBattleDistanceManager().GetCurrentDistanceType() == BattleDistanceManager.DistanceType.Far)
            {
                // 近戰技能的速度在[判定先後手方] 階段時,會降低一級

                if (_gameCharacterOne_Skill_SubskillData.Range == RangeType.melee && _gameCharacterOne_Skill_Speed > 1)
                {
                    _gameCharacterOne_Skill_Speed -= 1;
                }

                if (_gameCharacterTwo_Skill_SubskillData.Range == RangeType.melee && _gameCharacterTwo_Skill_Speed > 1)
                {
                    _gameCharacterTwo_Skill_Speed -= 1;
                }
            }

            resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>按下了的技能的速度是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetSpeedLevelText( _gameCharacterOne_Skill_Speed ) }</color>。" );
            resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>按下了的技能的速度是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetSpeedLevelText( _gameCharacterTwo_Skill_Speed ) }</color>。" );

            // 對比雙方技能的速度。
            if (_gameCharacterOne_Skill_Speed > _gameCharacterTwo_Skill_Speed)
            {
                gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Lead );
                gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Improviser );
            }
            else if (_gameCharacterOne_Skill_Speed < _gameCharacterTwo_Skill_Speed)
            {
                gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Improviser );
                gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Lead );
            }
            // 如果雙方的技能速度相同：
            else
            {
                resultLogList.Add( "雙方按下了的技能速度相同。" );

                bool _isGameCharacterOneCounterAttacking = gameCharacterOne.GetIsCounterAttacking();
                bool _isGameCharacterTwoCounterAttacking = gameCharacterTwo.GetIsCounterAttacking();

                // 如果有一方在【 反擊指令時間 】或【 近戰反擊指令時間 】階段按下主動技能或反擊技能：
                // 按下的一方得到先手方而另一方得到後手方。
                if (_isGameCharacterOneCounterAttacking || _isGameCharacterTwoCounterAttacking)
                {
                    if (_isGameCharacterOneCounterAttacking)
                    {
                        resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>在【 反擊指令時間 】或【 近戰反擊指令時間 】階段按下主動技能或反擊技能。" );

                        gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Lead );
                        gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Improviser );
                    }

                    if (_isGameCharacterTwoCounterAttacking)
                    {
                        resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>在【 反擊指令時間 】或【 近戰反擊指令時間 】階段按下主動技能或反擊技能。" );

                        gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Improviser );
                        gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Lead );
                    }
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

                    resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>的當前以太值（扣除技能以太值消耗後）是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterOne_StatePoint }</color>。" );
                    resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>的當前以太值（扣除技能以太值消耗後）是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterTwo_StatePoint }</color>。" );

                    // 對比雙方的當前以太值（扣除了按下的技能的以太值消耗後的數值）：
                    if (_gameCharacterOne_StatePoint > _gameCharacterTwo_StatePoint)
                    {
                        gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Lead );
                        gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Improviser );
                    }
                    else if (_gameCharacterOne_StatePoint < _gameCharacterTwo_StatePoint)
                    {
                        gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Improviser );
                        gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Lead );
                    }
                    // 如果雙方的當前以太值（扣除了按下的技能的以太值消耗後的數值）也是相同:
                    else
                    {
                        resultLogList.Add( "雙方的當前以太值（扣除技能以太值消耗後）是相同。因此，隨機決定先手方。" );

                        if (Random.value < 0.5f)
                        {
                            gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Lead );
                            gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Improviser );
                        }
                        else
                        {
                            gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Improviser );
                            gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Lead );
                        }
                    }
                }
            }
        }
        // 如果只有一方按下主動技能或反擊技能：
        else if (_isGameCharacterOneUsingAttackingSkill || _isGameCharacterTwoUsingAttackingSkill)
        {
            if (_isGameCharacterOneUsingAttackingSkill)
            {
                resultLogList.Add( $"只有一方（<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>）按下主動技能或反擊技能。" );

                gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Lead );
                gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Improviser );
            }

            if (_isGameCharacterTwoUsingAttackingSkill)
            {
                resultLogList.Add( $"只有一方（<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>）按下主動技能或反擊技能。" );

                gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.Improviser );
                gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.Lead );
            }
        }
        // 否則：
        else
        {
            resultLogList.Add( "雙方都沒有按下主動技能或反擊技能。" );
        }

        if (gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.Lead )
            && gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
        {
            return ( lead: gameCharacterOne, improviser: gameCharacterTwo );
        }

        if (gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.Improviser )
            && gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.Lead ))
        {
            return ( lead: gameCharacterTwo, improviser: gameCharacterOne );
        }

        return ( lead: null, improviser: null );
    }

    // 發動流向效果A
    public static void TriggerCategorizedPassiveSkillEffectA( ref BattleResultData battleResultData, GameCharacter gameCharacter )
    {
        // 開始檢查流向效果
        CategorizedPassiveSkillManager.CategoryType _selectedPassiveSkillCategoryType = gameCharacter.GetSelectedPassiveSkillCategoryType();
        CategorizedPassiveSkillManager.CategoryType _lastSelectedPassiveSkillCategoryType = gameCharacter.GetLastSelectedPassiveSkillCategoryType();

        // "己方"是否從"生命流"》"以太流"/"負荷流"?
        // YES
        if (_lastSelectedPassiveSkillCategoryType == CategorizedPassiveSkillManager.CategoryType.Life
            && _selectedPassiveSkillCategoryType is CategorizedPassiveSkillManager.CategoryType.State or CategorizedPassiveSkillManager.CategoryType.Stress)
        {
            CategorizedPassiveSkillManager.RunCyclePointConvert( ref battleResultData, gameCharacter );
        }
        // NO
        // "己方"的當前流向是否"生命流"?
        // YES
        else if (_selectedPassiveSkillCategoryType == CategorizedPassiveSkillManager.CategoryType.Life)
        {
            CategorizedPassiveSkillManager.CalculateLifeCategoryNiJingLiuZhuanScore( ref battleResultData, gameCharacter );
            CategorizedPassiveSkillManager.CalculateLifeCategoryNiJingLiuZhuanScore(ref battleResultData, gameCharacter);
        }
    }

    public static void DeclareAssaulterAndRecipient( GameCharacter assaulter, GameCharacter recipient )
    {
        assaulter.AddCharacterIdentityType( CharacterIdentityType.Assaulter );
        recipient.AddCharacterIdentityType( CharacterIdentityType.Recipient );

        DetermineLightOrHeavyRecipient( recipient: recipient, assaulter: assaulter );
    }

    public static void DetermineLightOrHeavyRecipient( GameCharacter recipient, GameCharacter assaulter )
    {
        // Temporarily determine that the recipient is a heavy recipient.
        recipient.AddCharacterIdentityType( CharacterIdentityType.HeavyRecipient );
        assaulter.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
    }

    // 頁面：Part A結算
    public static void DetermineResultForPartA( ref BattleResultData battleResultData, GameCharacter lead, GameCharacter improviser )
    {
        // 先手方使用技能時當前以太值結算
        CategorizedPassiveSkillManager.CalculateLeadCurrentStatePoint( ref battleResultData, lead, improviser );

        // 先手方使用技能時最大以太值結算
        CategorizedPassiveSkillManager.CalculateMaximumStatePoint( ref battleResultData, lead );

        // "先手方"的當前流向是否"以太流"?
        // YES
        if (lead.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.State)
        {
            // 先手方使用技能時當前以太值第2次結算
            CategorizedPassiveSkillManager.RunCurrentStatePointSecondTimeCalculation( ref battleResultData, lead );
        }
    }

    // 頁面：判定 Part B 結果及結算
    public static void DetermineResultForPartB( out BattleResultData battleResultData, GameCharacter lead, GameCharacter improviser, bool hasPartA )
    {
        battleResultData = new BattleResultData();

        // 更新"先手方"當前流向
        // 頁面：發動流向效果A
        lead.TriggerEvent( BattleAnimationManager.AnimationEvent.OnCategorizedPassiveTypeUpdated );
        BattleLogicManagerV2.TriggerCategorizedPassiveSkillEffectA( ref battleResultData, lead );

        // 更新"後手方"當前流向
        // 頁面：發動流向效果A
        improviser.TriggerEvent( BattleAnimationManager.AnimationEvent.OnCategorizedPassiveTypeUpdated );
        BattleLogicManagerV2.TriggerCategorizedPassiveSkillEffectA( ref battleResultData, improviser );

        // 從“判定先後手方”頁面來到這裡。
        if (!hasPartA)
        {
            DetermineResultForPartA( ref battleResultData, lead, improviser );
        }

        CharacterSkill _leadCurrentSkill = lead.GetCurrentSkill();
        CharacterSkill _improviserCurrentSkill = improviser.GetCurrentSkill();

        SkillType _improviserCurrentSkillType = ( _improviserCurrentSkill != null ) ? _improviserCurrentSkill.GetSkillData().skillType : SkillType.none;
        switch ( _improviserCurrentSkillType )
        {
            // 後手方已按下的技能是否迎擊技能？
            // YES
            case SkillType.repulse:

                // 進入“判定迎擊結果及結算”頁面。
                BattleLogicManagerV2.DetermineResultForRepulse( ref battleResultData, lead, improviser );

                break;

            case SkillType.backend:

                Subskill _improviserSubskillData = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData();

                if (_improviserSubskillData.IsDefendingSkill || _improviserSubskillData.IsEvadingSkill)
                {
                    // 後手方已按下的技能是否防禦技能？
                    // YES
                    if (_improviserSubskillData.IsDefendingSkill)
                    {
                        // 進入“判定防禦成敗及結算”頁面。
                        BattleLogicManagerV2.DetermineResultForDefense( ref battleResultData, lead, improviser );
                    }
                    // 後手方已按下的技能是否回避技能？
                    // YES
                    else if (_improviserSubskillData.IsEvadingSkill)
                    {
                        // 進入“判定回避成敗及結算”頁面。
                        BattleLogicManagerV2.DetermineResultForEvasion( ref battleResultData, lead, improviser );
                    }
                }

                break;

            default:

                // 後手方得到"受擊方"&“重受擊方”&"未能抵抗方"
                // 先手方得到"直擊方"&“重直擊方”。
                improviser.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.HeavyRecipient, CharacterIdentityType.NonResister } );
                lead.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Assaulter, CharacterIdentityType.HeavyAssaulter } );

                // 重受擊方：improviser
                // 重直擊方：lead

                // 重受擊方當前以太值結算
                CategorizedPassiveSkillManager.CalculateHeavyRecipientStatePoint( ref battleResultData, lead, improviser);

                BattleResultData.BattleResultData_GameCharacter _recipient_BattleResultData = battleResultData.GetGameCharacterResultData( improviser );

                // "重受擊方"有沒有因"重直擊方"的以太傷害導致當前以太值<0?
                // YES
                if (_recipient_BattleResultData.currentStatePoint < 0.0f)
                {
                    // "重受擊方"成為以太崩潰狀態,該維持值為1
                    // "重受擊方"得到"以太崩潰方"
                    battleResultData.AddGameCharacterResultData_StateBreakStatus( improviser, 1, out _ );
                    improviser.AddCharacterIdentityType( CharacterIdentityType.StateBreakStatusHolder );
                }

                // 重受擊方生命值結算
                CategorizedPassiveSkillManager.CalculateLightAndHeavyRecipientHealthResult( ref battleResultData, lead, improviser );

                // "雙方"的技能持續效果更新(例如:能量殘響-對方"能量殘響"維持值更新為5)
                BattleLogicManagerV2.UpdateSkillContinuousEffects( ref battleResultData, lead );
                BattleLogicManagerV2.UpdateSkillContinuousEffects( ref battleResultData, improviser );

                // 頁面：發動流向效果B
                CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref battleResultData, lead, improviser);
                CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref battleResultData, improviser, lead);

                break;
        }
    }

    public static BattleResultData DetermineResultForPartB( GameCharacter lead, GameCharacter improviser, out GameCharacter winner, out GameCharacter loser )
    {
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

            improviser.SetCurrentSkillRangeType( _improviserRangeType );

            if (_improviserSkillData.skillType == Skill.SkillType.repulse)
            {
                float _stressValueDamageMultiplierOnRepulseForLoser = GameConfiguration.Instance.GetBattleConfiguration().GetStressValueDamageMultiplierOnRepulseForLoser();

                // 判定迎擊中途結果。
                CompareCharacterSkillAttributes( ref _battleResultData, ActionType.Repulse, lead, improviser, out winner, out loser );

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

                _battleResultData.AddResultLog( _repulseResultLog );

                // ----------------------------------------------------------------------------------------------------

                if (winner == null)
                {
                    // 迎擊結果為雙方打平，雙方得到"平手方"。
                    lead.AddCharacterIdentityType( CharacterIdentityType.Deuce );
                    improviser.AddCharacterIdentityType( CharacterIdentityType.Deuce );

                    // 進入“迎擊平手方結算”。
                    SettleRepulseResultForDraw( ref _battleResultData, lead, improviser );
                }

                if (_leadRangeType == RangeType.melee)
                {
                    if (_improviserRangeType == RangeType.melee)
                    {
                        if (winner == lead)
                        {
                            // 先手近戰攻擊，後手近戰迎擊，先手攻擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true);
                        }
                        else if (winner == improviser)
                        {
                            // 先手近戰攻擊，後手近戰迎擊，後手迎擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: improviser, recipient: lead );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true);
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
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true);
                        }
                        else if (winner == improviser)
                        {
                            // 先手近戰攻擊，後手遠程迎擊，後手迎擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: improviser, recipient: lead );
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
                            DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                        }
                        else if (winner == improviser)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，後手迎擊勝利。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true);
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
                            DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true );
                        }
                        else if (winner == improviser)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，後手迎擊勝利。
                            DeclareAssaulterAndRecipient( assaulter: improviser, recipient: lead );
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
                    CompareCharacterSkillAttributes( ref _battleResultData, ActionType.Defend, lead, improviser, out winner, out loser );
                }
                else if (_improviserSubskillData.IsEvadingSkill)
                {
                    // 判定迴避成敗。
                    CompareCharacterSkillAttributes( ref _battleResultData, ActionType.Evade, lead, improviser, out winner, out loser );
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

                _battleResultData.AddResultLog( _resultLog );

                // ----------------------------------------------------------------------------------------------------

                if (winner == lead)
                {
                    DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
                }
                else if (winner == improviser)
                {
                    lead.AddCharacterIdentityType( CharacterIdentityType.Deuce );
                }

                ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: winner == lead, hasStatePointDamage: true, hasStressValueDamage: true );
            }
        }
        else
        {
            // 後手方得到"受擊方"&“重受擊方”&"未能抵抗方"先手方得到 “重直擊方”。

            DeclareAssaulterAndRecipient( assaulter: lead, recipient: improviser );
            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true );

            winner = lead;
            loser = improviser;

            // 重受擊方當前以太值結算

            // 重受擊方生命值結算
            CategorizedPassiveSkillManager.CalculateLightAndHeavyRecipientHealthResult( ref _battleResultData, lead, improviser );
        }

        // "己方"是否有給予"對方"HP傷害 / 受到"對方"HP傷害?

        BattleResultData_GameCharacter _lead_BattleResultData = lead.GetTemporaryBattleResultData();
        if (_lead_BattleResultData != null)
        {
            if (_lead_BattleResultData.actualHealthPointDamageDealt > 0
                || _lead_BattleResultData.virtualHealthPointDamageDealt > 0)
            {
                CategorizedPassiveSkillManager.CalculateLifeScoreEffect( ref _battleResultData, lead, true );
            }
            else if (_lead_BattleResultData.actualHealthPointDamageTaken > 0
                || _lead_BattleResultData.virtualHealthPointDamageTaken > 0)
            {
                CategorizedPassiveSkillManager.CalculateLifeScoreEffect( ref _battleResultData, lead, false );
            }
        }

        BattleResultData_GameCharacter _improviser_BattleResultData = improviser.GetTemporaryBattleResultData();
        if (_improviser_BattleResultData != null)
        {
            if (_improviser_BattleResultData.actualHealthPointDamageDealt > 0
                || _improviser_BattleResultData.virtualHealthPointDamageDealt > 0)
            {
                CategorizedPassiveSkillManager.CalculateLifeScoreEffect( ref _battleResultData, improviser, true );
            }
            else if (_improviser_BattleResultData.actualHealthPointDamageTaken > 0
                || _improviser_BattleResultData.virtualHealthPointDamageTaken > 0)
            {
                CategorizedPassiveSkillManager.CalculateLifeScoreEffect( ref _battleResultData, improviser, false );
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
        battleResultData.AddGameCharacterResultData_MaximumStatePointIncreaseForBase( caster, _maxStatePointUp, out _ );

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
                                                 bool hasHealthPointDamage = false, bool hasStatePointDamage = false, bool hasStressValueDamage = false,
                                                 float stressValueDamageMultiplier = 1.0f )
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
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncreaseForBonus( caster, _maxStatePointUp, out _casterBattleResultData );

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
            if (_statePointDamage > 0)
            {
                battleResultData.AddGameCharacterResultData_StatePointDamage( target, _statePointDamage, out _targetBattleResultData );
                _extraLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _statePointDamage }{ TerminologyManager.STATE_POINT }傷害</color>{ ( ( _isTargetHavingEnergyMarker ) ? "（帶有能量殘響）" : "" ) }";
            }
        }

        // 負荷傷害
        if (hasStressValueDamage)
        {
            _isTargetHavingEnergyMarker = ( _targetBattleResultData != null ) ? _targetBattleResultData.HasEnergyMarker() : target.HasEnergyMarker();

            float _stressValueDamage = BattleCalculationManager.AdjustAmount( BattleCalculationManager.GetStressValueDamage( _casterSubskillData ) * ( ( _isTargetHavingEnergyMarker ) ? _casterSubskillData.EnergyMarkerStressDamageRate : 1.0f ) * stressValueDamageMultiplier );
            if (_stressValueDamage > 0)
            {
                battleResultData.AddGameCharacterResultData_StressValueDamage( target, _stressValueDamage, out _targetBattleResultData );
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
                // 參考 Flowchart 裡的“虛傷實傷解說”。
                // 當角色受到直擊傷害時：
                // 如果該角色是“輕受擊方”，該直擊傷害為“虛傷”。
                // 如果該角色是“重受擊方”，該直擊傷害為“實傷”。

                bool _isActualDamage = target.HasCharacterIdentityType( CharacterIdentityType.HeavyRecipient );

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
            battleResultData.AddResultLog( _resultLog );
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

    private static void CompareCharacterSkillAttributes( ref BattleResultData battleResultData,
                                                         ActionType actionType, GameCharacter lead, GameCharacter improviser,
                                                         out GameCharacter winner, out GameCharacter loser )
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

            battleResultData.AddResultLog( _resultLog );
        }

        if (_improviserSkillStatIncrement > 0)
        {
            _resultLog = $"因為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ improviser.GetCharacterName() }</color>已看破<color={ BattleLog.KEYWORD_COLOR_CODE }>{ lead.GetCharacterName() }</color>使用的"
                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _leadSubskillData.DisplayName }</color>，所以<color={ BattleLog.KEYWORD_COLOR_CODE }>{ improviser.GetCharacterName() }</color>使用的"
                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _improviserSubskillData.DisplayName }</color>的強度和速度得到<color={ BattleLog.KEYWORD_COLOR_CODE }>+{ _improviserSkillStatIncrement }</color>。";

            battleResultData.AddResultLog( _resultLog );
        }

        switch ( actionType )
        {
            // 頁面：判定迎擊結果及結算
            case ActionType.Repulse:

                // Case B: 先手方的攻擊速度 > 後手方的迎擊速度。
                if (_leadSkillSpeed > _improviserSkillSpeed)
                {
                    winner = lead;
                }
                // Case A: 先手方的攻擊速度 <= 後手方的迎擊速度。
                else
                {
                    // TODO:
                    /*
                    強度較高一方得到"強度勝方"&
                    "勝利優惠機制方"
                    強度較低一方得到"強度負方"
                    */

                    if (_leadSkillStrength > _improviserSkillStrength)
                    {
                        winner = lead;
                    }
                    else if (_leadSkillStrength < _improviserSkillStrength)
                    {
                        winner = improviser;
                    }
                }

                // 如果沒有winner，就是雙方的強度相同。

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
            _gameCharacter.TriggerEvent( BattleAnimationManager.AnimationEvent.OnTransition );
            UpdateGameCharacterStatus( ref _battleResultData, _gameCharacter );
            UpdateSkillContinuousEffects( ref _battleResultData, _gameCharacter );

            // TODO: 頁面：形態切換
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
                                      + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observedSkillRecord.GetSubskillData().DisplayName }</color>的<color={ BattleLog.KEYWORD_COLOR_CODE }>看破儲蓄值</color>增加了"
                                      + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observationRate.ConvertToIntegerInPercentage() }%</color>後變成<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observedSkillRecord.GetCurrentObservedRate().ConvertToIntegerInPercentage() }%</color>。";

                    resultLogList.Add( _resultLog );
                }
            }

            // 取消所有身份。
            _gameCharacter.ClearCharacterIdentityTypeList();

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
            GameCharacter _gameCharacter = gameCharacters[ i ];
            _gameCharacter.SetLastAtlSkill( _gameCharacter.GetCurrentSkill() );
            _gameCharacter.TriggerEvent( BattleAnimationManager.AnimationEvent.OnAtlEnded );
        }
    }

    public static BattleResultData OnTheEndOfRound( GameCharacter[] gameCharacters )
    {
        BattleResultData _battleResultData = new();

        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();
        int _stressValueDecreaseOnRoundStart = _battleConfiguration.GetStressValueDecreaseOnRoundStart();
        float _healthPointRegenerationRateOnRoundStart = _battleConfiguration.GetHealthPointRegenerationRateOnRoundStart();
        int _maximumStatePointIncreaseOnRoundStart = _battleConfiguration.GetMaximumStatePointIncreaseOnRoundStart();

        for (int i = 0; i < gameCharacters.Length; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];

            /*
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
                .AddGameCharacterResultData_RestoreCurrentStatePoint( _gameCharacter, 1.0f, out _ );

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
            */

            CategorizedPassiveSkillManager.RunBasicRecoveryEffects( ref _battleResultData, _gameCharacter );

            string _resultLog = "";

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
                                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _subskillData.DisplayName }</color>的<color={ BattleLog.KEYWORD_COLOR_CODE }>看破儲蓄值</color>減少了"
                                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observationRateDeductionPerRound.ConvertToIntegerInPercentage() }%</color>後變成<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _observedSkillRecord.GetCurrentObservedRate().ConvertToIntegerInPercentage() }%</color>";

                            if (_observedSkillRecord.GetSubskillData() == null)
                            {
                                _skill.ResetObservedSkillRecord();
                                _resultLog += "和該記錄被刪除了";
                            }

                            _resultLog += "。";

                            _battleResultData.AddResultLog( _resultLog );
                        }
                    }
                }
            }
        }

        return _battleResultData;
    }

    public static void OnGameCharacterBeingInBreakStatus( GameCharacter gameCharacter )
    {
        if (gameCharacter.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[]
                                                          {
                                                              CharacterIdentityType.Recipient,
                                                              CharacterIdentityType.LightRecipient,
                                                              CharacterIdentityType.SuccessfulResister
                                                          }
                                                          ))
        {
            gameCharacter.AddCharacterIdentityType( CharacterIdentityType.HeavyRecipient );
            gameCharacter.GetCurrentAttacker().AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
        }
    }

    // result
    // 0 = None
    // 1 = Player wins
    // 2 = Enemy wins
    public static bool HasBattleEnded( GameCharacter[] gameCharacters, out int result )
    {
        result = 0;

        bool _hasPlayerCharacterSurvived = false;
        bool _hasEnemyCharacterSurvived = false;

        for (int i = 0; i < gameCharacters.Length; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];
            if (!BattleLogicManagerV2.IsGameCharacterDead( _gameCharacter ))
            {
                if (_gameCharacter.GetIsPlayer())
                {
                    _hasPlayerCharacterSurvived = true;
                }
                else
                {
                    _hasEnemyCharacterSurvived = true;
                }
            }
        }

        if (!_hasPlayerCharacterSurvived || !_hasEnemyCharacterSurvived)
        {
            if (_hasPlayerCharacterSurvived)
            {
                // Player wins.
                result = 1;
            }
            else if (_hasEnemyCharacterSurvived)
            {
                // Enemy wins.
                result = 2;
            }

            return true;
        }

        return false;
    }

    public static bool IsGameCharacterDead( GameCharacter gameCharacter )
    {
        BattleResultData_GameCharacter _temporaryBattleResultData = gameCharacter.GetTemporaryBattleResultData();

        if (_temporaryBattleResultData != null)
        {
            return _temporaryBattleResultData.isDead;
        }

        return gameCharacter.GetIsDead();
    }
}
