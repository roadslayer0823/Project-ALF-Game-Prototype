using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillType = DatabaseManager.Skill.SkillType;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;
using CommandTimeType = GameCharacter.CommandTimeType;
using DistanceType = BattleDistanceManager.DistanceType;
using AnimationParameterData = CharacterAnimationHandler.AnimationParameterData;

public partial class BattleAnimationManager : MonoBehaviour
{
    private const float ANIMATION_DURATION_FOR_PART_A = 1.6f;   // Part-A 的演出時長
    private const float ANIMATION_DURATION_FOR_PART_B = 1.8f;   // Part-B 的演出時長
    private const float RESULT_ANIMATION_TIMING_A = 0.1f;       // 結算演出時機A：在 Part-A 或 Part-B 的第 0.1 秒起播放
    private const float RESULT_ANIMATION_TIMING_B = 0.6f;       // 結算演出時機B：在 Part-B 的第 0.6 秒起播放
    private const float RESULT_ANIMATION_TIMING_C = 1.2f;       // 結算演出時機C：在 Part-B 的第 1.2 秒起播放

    private bool isAbleToAssignSkillInPartB = false;

    public IEnumerator RunBattleAnimationV2( BattleFlowRound_V2 battleFlowRound, BattleFlowATL_V2 battleFlowATL )
    {
        //ATLSlotListPanelV2 _atlSlotListPanel = this.battleGameManager.GetBattleUiManager().GetATLSlotListPanelV2();
        ATLSlotListPanelV3 _atlSlotListPanel = this.battleGameManager.GetBattleUiManager().GetATLSlotListPanelV3();

        BattleResultData _battleResultData = null;
        BattleResultData.BattleResultData_GameCharacter _leadBattleResultData = null;
        BattleResultData.BattleResultData_GameCharacter _improviserBattleResultData = null;

        PlayerCharacter _playerCharacter = this.battleGameManager.GetPlayerCharacter();
        EnemyCharacter _enemyCharacter = this.battleGameManager.GetEnemyCharacter();
        GameCharacter[] _gameCharacters = new GameCharacter[] { _playerCharacter, _enemyCharacter };

        _battleResultData = BattleLogicManagerV2.OnTheStartOfATL( _gameCharacters );
        _playerCharacter.ApplyBattleResultData( _battleResultData.GetGameCharacterResultData( _playerCharacter ), this.battleGameManager );
        _enemyCharacter.ApplyBattleResultData( _battleResultData.GetGameCharacterResultData( _enemyCharacter ), this.battleGameManager );

        _playerCharacter.PlayPrepareAnimation();
        _enemyCharacter.PlayIdleAnimation();

        // 是否有一方在[臨戰指令時間 (後)]/[反擊指令時間]/[近戰指令時間]/[近戰反擊指令時間]輸入主動/反擊/派生技能？
        // YES
        if (BattleLogicManagerV2.ShouldCombatCommandTimeBeSkipped( _playerCharacter, _enemyCharacter ))
        {
            _atlSlotListPanel.GoToATL( battleFlowATL.GetATLNumber(), 0.1f );
        }
        // NO
        else
        {
            // 當前距離重置為“中距離”
            this.battleGameManager.GetBattleDistanceManager().SetCurrentDistanceType( DistanceType.Normal );

            // 進入“臨戰指令時間（前）”頁面。
            // ------------------------------ 臨戰指令時間 (前) ------------------------------

            LeanTween.delayedCall( 0.1f, () =>
            {
                this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COMBAT_COMMAND_TIME, true );
                //this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COMBAT_COMMAND_TIME, false );
                BattleLog.Instance.AddOnScreenBattleLog( $"雙方進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.COMBAT_COMMAND_TIME } 】</color>。" );
            } );

            if (!battleGameManager.GetBattleVisualEffectManager().IsShowingCombatCommandCutScreen())
            {
                bool _isPlayingCombatCommandAnimation = true;
                battleGameManager.GetBattleVisualEffectManager().TriggerCombatCommandCutIn( () => { _isPlayingCombatCommandAnimation = false; } );
                yield return new WaitUntil( () => !_isPlayingCombatCommandAnimation );
            }

            battleGameManager.ShowPreparationView();
            _playerCharacter.TriggerEvent( AnimationEvent.OnCombatCommandTimeStarted );
            _enemyCharacter.TriggerEvent( AnimationEvent.OnCombatCommandTimeStarted );

            battleFlowATL.StartAttackOpportunityCountdownTimer( this.skillPromptPanel );
            _atlSlotListPanel.GoToATL( battleFlowATL.GetATLNumber(), battleFlowATL.GetAttackOpportunityDuration() );

            Coroutine _coroutineForCombatCommandTime = StartCoroutine( RunCheckingIfSkillIsSelectedInCommandCombatTime( _playerCharacter ) );
            yield return new WaitUntil( () => ( !battleFlowATL.GetIsDuringAttackOpportunityPeriod() || ( _playerCharacter.GetAssignedSkill() != null && _enemyCharacter.GetAssignedSkill() != null ) ) );
            StopCoroutine( _coroutineForCombatCommandTime );
            _coroutineForCombatCommandTime = null;
            battleFlowATL.StopAttackOpportunityCountdownTimer();
            yield return new WaitWhile( () => this.battleGameManager.GetBattleVisualEffectManager().IsPlayingCharacterTurningAnimation() );

            _playerCharacter.TriggerEvent( AnimationEvent.OnTransition );
            _enemyCharacter.TriggerEvent( AnimationEvent.OnTransition );

            _atlSlotListPanel.GoToMiddleAtCurrentAtlSlot( 0.1f );
            this.skillPromptPanel.HideCommandPhase( true );
            this.skillPromptPanel.HideCommandPhase( false );

            // ------------------------------------------------------------------------
        }

        // 進入“判定先后手方”頁面。
        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>判定先後手方</color>" );

        var ( _lead, _improviser ) = BattleLogicManagerV2.DetermineLeadAndImproviser( this.battleGameManager, _playerCharacter, _enemyCharacter, out List<string> _resultLogList );
        ShowBattleLog( _resultLogList );

        // 如果沒有“先手方”和“後手方”，當前 ATL 結束，直接進入下一個 ATL。
        if (_lead == null || _improviser == null)
        {
            BattleLog.Instance.AddOnScreenBattleLog( "沒有先後手方。當前 ATL 結束。" );

            _playerCharacter.SetLastAtlSkill( _playerCharacter.GetAssignedSkill() );
            _enemyCharacter.SetLastAtlSkill( _enemyCharacter.GetAssignedSkill() );

            _playerCharacter.ResetAssignedSkill();
            _enemyCharacter.ResetAssignedSkill();

            this.battleGameManager.GetBattleVisualEffectManager().TriggerCombatCommandCutOut();
            yield return new WaitWhile( () => this.battleGameManager.GetBattleVisualEffectManager().IsShowingCombatCommandCutScreen() );
            BattleLogicManagerV2.OnTheEndOfATL( _gameCharacters );

            yield break;
        }

        // “先手方”發動技能。
        _lead.ApplyAssignedSkillAsCurrentSkill();

        // 調用“判定距離中途結果”。
        battleGameManager.GetBattleDistanceManager().UpdateHalfwayDistanceResult( _lead, out _resultLogList );
        ShowBattleLog( _resultLogList );

        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>判定結果</color>為"
                                                 + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _lead.GetCharacterName() }</color>成为<color={ BattleLog.SPECIAL_COLOR_CODE }>“先手方”</color>，"
                                                 + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _improviser.GetCharacterName() }</color>成为<color={ BattleLog.SPECIAL_COLOR_CODE }>“后手方”</color>。" );

        float _animationDuration = 0.0f;
        float _animationStartTime = 0.0f;

        float _skillAnimationLength = 0.0f;
        float _skillCountdownTime = 0.0f;
        float _skillCountdownTimeStartTime = 0.0f;
        string _log = "";

        this.gameCharacterList = new List<GameCharacter>()
        {
            _lead,
            _improviser
        };

        // 先手方的技能
        CharacterSkill _leadCurrentSkill = _lead.GetCurrentSkill();
        Subskill _leadSubskillData = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData();
        RangeType _leadRangeType = _lead.GetCurrentSkillRangeType();

        // 後手方的技能
        CharacterSkill _improviserCurrentSkill = null;
        SkillType _improviserSkillType = SkillType.none;
        Subskill _improviserSubskillData = null;

        BringGameCharacterToFront( _lead );
        BringGameCharacterToBack( _improviser );

        if (_lead.GetIsPlayer())
        {
            ChangeToBackgroundPartB();
        }
        else
        {
            ChangeToBackgroundPartA();
        }

        this.battleGameManager.GetBattleVisualEffectManager().TriggerAnimationSetDarkenPartA();

        _lead.GetOwnContainer().SetActive( true );
        _lead.ShowCharacterObject();
        _lead.GetOpponentContainer().SetActive( false );

        this.battleGameManager.GetBattleVisualEffectManager().TriggerCombatCommandCutOut();
        yield return new WaitWhile( () => this.battleGameManager.GetBattleVisualEffectManager().IsShowingCombatCommandCutScreen() );

        // “先手方”發動技能。
        _lead.TriggerEvent( AnimationEvent.OnNormalSkillBeingUsed );
        _improviser.SetCurrentAttacker( _lead );

        BattleLog.Instance.AddOnScreenBattleLog(
            $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _lead.GetCharacterName() }</color>使出了"
            + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _leadSubskillData.DisplayName }</color>"
            + $" （{ TerminologyManager.GetSkillInformationText( _leadCurrentSkill ) }）。"
            );

        ShowCasterCurrentSkillInfo( _lead );

        bool _isLeadMeleeAttacking = _lead.GetIsMeleeAttacking();
        bool _isLeadCounterAttacking = _lead.GetIsCounterAttacking();

        for (int i = 0; i < _gameCharacters.Length; i++)
        {
            GameCharacter _gameCharacter = _gameCharacters[ i ];
            _gameCharacter.SetIsMeleeAttacking( false );
            _gameCharacter.SetIsCounterAttacking( false );
        }

        SkillType _leadSkillType = _lead.GetCurrentSkill().GetSkillData().skillType;

        // 是否有一方按下“派生技能”？
        // YES
        // Part B頁面："先手方"已按下技能是否"派生技能"?
        // YES
        if (_leadSkillType == SkillType.derived)
        {
            // 取消對方的任何技能指令並進入“判定 Part B 結果及結算”。
            _improviser.ResetAssignedSkill();
            _improviser.Reset();

            _lead.GetCharacterAnimationHandler().ResetAnimation();
            _improviser.GetCharacterAnimationHandler().ResetAnimation();

            yield return StartCoroutine( RunDerivedSkill( _lead, _improviser, _atlSlotListPanel, battleFlowRound.GetCurrentATL().GetATLNumber() ) );

            if (EndPartB( _lead, _improviser ))
            {
                yield break;
            }

            battleFlowRound.EndCurrentRound();
            yield return StartCoroutine( RunTransitioningToNextATL() );
            this.derivedSkillLastFrameObject.SetActive( false );

            yield break;
        }
        // NO

        if (_isLeadCounterAttacking)
        {
            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_COUNTER );
            yield return StartCoroutine( PlayAnimation( skillEffectUiAnimator, ( _lead.GetIsPlayer() ) ? "Player_Ariku_Counterattack" : ( this.isUsingGameCharacterV2 ) ? "Player_Ariku_Counterattack-enemy" : "Enemy_Enemy_Counterattack" ) );
        }

        // 先手方是否在“近戰指令時間”或“近戰反擊指令時間”按下主動或反擊技能？
        // YES
        // 当前的距离是否为“近距离”？
        // YES
        if (_isLeadMeleeAttacking
            && this.battleGameManager.GetBattleDistanceManager().GetCurrentDistanceType() == DistanceType.Near)
        {
            // “後手方”發動技能。
            _improviser.ApplyAssignedSkillAsCurrentSkill();

            _improviserCurrentSkill = _improviser.GetCurrentSkill();
            if (_improviserCurrentSkill != null)
            {
                _improviserSkillType = _improviserCurrentSkill.GetSkillData().skillType;
                _improviserSubskillData = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData();

                // 後手方的已按下技能的接觸判定"遠/近"變為"近戰"
                if (_improviserSubskillData.Range == RangeType.melee_or_ranged)
                {
                    _improviserCurrentSkill.SetCurrentRangeType( RangeType.melee );
                }
                // 後手方是否按下了遠程技能?
                // YES
                else if (_improviserSubskillData.Range == RangeType.ranged)
                {
                    // "後手方"得到"近距離遠程方"
                    _improviser.AddCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer );
                }
            }
        }
        // NO
        // 进入“Part A前”。
        else
        {
            _battleResultData = new BattleResultData();

            // 頁面：Part A前

            // 更新"先手方"當前流向
            _lead.TriggerEvent( AnimationEvent.OnCategorizedPassiveTypeUpdated );

            // 頁面：發動流向效果A
            BattleLogicManagerV2.TriggerCategorizedPassiveSkillEffectA( ref _battleResultData, _lead );

            // 判定PART A演出
            AnimationParameterData _leadAnimationParameterData = DetermineAnimationAndVisualEffectForPartA( _lead, _playerCharacter, _enemyCharacter );

            // 頁面：Part A

            // 演出時長為1.6秒
            _animationDuration = ANIMATION_DURATION_FOR_PART_A;
            _animationStartTime = Time.time;

            // -------------------- 先手方 --------------------

            // "先手方"進入【 Part A結算 】。
            BattleLogicManagerV2.DetermineResultForPartA( ref _battleResultData, _lead, _improviser );

            _leadBattleResultData = _battleResultData.GetGameCharacterResultData( _lead );
            ShowBattleLog( _battleResultData.GetResultLogList() );

            // 頁面：Part A結算
            // 播放演出動畫
            StartCoroutine( RunAnimationOfPartA( _lead, _animationDuration, _leadAnimationParameterData ) );

            // 結算演出時機 A - 在 Part A 的第 0.1 秒起播放。
            // 參數：以太值消耗、以太值提升
            LeanTween.delayedCall( RESULT_ANIMATION_TIMING_A, () =>
            {
                // 播放演出同時，UI反映各項參數。
                _lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager, true );

                _lead.ShowPopUpDisplayInfoV2(
                    maxStatePointUp: _leadBattleResultData.maximumStatePointIncreaseForBase
                    //,
                    //statePointDamage: _leadBattleResultData.statePointCost
                    );
            } );

            BattleLog.Instance.AddOnScreenBattleLog(
                $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _lead.GetCharacterName() }</color>"
                + $"消耗了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _leadBattleResultData.statePointCost }{ TerminologyManager.STATE_POINT }</color>"
                + $"和提升了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _leadBattleResultData.maximumStatePointIncreaseForBase }最大{ TerminologyManager.STATE_POINT }</color>。"
            );

            // -----------------------------------------------

            // -------------------- 後手方 --------------------

            // "後手方"進入【 迎擊指令時間 】。
            _improviser.SetIsInRepulseCommandTime( true );

            // 頁面：迎擊指令時間
            // 播放演出動畫
            StartCoroutine( RunAnimationOfPartA( _improviser, _animationDuration ) );

            // -----------------------------------------------

            // 播放演出同時，根據"當前距離"，更新畫面中ATL的距離圖標。
            this.battleGameManager.GetBattleDistanceManager().UpdateBattleDistancePanel();

            yield return new WaitWhile( () => Time.time - _animationStartTime < _animationDuration );

            // 指令結束，禁用技能。
            _lead.TriggerEvent( AnimationEvent.OnTransition );
            _improviser.TriggerEvent( AnimationEvent.OnTransition );

            // “後手方”發動技能。
            _improviser.ApplyAssignedSkillAsCurrentSkill();

            _improviserCurrentSkill = _improviser.GetCurrentSkill();
            if (_improviserCurrentSkill != null)
            {
                RangeType _improviserCurrentSkillRange = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;

                // 當前距離是否近距離?
                // YES
                if (this.battleGameManager.GetBattleDistanceManager().GetCurrentDistanceType() == DistanceType.Near)
                {
                    // 後手方的已按下技能的接觸判定"遠/近"變為"近戰"
                    if (_improviserCurrentSkillRange == RangeType.melee_or_ranged)
                    {
                        _improviserCurrentSkill.SetCurrentRangeType( RangeType.melee );
                    }
                    // "後手方"的已按下技能是否遠程技能?
                    // YES
                    else if (_improviserCurrentSkillRange == RangeType.ranged)
                    {
                        // "後手方"得到"近距離遠程方"
                        _improviser.AddCharacterIdentityType( CharacterIdentityType.NearDistanceRangedDealer );
                    }
                }
                // NO
                else
                {
                    // 後手方的已按下技能的接觸判定"遠/近"變為"遠程"
                    if (_improviserCurrentSkillRange == RangeType.melee_or_ranged)
                    {
                        _improviserCurrentSkill.SetCurrentRangeType( RangeType.ranged );
                    }
                }
            }

            // 開始0.2秒的“PART A戰鬥過場演出”並進入“判定 Part B 結果及結算”。
            this.hasTransitionAnimationEnded = false;
            this.battleGameManager.GetBattleVisualEffectManager().TransitionToNextPart( () => { this.hasTransitionAnimationEnded = true; } );
            yield return new WaitUntil( () => this.hasTransitionAnimationEnded );
        }

        // Hide the lead for Part B if the lead's range type is ranged.
        if (_leadRangeType == RangeType.ranged)
        {
            _lead.HideCharacterObject();
        }

        if (_lead.GetIsPlayer())
        {
            ChangeToBackgroundPartA();
        }
        else
        {
            ChangeToBackgroundPartB();
        }

        this.battleGameManager.GetBattleVisualEffectManager().TriggerAnimationSetDarkenPartB();

        this.targetCamera.transform.position = cameraPosition;
        this.targetCamera.orthographicSize = cameraOrthographicSize;

        _playerCharacter.GetCharacterAnimationHandler().ResetAnimation();
        _enemyCharacter.GetCharacterAnimationHandler().ResetAnimation();

        _lead.GetOpponentContainer().SetActive( true );
        _improviser.ShowCharacterObject();

        if (_improviser.IsInBreakStatus())
        {
            _improviser.Reset();
        }

        // “後手方”有已按下的技能。
        if (_improviserCurrentSkill != null)
        {
            _improviserSkillType = _improviserCurrentSkill.GetSkillData().skillType;
            _improviserSubskillData = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData();

            if (_improviserSkillType == SkillType.repulse)
            {
                _lead.SetCurrentAttacker( _improviser );
            }
            else if (_improviserSkillType == SkillType.backend)
            {
                if (!_improviserSubskillData.IsDefendingSkill && !_improviserSubskillData.IsEvadingSkill)
                {
                    _improviserSkillType = SkillType.none;
                }
            }

            BattleLog.Instance.AddOnScreenBattleLog(
                $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _improviser.GetCharacterName() }</color>使出了"
                + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _improviserSubskillData.DisplayName }</color>"
                + $" （{ TerminologyManager.GetSkillInformationText( _improviserCurrentSkill ) }）。"
                );

            ShowCasterCurrentSkillInfo( _improviser );
        }

        // 進入 Part B 階段。
        StartPartB( out _battleResultData, _lead, _improviser, true, out GameCharacter _winner, out GameCharacter _loser );
        ShowBattleLog( _battleResultData.GetResultLogList() );

        _leadBattleResultData = _battleResultData.GetGameCharacterResultData( _lead );
        _improviserBattleResultData = _battleResultData.GetGameCharacterResultData( _improviser );

        // 判定"己方"的指令時間
        BattleLogicManagerV2.DetermineCommandTimeInPartB( this.battleGameManager, _playerCharacter );
        BattleLogicManagerV2.DetermineCommandTimeInPartB( this.battleGameManager, _enemyCharacter );

        CharacterAnimationHandler _playerCharacterAnimationHandler = _playerCharacter.GetCharacterAnimationHandler();
        CharacterAnimationHandler _enemyCharacterAnimationHandler = _enemyCharacter.GetCharacterAnimationHandler();

        // 判定己方PART B演出
        var ( _animationParameterDataForPlayerOne, _extraAnimationParameterDataForPlayerOne ) = DetermineAnimationOfPlayerOneForPartB( _playerCharacter, _enemyCharacter );

        // 判定敵方PART B演出
        var ( _animationParameterDataForPlayerTwo, _extraAnimationParameterDataForPlayerTwo ) = DetermineAnimationOfPlayerTwoForPartB( _playerCharacter, _enemyCharacter );

        // 判定PART B共用特效
        var ( _visualEffectParameterDataForPlayerOne, _visualEffectParameterDataForPlayerTwo ) = DetermineVisualEffectForPartB_V2( _playerCharacter, _enemyCharacter );

        this.battleGameManager.GetBattleVisualEffectManager().ApplyBlurShaderAtRecipient(_playerCharacter.HasCharacterIdentityType(CharacterIdentityType.Recipient));
        _playerCharacter.HideCharacterObject();
        _enemyCharacter.HideCharacterObject();

        bool _needToWaitForOneSecond = false;

        // 判定後是否有[己方1秒演出]?
        if (_extraAnimationParameterDataForPlayerOne != null)
        {
            _needToWaitForOneSecond = true;

            // "己方"播放已判定的[己方1秒演出]。
            UpdateCameraView( _playerCharacter, _animationParameterDataForPlayerOne );
            _playerCharacterAnimationHandler.LoadAndPlayAnimation( _extraAnimationParameterDataForPlayerOne );
        }

        // 判定後是否有[敵方1秒演出]?
        if (_extraAnimationParameterDataForPlayerTwo != null)
        {
            _needToWaitForOneSecond = true;

            // "敵方"播放已判定的[敵方1秒演出]。
            UpdateCameraView( _enemyCharacter, _extraAnimationParameterDataForPlayerTwo );
            _enemyCharacterAnimationHandler.LoadAndPlayAnimation( _extraAnimationParameterDataForPlayerTwo );
        }

        UpdateGameCharacterVisibility();

        // 開始 Part-B 階段的倒數計時。
        _skillCountdownTime = ANIMATION_DURATION_FOR_PART_B + ( ( _needToWaitForOneSecond ) ? 1.0f : 0.0f );
        _skillCountdownTimeStartTime = Time.time;
        _atlSlotListPanel.GoToEndAtCurrentAtlSlot( _skillCountdownTime );

        // 播放演出同時，根據"當前距離"，更新畫面中ATL的距離圖標。
        this.battleGameManager.GetBattleDistanceManager().UpdateBattleDistancePanel();

        if (_improviserCurrentSkill != null)
        {
            // 結算演出時機 A - 在 Part B 的第 0.1 秒起播放。
            // 參數：以太值消耗、以太值提升
            LeanTween.delayedCall( RESULT_ANIMATION_TIMING_A, () =>
            {
                _improviser.TriggerEvent( AnimationEvent.OnNormalSkillBeingUsed );

                // 播放演出同時，UI反映各項參數。
                _improviser.ApplyBattleResultData( _improviserBattleResultData, this.battleGameManager, true );
                //StartCoroutine( ShowPopUpDisplayInfo( improviser, statePointReduced: improviserBattleResultData.statePointCost, maximumStatePointIncreased: improviserBattleResultData.maximumStatePointIncrease ) );
                _improviser.ShowPopUpDisplayInfoV2(
                    maxStatePointUp: _improviserBattleResultData.maximumStatePointIncreaseForBase
                    //,
                    //statePointDamage: _improviserBattleResultData.statePointCost
                    );
            } );
        }

        if (_needToWaitForOneSecond)
        {
            yield return new WaitForSeconds( 1.0f );
        }

        // 播放已判定的[己方PART B演出]&[敵方PART B演出]&[PART B特效]

        if (_animationParameterDataForPlayerOne != null)
        {
            UpdateCameraView( _playerCharacter, _animationParameterDataForPlayerOne );
            _playerCharacterAnimationHandler.LoadAndPlayAnimation( _animationParameterDataForPlayerOne );
        }

        if (_visualEffectParameterDataForPlayerOne != null)
        {
            _playerCharacterAnimationHandler.LoadAndPlayVisualEffect( _visualEffectParameterDataForPlayerOne );
        }

        if (_animationParameterDataForPlayerTwo != null)
        {
            UpdateCameraView( _enemyCharacter, _animationParameterDataForPlayerTwo );
            _enemyCharacterAnimationHandler.LoadAndPlayAnimation( _animationParameterDataForPlayerTwo );
        }

        if (_visualEffectParameterDataForPlayerTwo != null)
        {
            _enemyCharacterAnimationHandler.LoadAndPlayVisualEffect( _visualEffectParameterDataForPlayerTwo );
        }

        if ((_leadCurrentSkill != null && _leadCurrentSkill.GetSkillData().skillType == SkillType.repulse)
            || (_improviserCurrentSkill != null && _improviserCurrentSkill.GetSkillData().skillType == SkillType.repulse))
        {
            this.battleGameManager.GetBattleVisualEffectManager().ApplyBlurShaderAnimationAtRepulse();
        }

        UpdateGameCharacterVisibility();

        // 首0.3秒後，指令牌出現。
        LeanTween.delayedCall( 0.3f, () =>
        {
            string _commandTimeTypeText = TerminologyManager.GetCommandTimeTypeText( _playerCharacter.GetCurrentCommandTimeType() );
            this.skillPromptPanel.ShowCommandPhase( _commandTimeTypeText, true );
            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _playerCharacter.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { _commandTimeTypeText } 】</color>。" );

            // -------------------- 開始檢查“己方”和“敵方”是否有在 Part-B 階段裡輸入指令。 --------------------

            this.isAbleToAssignSkillInPartB = true;

            StartCoroutine( RunCheckingIfSkillIsSelectedInPartB( _playerCharacter ) );
            StartCoroutine( RunCheckingIfSkillIsSelectedInPartB( _enemyCharacter ) );

            if (_winner != null)
            {
                StartCoroutine( RunCheckingIfDerivedSkillIsSelectedInPartB( _winner, _skillCountdownTime, _skillCountdownTimeStartTime ) );
            }

            // ------------------------------------------------------------------------------------------
        } );

        // 0.5秒後參考【技能按鈕可用性】開啟"己方"可用的指令
        LeanTween.delayedCall( 0.5f, () =>
        {
            _lead.TriggerEvent( AnimationEvent.OnPartB );
            _improviser.TriggerEvent( AnimationEvent.OnPartB );

            // 倒數計時開始。
            ShowCommandPhaseCountdownTimer( true, _playerCharacter, 1.3f );
        } );

        if (this.isUsingGameCharacterV2)
        {
            string _crashef = "crashef";
            bool _hasCrashEffectForLead = false;
            bool _hasCrashEffectForImproviser = false;

            if (_lead == _playerCharacter)
            {
                if (_visualEffectParameterDataForPlayerOne != null)
                {
                    _hasCrashEffectForLead = _visualEffectParameterDataForPlayerOne.GetVisualEffectName().Contains( _crashef );
                }
            }
            else if (_lead == _enemyCharacter)
            {
                if (_visualEffectParameterDataForPlayerTwo != null)
                {
                    _hasCrashEffectForLead = _visualEffectParameterDataForPlayerTwo.GetVisualEffectName().Contains( _crashef );
                }
            }

            if (_improviser == _playerCharacter)
            {
                if (_visualEffectParameterDataForPlayerOne != null)
                {
                    _hasCrashEffectForImproviser = _visualEffectParameterDataForPlayerOne.GetVisualEffectName().Contains( _crashef );
                }
            }
            else if (_improviser == _enemyCharacter)
            {
                if (_visualEffectParameterDataForPlayerTwo != null)
                {
                    _hasCrashEffectForImproviser = _visualEffectParameterDataForPlayerTwo.GetVisualEffectName().Contains( _crashef );
                }
            }

            // 結算演出時機 B - 在partB的第0.6秒起播放。
            // 參數：以太傷害, 負荷傷害, 回避壓力消耗, break字樣演出
            // 生命傷害(只限沒有出現包含“crashef”的演出時)
            // (近戰獎勵版) 以太提升, 回流造成的當前以太增加
            LeanTween.delayedCall( RESULT_ANIMATION_TIMING_B, () =>
            {
                // 播放演出同時，UI反映結算階段的各項參數。
                _lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager );
                _improviser.ApplyBattleResultData( _improviserBattleResultData, this.battleGameManager );

                _lead.ShowPopUpDisplayInfoV2( healthPointDamage: ( _hasCrashEffectForLead ) ? 0.0f : _leadBattleResultData.GetHealthPointDamageTaken(),
                                                stressValueDamage: _leadBattleResultData.stressValueDamageTaken,
                                                statePointDamage: _leadBattleResultData.statePointDamageTaken,
                                                maxStatePointUp: _leadBattleResultData.maximumStatePointIncreaseForBonus
                                                );

                _improviser.ShowPopUpDisplayInfoV2( healthPointDamage: ( _hasCrashEffectForImproviser ) ? 0.0f : _improviserBattleResultData.GetHealthPointDamageTaken(),
                                                    stressValueDamage: _improviserBattleResultData.stressValueDamageTaken,
                                                    statePointDamage: _improviserBattleResultData.statePointDamageTaken,
                                                    maxStatePointUp: _improviserBattleResultData.maximumStatePointIncreaseForBonus
                                                    );
            } );

            // 結算演出時機 C - 在partB的第1.2秒起播放。
            // 參數：生命傷害(當該次演出出現包含“crashef”的演出時)
            LeanTween.delayedCall( RESULT_ANIMATION_TIMING_C, () =>
            {
                if (_hasCrashEffectForLead)
                {
                    _lead.ShowPopUpDisplayInfoV2( healthPointDamage: _leadBattleResultData.GetHealthPointDamageTaken() );
                }

                if (_hasCrashEffectForImproviser)
                {
                    _improviser.ShowPopUpDisplayInfoV2( healthPointDamage: _leadBattleResultData.GetHealthPointDamageTaken() );
                }
            } );
        }
        else
        {
            string _leadCharacterPartB = "";
            string _leadSkillEffectPartB = "";

            if (_lead.GetIsPlayer())
            {
                if (_leadRangeType == RangeType.melee)
                {
                    _leadCharacterPartB = "MeleeAttack_Part_B";
                    _leadSkillEffectPartB = "MeleeAttack_Part_B";
                }
                else if (_leadRangeType == RangeType.ranged)
                {
                    _leadCharacterPartB = "-";
                    _leadSkillEffectPartB = "Fireball_Part_B";
                }
            }
            else
            {
                if (_leadRangeType == RangeType.melee)
                {
                    _leadCharacterPartB = "Attack_Part_B";
                    _leadSkillEffectPartB = "HittingEffect";
                }
                else if (_leadRangeType == RangeType.ranged)
                {
                    _leadCharacterPartB = "-";
                    _leadSkillEffectPartB = "Fireball_Part_B";
                }
            }

            switch ( _improviserSkillType )
            {
                case SkillType.none:

                    //_skillCountdownTime = GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();

                    //_attacker.SetSkillCountdownTime( _skillCountdownTime );
                    //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attacker, AnimationEvent.OnAttackPartB_Cutoff ) );
                    //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                    //ShowCommandPhaseCountdownTimer( true, _attacker, _skillCountdownTime );
                    //ShowCommandPhaseCountdownTimer( true, _attackTarget, _skillCountdownTime );
                    //ShowCommandPhaseCountdownTimer( true, _playerCharacter, _skillCountdownTime );
                    //_atlSlotListPanel.GoToEndAtCurrentAtlSlot( _skillCountdownTime );

                    if (_leadCharacterPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlayCharacterAnimation( _lead, _leadCharacterPartB ) );
                    }

                    if (_leadSkillEffectPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlaySkillEffectAnimation( _lead, _leadSkillEffectPartB ) );
                    }

                    _lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager );
                    _improviser.ApplyBattleResultData( _improviserBattleResultData, this.battleGameManager );

                    this.cameraEffect.Shake();
                    AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                    yield return StartCoroutine( PlayCharacterAnimation( _improviser, GETTING_HIT_ANIMATION_NAME, _improviserBattleResultData ) );
                    //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );

                    break;

                case SkillType.repulse:

                    yield return StartCoroutine( PlayShowingSkillInformation( _improviser ) );

                    if (_leadCharacterPartB != NO_ANIMATION)
                    {
                        StartCoroutine( PlayCharacterAnimation( _lead, _leadCharacterPartB + "_" + REPULSE_ANIMATION_NAME ) );
                    }

                    if (_leadSkillEffectPartB != NO_ANIMATION)
                    {
                        StartCoroutine( PlaySkillEffectAnimation( _lead, _leadSkillEffectPartB + "_" + REPULSE_ANIMATION_NAME ) );
                    }

                    //_skillCountdownTime = ( GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                    //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                    //ShowCommandPhaseCountdownTimer( true, _attacker, _skillCountdownTime );
                    //ShowCommandPhaseCountdownTimer( true, _attackTarget, _skillCountdownTime );
                    //ShowCommandPhaseCountdownTimer( true, _playerCharacter, _skillCountdownTime );
                    //_atlSlotListPanel.GoToEndAtCurrentAtlSlot( _skillCountdownTime );

                    yield return StartCoroutine( PlayCharacterAnimation( _improviser, REPULSE_ANIMATION_NAME ) );
                    yield return StartCoroutine( PlaySkillEffectAnimation( _improviser, REPULSE_ANIMATION_NAME ) );

                    _lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager );
                    _improviser.ApplyBattleResultData( _improviserBattleResultData, this.battleGameManager );

                    bool _hasAssault = false;
                    if (_winner != null)
                    {
                        if (_winner.HasOneOfCharacterIdentityTypes( new CharacterIdentityType[]
                                                                    {
                                                                        CharacterIdentityType.LightAssaulter,
                                                                        CharacterIdentityType.HeavyAssaulter
                                                                    }
                                                                    ))
                        {
                            _hasAssault = true;

                            //_skillCountdownTime = 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                            //_winner.SetSkillCountdownTime( _skillCountdownTime );
                            //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _winner, AnimationEvent.OnRepulseWin_Cutoff ) );

                            this.cameraEffect.Shake();
                            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );

                            string _animationName = GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_" + ( ( _lead.GetIsPlayer() ) ? "Left" : "Right" );

                            ShowPopUpDisplayInfo( _lead, _leadBattleResultData );
                            ShowPopUpDisplayInfo( _improviser, _improviserBattleResultData );

                            if (_winner == _lead && _improviser.IsCharacterObjectActive())
                            {
                                yield return StartCoroutine( PlayCharacterAnimation( _improviser, _animationName, _improviserBattleResultData ) );
                            }
                            else if (_winner == _improviser && _lead.IsCharacterObjectActive())
                            {
                                yield return StartCoroutine( PlayCharacterAnimation( _lead, _animationName, _improviserBattleResultData ) );
                            }
                        }
                    }

                    if (!_hasAssault)
                    {
                        ShowPopUpDisplayInfo( _lead, _leadBattleResultData );
                        ShowPopUpDisplayInfo( _improviser, _improviserBattleResultData );
                    }

                    //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );

                    break;

                case SkillType.backend:

                    yield return StartCoroutine( PlayShowingSkillInformation( _improviser ) );

                    //_skillCountdownTime = ( GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                    //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                    //ShowCommandPhaseCountdownTimer( true, _attacker, _skillCountdownTime );
                    //ShowCommandPhaseCountdownTimer( true, _attackTarget, _skillCountdownTime );
                    //ShowCommandPhaseCountdownTimer( true, _playerCharacter, _skillCountdownTime );
                    //_atlSlotListPanel.GoToEndAtCurrentAtlSlot( _skillCountdownTime );

                    if (_leadCharacterPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlayCharacterAnimation( _lead, _leadCharacterPartB ) );
                    }

                    if (_leadSkillEffectPartB != NO_ANIMATION)
                    {
                        if (_leadSkillEffectPartB != "HittingEffect")
                        {
                            yield return StartCoroutine( PlaySkillEffectAnimation( _lead, _leadSkillEffectPartB ) );
                        }
                    }

                    BattleLog.Instance.AddOnScreenBattleLog( _log );

                    string _attackTargetBackendSkillAnimationCharacterPartA = "";
                    string _attackTargetBackendSkillAnimationSkillEffectPartA = "";

                    if (_improviserSubskillData.IsDefendingSkill)
                    {
                        _attackTargetBackendSkillAnimationCharacterPartA = "Defend";
                        _attackTargetBackendSkillAnimationSkillEffectPartA = "Defend";
                    }
                    else if (_improviserSubskillData.IsEvadingSkill)
                    {
                        _attackTargetBackendSkillAnimationCharacterPartA = "Evade";
                        _attackTargetBackendSkillAnimationSkillEffectPartA = NO_ANIMATION;
                    }

                    _lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager );
                    _improviser.ApplyBattleResultData( _improviserBattleResultData, this.battleGameManager );

                    if (_winner == _lead)
                    {
                        if (_leadSkillEffectPartB == "HittingEffect")
                        {
                            yield return StartCoroutine( PlaySkillEffectAnimation( _lead, _leadSkillEffectPartB ) );
                        }

                        //_skillCountdownTime = 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                        //_attacker.SetSkillCountdownTime( _skillCountdownTime );
                        //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attacker, AnimationEvent.OnAttackPartB_Cutoff ) );

                        this.cameraEffect.Shake();
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                        ShowPopUpDisplayInfo( _lead, _leadBattleResultData );
                        yield return StartCoroutine( PlayCharacterAnimation( _improviser, GETTING_HIT_ANIMATION_NAME, _improviserBattleResultData ) );
                        //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
                    }
                    else if (_winner == _improviser)
                    {
                        //_skillCountdownTime = ( GetAttackAnimationLength( _attackTarget, _attackTargetBackendSkillAnimationCharacterPartA, _attackTargetBackendSkillAnimationSkillEffectPartA ) + 1.0f ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                        //_attackTarget.SetSkillCountdownTime( _skillCountdownTime );
                        //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnDefenseWin_Cutoff ) );

                        if (_improviserSubskillData.IsDefendingSkill)
                        {
                            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DEFEND );
                        }
                        else if (_improviserSubskillData.IsEvadingSkill)
                        {
                            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DODGE );
                        }

                        ShowPopUpDisplayInfo( _lead, _leadBattleResultData );

                        if (_attackTargetBackendSkillAnimationCharacterPartA != NO_ANIMATION)
                        {
                            yield return StartCoroutine( PlayCharacterAnimation( _improviser, _attackTargetBackendSkillAnimationCharacterPartA, _improviserBattleResultData ) );
                        }

                        if (_attackTargetBackendSkillAnimationSkillEffectPartA != NO_ANIMATION)
                        {
                            yield return StartCoroutine( PlaySkillEffectAnimation( _improviser, _attackTargetBackendSkillAnimationSkillEffectPartA ) );
                        }

                        //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
                    }

                    break;
            }
        }

        while (Time.time - _skillCountdownTimeStartTime < _skillCountdownTime)
        {
            yield return null;
        }

        this.isAbleToAssignSkillInPartB = false;

        for (int i = 0; i < _gameCharacters.Length; i++)
        {
            _gameCharacters[ i ].SetCurrentCommandTimeType( CommandTimeType.None );
        }

        _playerCharacter.TriggerEvent( AnimationEvent.OnTransition );
        _enemyCharacter.TriggerEvent( AnimationEvent.OnTransition );

        if (EndPartB( _lead, _improviser ))
        {
            yield break;
        }

        CharacterSkill _leadAssignedSkill = _lead.GetAssignedSkill();
        CharacterSkill _improviserAssignedSkill = _improviser.GetAssignedSkill();

        if (battleFlowRound.GetCurrentATL().GetATLNumber() == GameConfiguration.Instance.GetBattleConfiguration().GetNumberOfATLSlots())
        {
            if (_leadAssignedSkill != null)
            {
                if (_leadAssignedSkill.GetSkillData().skillType == SkillType.derived)
                {
                    battleFlowRound.AddExtraATL();
                }
            }

            if (_improviserAssignedSkill != null)
            {
                if (_improviserAssignedSkill.GetSkillData().skillType == SkillType.derived)
                {
                    battleFlowRound.AddExtraATL();
                }
            }
        }

        bool _isDerivedSkill = false;

        if (( _leadAssignedSkill != null && _leadAssignedSkill.GetSkillData().skillType == SkillType.derived )
            || ( _improviserAssignedSkill != null && _improviserAssignedSkill.GetSkillData().skillType == SkillType.derived ))
        {
            _isDerivedSkill = true;
        }

        if (!_isDerivedSkill)
        {
            yield return StartCoroutine( RunTransitioningToNextATL() );
        }
    }

    private IEnumerator RunCheckingIfSkillIsSelectedInCommandCombatTime( GameCharacter gameCharacter )
    {
        yield return new WaitUntil( () => gameCharacter.GetAssignedSkill() != null );

        if (this.battleGameManager.GetBattleVisualEffectManager().IsShowingCombatCommandCutScreen())
        {
            this.battleGameManager.GetBattleVisualEffectManager().TriggerCharacterTurningAnimation();
        }
    }

    private IEnumerator RunCheckingIfSkillIsSelectedInPartB( GameCharacter gameCharacter )
    {
        gameCharacter.SetIsMeleeAttacking( false );
        gameCharacter.SetIsCounterAttacking( false );

        yield return new WaitWhile( () => ( this.isAbleToAssignSkillInPartB && gameCharacter.GetAssignedSkill() == null ) );

        CharacterSkill _assignedSkill = gameCharacter.GetAssignedSkill();
        if (_assignedSkill != null)
        {
            CommandTimeType _commandTimeType = gameCharacter.GetCurrentCommandTimeType();

            // 是否在“反擊指令”或“近戰反擊指令”階段按下反擊或主動技能？
            if (_commandTimeType is CommandTimeType.CounterAttack or CommandTimeType.MeleeCounterAttack)
            {
                if (_assignedSkill.GetSkillData().skillType is SkillType.counter or SkillType.active)
                {
                    gameCharacter.SetIsCounterAttacking( true );
                }
            }

            // 是否在“近戰指令時間”或“近戰反擊指令時間”按下主動或反擊技能？
            if (_commandTimeType is CommandTimeType.MeleeCombat or CommandTimeType.MeleeCounterAttack)
            {
                if (_assignedSkill.GetSkillData().skillType is SkillType.active or SkillType.counter)
                {
                    gameCharacter.SetIsMeleeAttacking( true );
                }
            }
        }
    }

    private IEnumerator RunCheckingIfDerivedSkillIsSelectedInPartB( GameCharacter gameCharacter, float countdownTime, float countdownStartTime )
    {
        yield return new WaitWhile( () => ( this.isAbleToAssignSkillInPartB && gameCharacter.GetAssignedSkill() == null ) );

        CharacterSkill _assignedSkill = gameCharacter.GetAssignedSkill();
        if (_assignedSkill != null
            && _assignedSkill.GetSkillData().skillType == SkillType.derived)
        {
            this.deriveSkillAnimationHandler.Show( gameCharacter );
            this.deriveSkillAnimationHandler.PlayDeriveAnimationPart1( countdownTime - ( Time.time - countdownStartTime ) );
        }
    }

    private IEnumerator RunTransitioningToNextATL()
    {
        this.hasTransitionAnimationEnded = false;
        this.battleGameManager.GetBattleVisualEffectManager().TransitionToNextATL( () => { this.hasTransitionAnimationEnded = true; } );
        yield return new WaitUntil( () => this.hasTransitionAnimationEnded );
    }

    private IEnumerator RunAnimationOfPartA( GameCharacter gameCharacter, float animationDuration, AnimationParameterData animationParameterData = null )
    {
        float _animationStartTime = 0.0f;

        // “Part A結算”頁面："己方"播放已判定的[已按下技能演出]&[共用特效]
        if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.Lead ))
        {
            if (this.isUsingGameCharacterV2)
            {
                this.skillPromptPanel.PlaySpeedStrengthAnimation( gameCharacter );

                if (animationParameterData != null)
                {
                    gameCharacter.GetCharacterAnimationHandler().LoadAndPlayAnimation( animationParameterData );
                }
            }
            else
            {
                yield return StartCoroutine( ZoomInCameraToTarget( gameCharacter, 0.6f ) );

                RangeType _currentSkillRangeType = gameCharacter.GetCurrentSkillRangeType();
                string _characterPartA = "";
                string _skillEffectPartA = "";

                if (gameCharacter.GetIsPlayer())
                {
                    if (_currentSkillRangeType == RangeType.melee)
                    {
                        _characterPartA = "MeleeAttack_Part_A";
                        _skillEffectPartA = "MeleeAttack_Part_A";
                    }
                    else if (_currentSkillRangeType == RangeType.ranged)
                    {
                        _characterPartA = "RangedAttack";
                        _skillEffectPartA = "Fireball_Part_A";
                    }
                }
                else
                {
                    if (_currentSkillRangeType == RangeType.melee)
                    {
                        _characterPartA = "Attack_Part_A";
                        _skillEffectPartA = "-";
                    }
                    else if (_currentSkillRangeType == RangeType.ranged)
                    {
                        _characterPartA = "RangedAttack";
                        _skillEffectPartA = "Fireball_Part_A";
                    }
                }

                if (_characterPartA != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlayCharacterAnimation( gameCharacter, _characterPartA ) );
                }

                if (_skillEffectPartA != NO_ANIMATION)
                {
                    if (_skillEffectPartA == "Fireball_Part_A")
                    {
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_FIREBALL );
                    }

                    yield return StartCoroutine( PlaySkillEffectAnimation( gameCharacter, _skillEffectPartA ) );
                }
            }
        }
        // “迎擊指令時間”頁面："己方"播放已判定的演出&特效
        else if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
        {
            gameCharacter.SetSkillCountdownTime( animationDuration );

            if (gameCharacter.GetIsPlayer())
            {
                yield return new WaitWhile( () => Time.time - _animationStartTime < 0.1f );

                // 首0.1秒後，指令牌出現。
                this.skillPromptPanel.ShowCommandPhase( TerminologyManager.REPULSE_COMMAND_TIME, true, animationDuration );
            }

            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacter.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.REPULSE_COMMAND_TIME } 】</color>。" );
        }

        yield return new WaitWhile( () => Time.time - _animationStartTime < 0.3f );

        // ⁠0.3秒後，參考【技能按鈕可用性】，開啟可用的指令。
        gameCharacter.TriggerEvent( AnimationEvent.OnPartA );

        if (gameCharacter.GetIsInRepulseCommandTime())
        {
            // 倒數計時開始。
            ShowCommandPhaseCountdownTimer( false, gameCharacter, animationDuration - ( Time.time - _animationStartTime ) );
            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_REPULSE );
        }
    }

    private void StartPartB( out BattleResultData battleResultData,
                             GameCharacter lead, GameCharacter improviser, bool hasPartA,
                             out GameCharacter winner, out GameCharacter loser )
    {
        // ------------------------------ 判定 Part B 結果及結算 ------------------------------

        //battleResultData = BattleLogicManagerV2.DetermineResultForPartB( attacker, attackTarget, out winner, out loser );

        BattleLogicManagerV2.DetermineResultForPartB( out battleResultData, lead, improviser, hasPartA );

        GameCharacter[] _gameCharacters = new GameCharacter[] { lead, improviser };

        winner = BattleLogicManagerV2.GetGameCharacterThatMatchesOneOfCharacterIdentityTypes(
            new CharacterIdentityType[]
            {
                CharacterIdentityType.Assaulter,
                CharacterIdentityType.LightAssaulter,
                CharacterIdentityType.HeavyAssaulter,
                CharacterIdentityType.SuccessfulResister
            },
            _gameCharacters );

        loser = BattleLogicManagerV2.GetGameCharacterThatMatchesOneOfCharacterIdentityTypes(
            new CharacterIdentityType[]
            {
                CharacterIdentityType.Recipient,
                CharacterIdentityType.LightRecipient,
                CharacterIdentityType.HeavyRecipient
            },
            _gameCharacters );

        // ----------------------------------------------------------------------------------

        // 頁面：Part B
        // 頁面：判定距離結果
        battleGameManager.GetBattleDistanceManager().UpdateFinalDistanceResult( improviser );
    }

    private bool EndPartB( GameCharacter attacker, GameCharacter attackTarget )
    {
        GameCharacter[] _gameCharacters = new GameCharacter[] { attacker, attackTarget };

        BattleLogicManagerV2.OnTheEndOfPartB( _gameCharacters, out List<string> _resultLogList );
        ShowBattleLog( _resultLogList );

        this.battleGameManager.GetBattleVisualEffectManager().TurnOffBlurShader();

        if (CheckHasBattleEndedV2())
        {
            this.deriveSkillAnimationHandler.Hide();
            this.derivedSkillLastFrameObject.SetActive( false );
            return true;
        }

        BattleLogicManagerV2.OnTheEndOfATL( _gameCharacters );

        // ---------------------------- Reset ----------------------------

        if (this.isUsingGameCharacterV2)
        {
            attacker.HideCharacterObject();
            attackTarget.HideCharacterObject();
        }
        else
        {
            attacker.GetOwnContainer().SetActive( false );
            attacker.ShowCharacterObject();
            attacker.PlayIdleAnimation();

            attackTarget.PlayIdleAnimation();
        }

        attacker.Reset();
        attackTarget.Reset();
        UpdateGameCharacterVisibility();

        // ---------------------------------------------------------------

        return false;
    }

    private void ShowCommandPhaseCountdownTimer( bool isActiveSkill, GameCharacter gameCharacter, float countdownTime )
    {
        StartCoroutine( RunCommandPhaseCountdownTimer( isActiveSkill, gameCharacter, countdownTime ) );
    }

    private IEnumerator RunCommandPhaseCountdownTimer( bool isActiveSkill, GameCharacter gameCharacter, float countdownTime )
    {
        bool _isPlayer = gameCharacter.GetIsPlayer();

        this.skillPromptPanel.SetCommandPhaseProgressBar( 1.0f, isActiveSkill, _isPlayer );

        //float _startTime = Time.realtimeSinceStartup;
        float _startTime = Time.time;
        float _remainingTime = 0.0f;

        do
        {
            yield return null;

            //_remainingTime = countdownTime - ( Time.realtimeSinceStartup - _startTime );
            _remainingTime = countdownTime - ( Time.time - _startTime );

            float _remainingTimePercentage = _remainingTime / countdownTime;
            skillPromptPanel.SetCommandPhaseProgressBar( _remainingTimePercentage, isActiveSkill, _isPlayer );
        }
        while (_remainingTime > 0);

        gameCharacter.SetIsInRepulseCommandTime( false );
        this.skillPromptPanel.HideCommandPhase( _isPlayer );
    }

    //private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, ATLSlotListPanelV2 atlSlotListPanel, int atlNumber,
    private IEnumerator RunDerivedSkill( GameCharacter lead, GameCharacter improviser, ATLSlotListPanelV3 atlSlotListPanel, int atlNumber )
    {
        StartPartB( out BattleResultData _battleResultData, lead, improviser, false, out GameCharacter _, out GameCharacter _ );

        BattleResultData.BattleResultData_GameCharacter _leadBattleResultData = _battleResultData.GetGameCharacterResultData( lead );
        BattleResultData.BattleResultData_GameCharacter _improviserBattleResultData = _battleResultData.GetGameCharacterResultData( improviser );
        ShowBattleLog( _battleResultData.GetResultLogList() );

        // -------------------------------------------------- 頁面：派生技能演出 --------------------------------------------------

        this.isAnimationEventTriggered = false;
        this.deriveSkillAnimationHandler.Show( lead );
        this.deriveSkillAnimationHandler.PlayDeriveAnimationPart2();

        // 結算演出時機 A - 在 Part B 的第 0.1 秒起播放。
        // 參數：以太值消耗、以太值提升
        LeanTween.delayedCall( RESULT_ANIMATION_TIMING_A, () =>
        {
            // 播放演出同時，UI反映各項參數。
            lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager, true );

            PopUpDisplayInfoV2Canvas.SpawnPopUpDisplayInfoV2( this.popUpDisplayInfoPrefabV2, this.canvasTransform,
                                                              this.deriveSkillAnimationHandler.GetPivot().position, !lead.GetIsPlayer(),
                                                              maxStatePointUp: _leadBattleResultData.maximumStatePointIncreaseForBase
                                                              //,
                                                              //statePointDamage: _leadBattleResultData.statePointCost
                                                              );
        } );

        yield return new WaitUntil( () => this.isAnimationEventTriggered );

        lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager );
        improviser.ApplyBattleResultData( _improviserBattleResultData, this.battleGameManager );

        PopUpDisplayInfoV2Canvas.SpawnPopUpDisplayInfoV2( this.popUpDisplayInfoPrefabV2, this.canvasTransform,
                                                          this.deriveSkillAnimationHandler.GetPivot().position, !lead.GetIsPlayer(),
                                                          healthPointDamage: _leadBattleResultData.actualHealthPointDamageTaken,
                                                          stressValueDamage: _leadBattleResultData.stressValueDamageTaken,
                                                          statePointDamage: _leadBattleResultData.statePointDamageTaken,
                                                          maxStatePointUp: _leadBattleResultData.maximumStatePointIncreaseForBonus );

        PopUpDisplayInfoV2Canvas.SpawnPopUpDisplayInfoV2( this.popUpDisplayInfoPrefabV2, this.canvasTransform,
                                                          this.deriveSkillAnimationHandler.GetPivot().position, !improviser.GetIsPlayer(),
                                                          healthPointDamage: _improviserBattleResultData.actualHealthPointDamageTaken,
                                                          stressValueDamage: _improviserBattleResultData.stressValueDamageTaken,
                                                          statePointDamage: _improviserBattleResultData.statePointDamageTaken,
                                                          maxStatePointUp: _improviserBattleResultData.maximumStatePointIncreaseForBonus );

        this.isAnimationEventTriggered = false;
        yield return new WaitUntil( () => this.isAnimationEventTriggered );

        if (_battleResultData != null)
        {
            //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
        }
        else
        {
            OnCasterBeingUnableToUseSkill( lead );
            yield return new WaitForSeconds( 1.0f );
        }

        this.derivedSkillLastFrameObject.SetActive( true );
        this.deriveSkillAnimationHandler.Hide();

        // ---------------------------------------------------------------------------------------------------------------------
    }

    private IEnumerator PlayCharacterAnimation( GameCharacter gameCharacter, string animationName, BattleResultData.BattleResultData_GameCharacter battleResultData )
    {
        float _damageTaken = 0.0f;
        if (battleResultData.actualHealthPointDamageTaken > 0)
        {
            _damageTaken = battleResultData.actualHealthPointDamageTaken;
        }
        else if (battleResultData.virtualHealthPointDamageTaken > 0)
        {
            _damageTaken = battleResultData.virtualHealthPointDamageTaken;
        }

        yield return StartCoroutine( PlayCharacterAnimation( gameCharacter, animationName, _damageTaken, battleResultData.stressValueDamageTaken, battleResultData.statePointDamageTaken ) );
    }

    private void ShowBattleLog( List<string> resultLogList )
    {
        for (int i = 0; i < resultLogList.Count; i++)
        {
            BattleLog.Instance.AddOnScreenBattleLog( resultLogList[ i ] );
        }
    }

    public bool CheckHasBattleEndedV2()
    {
        List<GameCharacter> _gameCharacters = this.battleGameManager.GetGameCharacterList();

        for (int i = 0; i < _gameCharacters.Count; i++)
        {
            GameCharacter _gameCharacter = _gameCharacters[ i ];
            if (_gameCharacter.GetHasJustDied())
            {
                _gameCharacter.gameObject.SetActive( false );
                BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacter.GetCharacterName() }</color>被擊倒了。" );
            }
        }

        if (BattleLogicManagerV2.HasBattleEnded( _gameCharacters.ToArray(), out int _result ))
        {
            this.onBattleEndedCallback?.Invoke( _result == 1 );
            return true;
        }

        return false;
    }

    private void UpdateCameraView( GameCharacter gameCharacter, AnimationParameterData animationParameterData )
    {
        string _codeTypeString = animationParameterData.GetCodeType().ToString();
        PlayerCharacter _playerCharacter = this.battleGameManager.GetPlayerCharacter();
        EnemyCharacter _enemyCharacter = this.battleGameManager.GetEnemyCharacter();

        if (_codeTypeString.Contains( "camA" ))
        {
            ChangeToBackgroundPartA();
            gameCharacter.ShowCharacterObject();
            BringGameCharacterToFront( _playerCharacter );
            BringGameCharacterToBack( _enemyCharacter );
        }
        else if (_codeTypeString.Contains( "camB" ))
        {
            ChangeToBackgroundPartB();
            gameCharacter.ShowCharacterObject();
            BringGameCharacterToBack( _playerCharacter );
            BringGameCharacterToFront( _enemyCharacter );
        }
    }

    public void UpdateGameCharacterVisibility()
    {
        PlayerCharacter _playerCharacter = this.battleGameManager.GetPlayerCharacter();
        EnemyCharacter _enemyCharacter = this.battleGameManager.GetEnemyCharacter();

        if (this.backgroundIndex == 1)
        {
            _enemyCharacter.ShowCharacterObject();
        }
        else if (this.backgroundIndex == 2)
        {
            _playerCharacter.ShowCharacterObject();
        }
    }

    private void BringGameCharacterToFront( GameCharacter gameCharacter )
    {
        gameCharacter.GetSortingGroup().sortingOrder = 3;
    }

    private void BringGameCharacterToBack( GameCharacter gameCharacter )
    {
        gameCharacter.GetSortingGroup().sortingOrder = 1;
    }

    private void ShowCasterCurrentSkillInfo( GameCharacter caster )
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HINTS );
        this.skillPromptPanel.ShowCasterCurrentSkillInfo( caster );
    }

    public bool GetIsAbleToAssignSkillInPartB()
    {
        return this.isAbleToAssignSkillInPartB;
    }
}
