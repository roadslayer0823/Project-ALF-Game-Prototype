using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;

public partial class BattleAnimationManager : MonoBehaviour
{
    public IEnumerator RunBattleAnimationV2( BattleFlowRound_V2 battleFlowRound, BattleFlowATL_V2 battleFlowATL )
    {
        //ATLSlotListPanelV2 _atlSlotListPanel = this.battleGameManager.GetBattleUiManager().GetATLSlotListPanelV2();
        ATLSlotListPanelV3 _atlSlotListPanel = this.battleGameManager.GetBattleUiManager().GetATLSlotListPanelV3();

        BattleResultData _battleResultData = null;
        BattleResultData.BattleResultData_GameCharacter _leadBattleResultData = null;
        BattleResultData.BattleResultData_GameCharacter _attackTargetBattleResultData = null;

        PlayerCharacter _playerCharacter = this.battleGameManager.GetPlayerCharacter();
        EnemyCharacter _enemyCharacter = this.battleGameManager.GetEnemyCharacter();
        GameCharacter[] _gameCharacters = new GameCharacter[] { _playerCharacter, _enemyCharacter };

        _battleResultData = BattleLogicManagerV2.OnTheStartOfATL( _gameCharacters );
        _playerCharacter.ApplyBattleResultData( _battleResultData.GetGameCharacterResultData( _playerCharacter ), this.battleGameManager );
        _enemyCharacter.ApplyBattleResultData( _battleResultData.GetGameCharacterResultData( _enemyCharacter ), this.battleGameManager );

        bool _isPlayingCombatCommandAnimation = true;

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
            this.battleGameManager.GetBattleDistanceManager().SetCurrentDistanceType( BattleDistanceManager.DistanceType.Normal );

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
                battleGameManager.GetBattleVisualEffectManager().TriggerCombatCommandCutIn( () => { _isPlayingCombatCommandAnimation = false; } );
                yield return new WaitUntil( () => !_isPlayingCombatCommandAnimation );
            }

            battleGameManager.ShowPreparationView();
            _playerCharacter.TriggerEvent( AnimationEvent.OnCombatCommandTimeStarted );
            _enemyCharacter.TriggerEvent( AnimationEvent.OnCombatCommandTimeStarted );

            battleFlowATL.StartAttackOpportunityCountdownTimer( this.skillPromptPanel );
            _atlSlotListPanel.GoToATL( battleFlowATL.GetATLNumber(), battleFlowATL.GetAttackOpportunityDuration() );

            StartCoroutine( RunCheckingIfSkillIsSelectedInCommandCombatTime( _playerCharacter ) );
            yield return new WaitUntil( () => ( !battleFlowATL.GetIsDuringAttackOpportunityPeriod() || ( _playerCharacter.GetAssignedSkill() != null && _enemyCharacter.GetAssignedSkill() != null ) ) );
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
            _playerCharacter.ResetAssignedSkill();
            _enemyCharacter.ResetAssignedSkill();
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

        _lead.PlayCharacterAnimation( "Idle" );
        _improviser.PlayCharacterAnimation( "Idle" );

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

        CharacterSkill _leadCurrentSkill = _lead.GetCurrentSkill();
        Subskill _leadSubskillData = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData();
        RangeType _leadRangeType = _lead.GetCurrentSkillRangeType();

        string _leadCharacterPartB = "";
        string _leadSkillEffectPartB = "";

        _lead.GetSortingGroup().sortingOrder = 3;
        _improviser.GetSortingGroup().sortingOrder = 1;

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
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CAMERACHANGE);
            ChangeToBackgroundPartA();
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

            ChangeToBackgroundPartB();
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

        yield return StartCoroutine( PlayShowingSkillInformation( _lead ) );

        bool _isAttackerCounterAttacking = _lead.GetIsCounterAttacking();
        _lead.SetIsCounterAttacking( false );
        _improviser.SetIsCounterAttacking( false );

        Skill.SkillType _leadSkillType = _lead.GetCurrentSkill().GetSkillData().skillType;

        // 是否有一方按下“派生技能”？
        // YES
        // “先手方”使用派生技能。
        if (_leadSkillType == Skill.SkillType.derived)
        {
            // TODO: 取消對方的任何技能指令並進入“判定 Part B 結果及結算”。

            yield return StartCoroutine( RunDerivedSkill( _lead, _improviser, _atlSlotListPanel, battleFlowRound.GetCurrentATL().GetATLNumber() ) );

            if (EndPartB( _lead, _improviser ))
            {
                yield break;
            }

            battleFlowRound.EndCurrentRound();
            yield return StartCoroutine( RunTransitioningToNextATL() );

            yield break;
        }
        // NO
        // TODO: 先手方是否在“近戰指令時間”或“近戰反擊指令時間”按下主動或反擊技能？
        // YES
        // “先手方”使用反擊技能。
        else if (_isAttackerCounterAttacking)
        {
            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_COUNTER );
            yield return StartCoroutine( PlayAnimation( skillEffectUiAnimator, ( _lead.GetIsPlayer() ) ? "Player_Ariku_Counterattack" : "Enemy_Enemy_Counterattack" ) );
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

            // TODO: 判定PART A演出

            // 頁面：Part A

            // 演出時長為1.6秒
            _animationDuration = 1.6f;
            _animationStartTime = Time.time;

            // -------------------- 先手方 --------------------

            // "先手方"進入【 Part A結算 】。
            BattleLogicManagerV2.DetermineResultForPartA( ref _battleResultData, _lead, _improviser );

            _leadBattleResultData = _battleResultData.GetGameCharacterResultData( _lead );
            _lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager );
            ShowBattleLog( _battleResultData.GetResultLogList() );

            // 頁面：Part A結算
            // 播放演出動畫
            StartCoroutine( RunAnimationOfPartA( _lead, _animationDuration ) );

            // 結算演出時機 A - 在 Part A 的第 0.1 秒起播放。
            // 參數：以太值消耗、以太值提升
            LeanTween.delayedCall( 0.1f, () =>
            {
                // 播放演出同時，UI反映各項參數。
                _lead.ShowPopUpDisplayInfoV2( maxStatePointUp: _leadBattleResultData.maximumStatePointIncrease/*, statePointDamage: _attackerBattleResultData.statePointCost*/ );
            } );

            BattleLog.Instance.AddOnScreenBattleLog(
                $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _lead.GetCharacterName() }</color>"
                + $"消耗了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _leadBattleResultData.statePointCost }{ TerminologyManager.STATE_POINT }</color>"
                + $"和提升了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _leadBattleResultData.maximumStatePointIncrease }最大{ TerminologyManager.STATE_POINT }</color>。"
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

            CharacterSkill _improviserCurrentSkill = _improviser.GetCurrentSkill();
            if (_improviserCurrentSkill != null)
            {
                RangeType _improviserCurrentSkillRange = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;

                // 當前距離是否近距離?
                // YES
                if (this.battleGameManager.GetBattleDistanceManager().GetCurrentDistanceType() == BattleDistanceManager.DistanceType.Near)
                {
                    // 後手方的已按下技能的接觸判定"遠/近"變為"近戰"
                    if (_improviserCurrentSkillRange == RangeType.melee_or_ranged)
                    {
                        _improviser.SetCurrentSkillRangeType( RangeType.melee );
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
                        _improviser.SetCurrentSkillRangeType( RangeType.ranged );
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
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CAMERACHANGE);
            ChangeToBackgroundPartB();
        }
        else
        {
            ChangeToBackgroundPartA();
        }

        this.battleGameManager.GetBattleVisualEffectManager().TriggerAnimationSetDarkenPartB();

        this.targetCamera.transform.position = cameraPosition;
        this.targetCamera.orthographicSize = cameraOrthographicSize;

        _lead.GetOpponentContainer().SetActive( true );
        _improviser.ShowCharacterObject();

        if (_improviser.IsInBreakStatus())
        {
            _improviser.Reset();
        }

        Skill.SkillType _attackTargetSkillType = Skill.SkillType.none;
        Subskill _attackTargetSubskillData = null;

        // “後手方”有已按下的技能。
        if (_improviser.GetCurrentSkill() != null)
        {
            _attackTargetSkillType = _improviser.GetCurrentSkill().GetSkillData().skillType;
            _attackTargetSubskillData = _improviser.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

            if (_attackTargetSkillType == Skill.SkillType.repulse)
            {
                _lead.SetCurrentAttacker( _improviser );
            }
            else if (_attackTargetSkillType == Skill.SkillType.backend)
            {
                if (!_attackTargetSubskillData.IsDefendingSkill && !_attackTargetSubskillData.IsEvadingSkill)
                {
                    _attackTargetSkillType = Skill.SkillType.none;
                }
            }
        }

        // 進入 Part B 階段。
        _skillCountdownTime = 1.8f;
        _skillCountdownTimeStartTime = Time.time;
        _atlSlotListPanel.GoToEndAtCurrentAtlSlot( _skillCountdownTime );

        StartPartB( out List<BattleResultData> _battleResultDataList, _lead, _improviser, out GameCharacter _winner, out GameCharacter _loser );

        for (int i = 0; i < _battleResultDataList.Count; i++)
        {
            ShowBattleLog( _battleResultDataList[ i ].GetResultLogList() );
        }

        _battleResultData = _battleResultDataList[ 1 ];
        _leadBattleResultData = _battleResultData.GetGameCharacterResultData( _lead );
        _attackTargetBattleResultData = _battleResultData.GetGameCharacterResultData( _improviser );

        if (_winner != null)
        {
            StartCoroutine( RunCheckingIfDerivedSkillIsSelected( _winner, _skillCountdownTime, _skillCountdownTimeStartTime ) );
        }

        LeanTween.delayedCall( 0.3f, () =>
        {
            if (_lead.GetIsPlayer())
            {
                this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COMBAT_COMMAND_TIME, true );
            }

            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _lead.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.COMBAT_COMMAND_TIME } 】</color>。" );

            if (_improviser.HasCharacterIdentityType( CharacterIdentityType.SuccessfulResister ))
            {
                _improviser.SetIsCounterAttacking( true );

                if (_improviser.GetIsPlayer())
                {
                    this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COUNTER_COMMAND_TIME, true );
                }

                BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _improviser.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.COUNTER_COMMAND_TIME } 】</color>。" );
            }
            else
            {
                if (_improviser.GetIsPlayer())
                {
                    this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COMBAT_COMMAND_TIME, true );
                }

                BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _improviser.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.COMBAT_COMMAND_TIME } 】</color>。" );
            }
        } );

        LeanTween.delayedCall( 0.5f, () =>
        {
            ShowCommandPhaseCountdownTimer( true, _playerCharacter, 1.3f );
        } );

        switch ( _attackTargetSkillType )
        {
            case Skill.SkillType.none:

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
                _improviser.ApplyBattleResultData( _attackTargetBattleResultData, this.battleGameManager );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return StartCoroutine( PlayCharacterAnimation( _improviser, GETTING_HIT_ANIMATION_NAME, _attackTargetBattleResultData ) );
                //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );

                break;

            case Skill.SkillType.repulse:

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
                _improviser.ApplyBattleResultData( _attackTargetBattleResultData, this.battleGameManager );

                bool _hasAssault = false;
                if (_winner != null)
                {
                    if (_winner.HasCharacterIdentityTypes( new CharacterIdentityType[]
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
                        ShowPopUpDisplayInfo( _improviser, _attackTargetBattleResultData );

                        if (_winner == _lead && _improviser.IsCharacterObjectActive())
                        {
                            yield return StartCoroutine( PlayCharacterAnimation( _improviser, _animationName, _attackTargetBattleResultData ) );
                        }
                        else if (_winner == _improviser && _lead.IsCharacterObjectActive())
                        {
                            yield return StartCoroutine( PlayCharacterAnimation( _lead, _animationName, _attackTargetBattleResultData ) );
                        }
                    }
                }

                if (!_hasAssault)
                {
                    ShowPopUpDisplayInfo( _lead, _leadBattleResultData );
                    ShowPopUpDisplayInfo( _improviser, _attackTargetBattleResultData );
                }

                //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );

                break;

            case Skill.SkillType.backend:

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

                if (_attackTargetSubskillData.IsDefendingSkill)
                {
                    _attackTargetBackendSkillAnimationCharacterPartA = "Defend";
                    _attackTargetBackendSkillAnimationSkillEffectPartA = "Defend";
                }
                else if (_attackTargetSubskillData.IsEvadingSkill)
                {
                    _attackTargetBackendSkillAnimationCharacterPartA = "Evade";
                    _attackTargetBackendSkillAnimationSkillEffectPartA = NO_ANIMATION;
                }

                _lead.ApplyBattleResultData( _leadBattleResultData, this.battleGameManager );
                _improviser.ApplyBattleResultData( _attackTargetBattleResultData, this.battleGameManager );

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
                    yield return StartCoroutine( PlayCharacterAnimation( _improviser, GETTING_HIT_ANIMATION_NAME, _attackTargetBattleResultData ) );
                    //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
                }
                else if (_winner == _improviser)
                {
                    //_skillCountdownTime = ( GetAttackAnimationLength( _attackTarget, _attackTargetBackendSkillAnimationCharacterPartA, _attackTargetBackendSkillAnimationSkillEffectPartA ) + 1.0f ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                    //_attackTarget.SetSkillCountdownTime( _skillCountdownTime );
                    //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnDefenseWin_Cutoff ) );

                    if (_attackTargetSubskillData.IsDefendingSkill)
                    {
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DEFEND );
                    }
                    else if (_attackTargetSubskillData.IsEvadingSkill)
                    {
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DODGE );
                    }

                    ShowPopUpDisplayInfo( _lead, _leadBattleResultData );

                    if (_attackTargetBackendSkillAnimationCharacterPartA != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlayCharacterAnimation( _improviser, _attackTargetBackendSkillAnimationCharacterPartA, _attackTargetBattleResultData ) );
                    }

                    if (_attackTargetBackendSkillAnimationSkillEffectPartA != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlaySkillEffectAnimation( _improviser, _attackTargetBackendSkillAnimationSkillEffectPartA ) );
                    }

                    //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
                }

                break;
        }

        while (Time.time - _skillCountdownTimeStartTime < _skillCountdownTime)
        {
            yield return null;
        }

        _playerCharacter.TriggerEvent( AnimationEvent.OnTransition );
        _enemyCharacter.TriggerEvent( AnimationEvent.OnTransition );

        if (EndPartB( _lead, _improviser ))
        {
            yield break;
        }

        if (battleFlowRound.GetCurrentATL().GetATLNumber() == GameConfiguration.Instance.GetBattleConfiguration().GetNumberOfATLSlots())
        {
            CharacterSkill _attackerAssignedSkill = _lead.GetAssignedSkill();
            if (_attackerAssignedSkill != null)
            {
                if (_attackerAssignedSkill.GetSkillData().skillType == Skill.SkillType.derived)
                {
                    battleFlowRound.AddExtraATL();
                }
            }

            CharacterSkill _attackTargetAssignedSkill = _improviser.GetAssignedSkill();
            if (_attackTargetAssignedSkill != null)
            {
                if (_attackTargetAssignedSkill.GetSkillData().skillType == Skill.SkillType.derived)
                {
                    battleFlowRound.AddExtraATL();
                }
            }
        }

        yield return StartCoroutine( RunTransitioningToNextATL() );
    }

    private IEnumerator RunCheckingIfSkillIsSelectedInCommandCombatTime( GameCharacter gameCharacter )
    {
        yield return new WaitUntil( () => gameCharacter.GetAssignedSkill() != null );

        if (this.battleGameManager.GetBattleVisualEffectManager().IsShowingCombatCommandCutScreen())
        {
            this.battleGameManager.GetBattleVisualEffectManager().TriggerCharacterTurningAnimation();
        }
    }

    private IEnumerator RunCheckingIfDerivedSkillIsSelected( GameCharacter gameCharacter, float countdownTime, float countdownStartTime )
    {
        yield return new WaitUntil( () => gameCharacter.GetAssignedSkill() != null );

        if (gameCharacter.GetAssignedSkill().GetSkillData().skillType == Skill.SkillType.derived)
        {
            this.deriveSkillAnimationHandler.gameObject.SetActive( true );
            this.deriveSkillAnimationHandler.PlayDeriveAnimationPart1( countdownTime - ( Time.time - countdownStartTime ) );
        }
    }

    private IEnumerator RunTransitioningToNextATL()
    {
        this.hasTransitionAnimationEnded = false;
        this.battleGameManager.GetBattleVisualEffectManager().TransitionToNextATL( () => { this.hasTransitionAnimationEnded = true; } );
        yield return new WaitUntil( () => this.hasTransitionAnimationEnded );
    }

    private IEnumerator RunAnimationOfPartA( GameCharacter gameCharacter, float animationDuration )
    {
        float _animationStartTime = 0.0f;

        // “Part A結算”頁面："己方"播放已判定的[已按下技能演出]&[共用特效]
        if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.Lead ))
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

    private void StartPartB( out List<BattleResultData> battleResultDataList,
                             GameCharacter lead, GameCharacter improviser, out GameCharacter winner, out GameCharacter loser )
    {
        // ------------------------------ 判定 Part B 結果及結算 ------------------------------

        //battleResultData = BattleLogicManagerV2.DetermineResultForPartB( attacker, attackTarget, out winner, out loser );

        battleResultDataList = new List<BattleResultData>();
        BattleLogicManagerV2.DetermineResultForPartB( ref battleResultDataList, lead, improviser, true );

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

        // "先手方"已按下技能是否"派生技能"?
        // YES
        if (lead.GetCurrentSkill().GetSkillData().skillType == Skill.SkillType.derived)
        {
            // TODO: 派生技能演出
        }

        // TODO: 判定"己方"的指令時間

        // TODO: 判定己方PART B演出

        // TODO: 判定敵方PART B演出

        // TODO: 判定PART B共用特效

        if (improviser.GetCurrentSkill() != null)
        {
            BattleResultData.BattleResultData_GameCharacter _improviserBattleResultData = battleResultDataList[ 0 ].GetGameCharacterResultData( improviser );

            // 結算演出時機 A - 在 Part B 的第 0.1 秒起播放。
            // 參數：以太值消耗、以太值提升
            LeanTween.delayedCall( 0.1f, () =>
            {
                improviser.TriggerEvent( AnimationEvent.OnNormalSkillBeingUsed );
                //StartCoroutine( ShowPopUpDisplayInfo( improviser, statePointReduced: improviserBattleResultData.statePointCost, maximumStatePointIncreased: improviserBattleResultData.maximumStatePointIncrease ) );
                improviser.ShowPopUpDisplayInfoV2( maxStatePointUp: _improviserBattleResultData.maximumStatePointIncrease/*, statePointDamage: improviserBattleResultData.statePointCost*/ );
            } );
        }

        this.battleGameManager.GetBattleVisualEffectManager().ApplyBlurShaderAtRecipient();

        LeanTween.delayedCall( 0.5f, () =>
        {
            lead.TriggerEvent( AnimationEvent.OnPartB );
            improviser.TriggerEvent( AnimationEvent.OnPartB );
        } );
    }

    private bool EndPartB( GameCharacter attacker, GameCharacter attackTarget )
    {
        GameCharacter[] _gameCharacters = new GameCharacter[] { attacker, attackTarget };

        BattleLogicManagerV2.OnTheEndOfPartB( _gameCharacters, out List<string> _resultLogList );
        ShowBattleLog( _resultLogList );

        this.battleGameManager.GetBattleVisualEffectManager().TurnOffBlurShader();

        if (CheckHasBattleEndedV2())
        {
            return true;
        }

        BattleLogicManagerV2.OnTheEndOfATL( _gameCharacters );

        // ---------------------------- Reset ----------------------------

        attacker.GetOwnContainer().SetActive( false );
        attacker.ShowCharacterObject();
        attacker.PlayCharacterAnimation( IDLE_ANIMATION_NAME );
        attacker.Reset();

        attackTarget.PlayCharacterAnimation( IDLE_ANIMATION_NAME );
        attackTarget.Reset();

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
    private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, ATLSlotListPanelV3 atlSlotListPanel, int atlNumber )
    {
        StartPartB( out List<BattleResultData> _battleResultDataList,
                    attacker, attackTarget, out GameCharacter _, out GameCharacter _ );

        BattleResultData _battleResultData = _battleResultDataList[ 1 ];
        BattleResultData.BattleResultData_GameCharacter _attackerBattleResultData = _battleResultData.GetGameCharacterResultData( attacker );
        BattleResultData.BattleResultData_GameCharacter _attackTargetBattleResultData = _battleResultData.GetGameCharacterResultData( attackTarget );
        ShowBattleLog( _battleResultData.GetResultLogList() );

        this.isAnimationEventTriggered = false;
        this.deriveSkillAnimationHandler.PlayDeriveAnimationPart2();
        yield return new WaitUntil( () => this.isAnimationEventTriggered );
        attacker.ApplyBattleResultData( _attackerBattleResultData, this.battleGameManager );
        attackTarget.ApplyBattleResultData( _attackTargetBattleResultData, this.battleGameManager );

        PopUpDisplayInfoV2Canvas.SpawnPopUpDisplayInfoV2( this.popUpDisplayInfoPrefabV2, this.canvasTransform,
                                                          this.deriveSkillAnimationHandler.GetPivot().position, !attackTarget.GetIsPlayer(),
                                                          healthPointDamage: _attackTargetBattleResultData.actualHealthPointDamageTaken,
                                                          stressValueDamage: _attackTargetBattleResultData.stressValueDamageTaken,
                                                          statePointDamage: _attackTargetBattleResultData.statePointDamageTaken );

        this.isAnimationEventTriggered = false;
        yield return new WaitUntil( () => this.isAnimationEventTriggered );

        /*
        attacker.GetSortingGroup().sortingOrder = 3;
        attackTarget.GetSortingGroup().sortingOrder = 1;

        CharacterSkill _attackerSkill = attacker.GetCurrentSkill();
        Subskill _attackerSubskillData = _attackerSkill.GetCharacterSubskillData().GetSubskillData();
        RangeType _attackerRangeType = _attackerSubskillData.Range;

        if (_attackerRangeType == RangeType.melee)
        {
            if (_battleResultData != null)
            {
                yield return StartCoroutine( RunMeleeDerivedSkillAnimation( attacker, _attackerSkill, DatabaseManager.Instance.GetSkillAnimation( _attackerSubskillData.Id ), atlSlotListPanel, atlNumber ) );

                attacker.ApplyBattleResultData( _attackerBattleResultData, this.battleGameManager );
                attackTarget.ApplyBattleResultData( _attackTargetBattleResultData, this.battleGameManager );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return StartCoroutine( PlayCharacterAnimation( attackTarget, GETTING_HIT_ANIMATION_NAME, _attackTargetBattleResultData ) );
            }
        }
        else
        {
            attacker.ShowCharacterObject();
            ChangeToBackgroundPartA();
            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CAMERACHANGE);
            attacker.GetOpponentContainer().SetActive( false );

            if (_battleResultData != null)
            {
                yield return StartCoroutine( RunRangedDerivedSkillAnimation( attacker, _attackerSkill, attackTarget, atlSlotListPanel, atlNumber ) );

                attacker.ApplyBattleResultData( _attackerBattleResultData, this.battleGameManager );
                attackTarget.ApplyBattleResultData( _attackTargetBattleResultData, this.battleGameManager );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return null;
                ShowPopUpDisplayInfo( attackTarget, _attackTargetBattleResultData );
                yield return new WaitForSeconds( 0.7f );
            }
        }
        */

        if (_battleResultData != null)
        {
            //yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
        }
        else
        {
            OnCasterBeingUnableToUseSkill( attacker );
            yield return new WaitForSeconds( 1.0f );
        }

        this.deriveSkillAnimationHandler.gameObject.SetActive( false );
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
}
