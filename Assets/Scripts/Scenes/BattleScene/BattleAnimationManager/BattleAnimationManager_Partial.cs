using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using SkillAnimation = DatabaseManager.SkillAnimation;

public class BattleAnimationManagerV2 : MonoBehaviour
{
    //public IEnumerator RunBattleAnimationV3(BattleGameManager battleGameManager, BattleFlowRound_V2 battleFlowRound, BattleFlowATL_V2 battleFlowATL)
    //{
    //    //ATLSlotListPanelV2 _atlSlotListPanel = battleGameManager.GetBattleUiManager().GetATLSlotListPanelV2();
    //    ATLSlotListPanelV3 _atlSlotListPanel = battleGameManager.GetBattleUiManager().GetATLSlotListPanelV3();

    //    BattleResultData _battleResultData = null;
    //    BattleResultData.BattleResultData_GameCharacter _attackerBattleResultData = null;
    //    BattleResultData.BattleResultData_GameCharacter _attackTargetBattleResultData = null;

    //    PlayerCharacter _playerCharacter = battleGameManager.GetPlayerCharacter();
    //    _playerCharacter.TriggerEvent(AnimationEvent.SetCharacter);

    //    EnemyCharacter _enemyCharacter = battleGameManager.GetEnemyCharacter();
    //    _enemyCharacter.TriggerEvent(AnimationEvent.SetCharacter);

    //    if (BattleLogicManagerV2.ShouldCombatCommandTimeBeSkipped(_playerCharacter, _enemyCharacter))
    //    {
    //        _atlSlotListPanel.GoToATL(battleFlowATL.GetATLNumber(), 0.1f);
    //    }
    //    else
    //    {
    //        this.skillPromptPanel.ShowCommandPhase(TerminologyManager.COMBAT_COMMAND_TIME, true);
    //        this.skillPromptPanel.ShowCommandPhase(TerminologyManager.COMBAT_COMMAND_TIME, false);
    //        BattleLog.Instance.AddOnScreenBattleLog($"雙方進入<color={BattleLog.SPECIAL_COLOR_CODE}>【 {TerminologyManager.COMBAT_COMMAND_TIME} 】</color>。");

    //        _playerCharacter.TriggerEvent(AnimationEvent.OnCombatCommandTimeStarted);
    //        _enemyCharacter.TriggerEvent(AnimationEvent.OnCombatCommandTimeStarted);

    //        battleFlowATL.StartAttackOpportunityCountdownTimer(this.skillPromptPanel);
    //        _atlSlotListPanel.GoToATL(battleFlowATL.GetATLNumber(), battleFlowATL.GetAttackOpportunityDuration());
    //        yield return new WaitUntil(() => (!battleFlowATL.GetIsDuringAttackOpportunityPeriod() || (_playerCharacter.GetAssignedSkill() != null && _enemyCharacter.GetAssignedSkill() != null)));
    //        _atlSlotListPanel.GoToMiddleAtCurrentAtlSlot(0.1f);
    //        this.skillPromptPanel.HideCommandPhase(true);
    //        this.skillPromptPanel.HideCommandPhase(false);
    //    }

    //    BattleLog.Instance.AddOnScreenBattleLog($"<color={BattleLog.SPECIAL_COLOR_CODE}>判定先後手方</color>");

    //    var (_attacker, _attackTarget) = BattleLogicManagerV2.DetermineLeadAndImproviser(_playerCharacter, _enemyCharacter);

    //    if (_attacker == null || _attackTarget == null)
    //    {
    //        BattleLog.Instance.AddOnScreenBattleLog("沒有先後手方。當前 ATL 結束。");
    //        yield break;
    //    }

    //    // 有“先手方”和“後手方”。

    //    BattleLog.Instance.AddOnScreenBattleLog($"<color={BattleLog.SPECIAL_COLOR_CODE}>判定結果</color>為"
    //                                             + $"<color={BattleLog.KEYWORD_COLOR_CODE}>{_attacker.GetCharacterName()}</color>成为<color={BattleLog.SPECIAL_COLOR_CODE}>“先手方”</color>，"
    //                                             + $"<color={BattleLog.KEYWORD_COLOR_CODE}>{_attackTarget.GetCharacterName()}</color>成为<color={BattleLog.SPECIAL_COLOR_CODE}>“后手方”</color>。");

    //    // “先手方”發動技能。
    //    _attacker.ApplyAssignedSkillAsCurrentSkill();
    //    _attacker.PlayCharacterAnimation("Idle");
    //    _attackTarget.PlayCharacterAnimation("Idle");

    //    float _skillAnimationLength = 0.0f;
    //    float _skillCountdownTime = 0.0f;
    //    bool _isAbleToUseSkill = false;
    //    string _log = "";

    //    this.gameCharacterList = new List<GameCharacter>()
    //    {
    //        _attacker,
    //        _attackTarget
    //    };

    //    Subskill _attackerSubskillData = _attacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
    //    SkillAnimation _skillAnimation = DatabaseManager.Instance.GetSkillAnimation(_attackerSubskillData.Id);
    //    RangeType _attackerRangeType = _attackerSubskillData.Range;

    //    string _attackerCharacterPartA = _skillAnimation.CharacterPartA;
    //    string _attackerCharacterPartB = _skillAnimation.CharacterPartB;
    //    string _attackerSkillEffectPartA = _skillAnimation.SkillEffectPartA;
    //    string _attackerSkillEffectPartB = _skillAnimation.SkillEffectPartB;

    //    _attacker.GetSortingGroup().sortingOrder = 3;
    //    _attackTarget.GetSortingGroup().sortingOrder = 1;

    //    if (_attacker.GetIsPlayer())
    //    {
    //        // TODO: Currently, the player character has ranged attack animations only.
    //        // TODO: Added a temporary fix here to handle the situation that the skill's range type is melee-or-ranged.
    //        _attackerRangeType = RangeType.ranged;
    //        ChangeToBackgroundPartA();
    //    }
    //    else
    //    {
    //        // TODO: Currently, the enemy character has melee attack animations only.
    //        _attackerRangeType = RangeType.melee;
    //        _attackerCharacterPartA = "Attack_Part_A";
    //        _attackerCharacterPartB = "Attack_Part_B";
    //        _attackerSkillEffectPartA = "-";
    //        _attackerSkillEffectPartB = "HittingEffect";

    //        ChangeToBackgroundPartB();
    //    }

    //    _attacker.GetOwnContainer().SetActive(true);
    //    _attacker.ShowCharacterObject();
    //    _attacker.GetOpponentContainer().SetActive(false);

    //    yield return new WaitForSeconds(0.1f);

    //    _isAbleToUseSkill = BattleLogicManager.IsAbleToUseSkill(_attacker);

    //    if (_isAbleToUseSkill)
    //    {
    //        _attacker.TriggerEvent(AnimationEvent.OnSkillBeingUsed);

    //        _battleResultData = new BattleResultData();
    //        BattleLogicManagerV2.ExecuteCasterSkillOnUse(ref _battleResultData, _attacker, _attackTarget);

    //        _attackerBattleResultData = _battleResultData.GetGameCharacterResultData(_attacker);
    //        _attacker.ApplyBattleResultData(_attackerBattleResultData);

    //        //StartCoroutine( ShowPopUpDisplayInfo( _attacker, statePointReduced: _attackerBattleResultData.statePointCost, maximumStatePointIncreased: _attackerBattleResultData.maximumStatePointIncrease ) );
    //        _attacker.ShowPopUpDisplayInfoV2(maxStatePointUp: _attackerBattleResultData.maximumStatePointIncrease/*, statePointDamage: _attackerBattleResultData.statePointCost*/ );

    //        _attackTarget.SetCurrentAttacker(_attacker);
    //        this.currentCaster = _attacker;

    //        _attacker.TriggerEvent(AnimationEvent.OnPartA);
    //        _attackTarget.TriggerEvent(AnimationEvent.OnPartA);

    //        yield return StartCoroutine(PlayShowingSkillInformation(_attacker));

    //        Skill.SkillType _attackerSkillType = _attacker.GetCurrentSkill().GetSkillData().skillType;
    //        if (_attackerSkillType == Skill.SkillType.derived)
    //        {
    //            _battleResultData = BattleLogicManagerV2.DetermineResultForPartB(_attacker, _attackTarget, out _, out _);
    //            _attackerBattleResultData = _battleResultData.GetGameCharacterResultData(_attacker);
    //            _attackTargetBattleResultData = _battleResultData.GetGameCharacterResultData(_attackTarget);

    //            yield return StartCoroutine(RunDerivedSkill(_attacker, _attackTarget, battleFlowRound, _atlSlotListPanel, _battleResultData, _attackerBattleResultData, _attackTargetBattleResultData));

    //            if (CheckHasBattleEnded())
    //            {
    //                yield break;
    //            }

    //            _attacker.GetOwnContainer().SetActive(false);
    //            _attacker.ShowCharacterObject();
    //            _attacker.PlayCharacterAnimation(IDLE_ANIMATION_NAME);
    //            _attackTarget.PlayCharacterAnimation(IDLE_ANIMATION_NAME);

    //            _attacker.Reset();
    //            _attackTarget.Reset();

    //            yield break;
    //        }
    //        else if (_attackerSkillType == Skill.SkillType.counter)
    //        {
    //            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_COUNTER);

    //            if (_attacker.GetIsPlayer())
    //            {
    //                yield return StartCoroutine(PlayAnimation(skillEffectUiAnimator, "Player_Ariku_Counterattack"));
    //            }
    //            else
    //            {
    //                yield return StartCoroutine(PlayAnimation(skillEffectUiAnimator, "Enemy_Enemy_Counterattack"));
    //            }
    //        }
    //        else
    //        {
    //            _attackTarget.SetIsInRepulseCommandTime(true);

    //            _skillAnimationLength = GetAttackAnimationLength(_attacker, _attackerCharacterPartA, _attackerSkillEffectPartA) + 1.0f;
    //            _skillCountdownTime = _skillAnimationLength * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
    //            _attackTarget.SetSkillCountdownTime(_skillCountdownTime);
    //            StartCoroutine(CountdownForEventCutoff(_skillCountdownTime, _attackTarget, AnimationEvent.OnDefensePartA_Cutoff));

    //            this.skillPromptPanel.ShowCommandPhase(TerminologyManager.REPULSE_COMMAND_TIME, _attackTarget.GetIsPlayer(), _skillCountdownTime);
    //            ShowCommandPhaseCountdownTimer(true, _attackTarget, _skillCountdownTime);
    //            BattleLog.Instance.AddOnScreenBattleLog($"<color={BattleLog.KEYWORD_COLOR_CODE}>{_attackTarget.GetCharacterName()}</color>進入<color={BattleLog.SPECIAL_COLOR_CODE}>【 {TerminologyManager.REPULSE_COMMAND_TIME} 】</color>。");
    //        }
    //    }
    //    else
    //    {
    //        OnCasterBeingUnableToUseSkill(_attacker);
    //    }

    //    yield return StartCoroutine(ZoomInCameraToTarget(_attacker, 1.0f));

    //    if (!_isAbleToUseSkill)
    //    {
    //        this.targetCamera.transform.position = cameraPosition;
    //        this.targetCamera.orthographicSize = cameraOrthographicSize;

    //        _attacker.Reset();
    //        _attackTarget.Reset();

    //        yield break;
    //    }

    //    if (_attackerCharacterPartA != NO_ANIMATION)
    //    {
    //        yield return StartCoroutine(PlayCharacterAnimation(_attacker, _attackerCharacterPartA));
    //    }

    //    if (_attackerSkillEffectPartA != NO_ANIMATION)
    //    {
    //        if (_attackerSkillEffectPartA == "Fireball_Part_A")
    //        {
    //            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_FIREBALL);
    //        }

    //        yield return StartCoroutine(PlaySkillEffectAnimation(_attacker, _attackerSkillEffectPartA));
    //    }

    //    // Hide the attacker for Part B if the attacker's range type is ranged.
    //    if (_attackerRangeType == RangeType.ranged)
    //    {
    //        _attacker.HideCharacterObject();
    //    }

    //    if (_attacker.GetIsPlayer())
    //    {
    //        ChangeToBackgroundPartB();
    //    }
    //    else
    //    {
    //        ChangeToBackgroundPartA();
    //    }

    //    this.targetCamera.transform.position = cameraPosition;
    //    this.targetCamera.orthographicSize = cameraOrthographicSize;

    //    _attacker.GetOpponentContainer().SetActive(true);
    //    _attackTarget.ShowCharacterObject();

    //    // “後手方”發動技能。
    //    _attackTarget.ApplyAssignedSkillAsCurrentSkill();

    //    float _attackDamage = 0;
    //    float _stressValueDamage = 0;
    //    float _statePointDamage = 0;

    //    if (_attackTarget.GetIsInBreakStatus())
    //    {
    //        _attackTarget.Reset();
    //    }

    //    Skill.SkillType _attackTargetSkillType = Skill.SkillType.none;
    //    Subskill _attackTargetSubskillData = null;

    //    // “後手方”有已按下的技能。
    //    if (_attackTarget.GetCurrentSkill() != null)
    //    {
    //        _attackTargetSkillType = _attackTarget.GetCurrentSkill().GetSkillData().skillType;
    //        _attackTargetSubskillData = _attackTarget.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

    //        if (_attackTargetSkillType != Skill.SkillType.none)
    //        {
    //            if (BattleLogicManager.IsAbleToUseSkill(_attackTarget))
    //            {
    //                if (_attackTargetSkillType == Skill.SkillType.backend)
    //                {
    //                    if (!_attackTargetSubskillData.IsDefendingSkill && !_attackTargetSubskillData.IsEvadingSkill)
    //                    {
    //                        _attackTargetSkillType = Skill.SkillType.none;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                _attackTargetSkillType = Skill.SkillType.none;
    //                OnCasterBeingUnableToUseSkill(_attackTarget);
    //            }
    //        }
    //    }

    //    // 判定 Part B 結果及結算。
    //    _battleResultData = BattleLogicManagerV2.DetermineResultForPartB(_attacker, _attackTarget, out GameCharacter _winner, out GameCharacter _loser);
    //    _attackerBattleResultData = _battleResultData.GetGameCharacterResultData(_attacker);
    //    _attackTargetBattleResultData = _battleResultData.GetGameCharacterResultData(_attackTarget);

    //    // 結算“後手方”已按下的技能的以太值和最大以太值提升。
    //    _attackTarget.TriggerEvent(AnimationEvent.OnSkillBeingUsed);
    //    //StartCoroutine( ShowPopUpDisplayInfo( _attackTarget, statePointReduced: _attackTargetBattleResultData.statePointCost, maximumStatePointIncreased: _attackTargetBattleResultData.maximumStatePointIncrease ) );
    //    _attackTarget.ShowPopUpDisplayInfoV2(maxStatePointUp: _attackTargetBattleResultData.maximumStatePointIncrease/*, statePointDamage: _attackTargetBattleResultData.statePointCost*/ );
    //    this.currentCaster = _attackTarget;

    //    _attacker.TriggerEvent(AnimationEvent.OnPartB);
    //    _attackTarget.TriggerEvent(AnimationEvent.OnPartB);

    //    this.skillPromptPanel.ShowCommandPhase(TerminologyManager.COMBAT_COMMAND_TIME, _attacker.GetIsPlayer());
    //    BattleLog.Instance.AddOnScreenBattleLog($"<color={BattleLog.KEYWORD_COLOR_CODE}>{_attacker.GetCharacterName()}</color>進入<color={BattleLog.SPECIAL_COLOR_CODE}>【 {TerminologyManager.COMBAT_COMMAND_TIME} 】</color>。");

    //    if (_attackTarget.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.SuccessfulResister
    //        || _attackTarget.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.SuccessfulDefender
    //        || _attackTarget.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.SuccessfulEvader)
    //    {
    //        this.skillPromptPanel.ShowCommandPhase(TerminologyManager.COUNTER_COMMAND_TIME, _attackTarget.GetIsPlayer());
    //        BattleLog.Instance.AddOnScreenBattleLog($"<color={BattleLog.KEYWORD_COLOR_CODE}>{_attackTarget.GetCharacterName()}</color>進入<color={BattleLog.SPECIAL_COLOR_CODE}>【 {TerminologyManager.COUNTER_COMMAND_TIME} 】</color>。");
    //    }
    //    else
    //    {
    //        this.skillPromptPanel.ShowCommandPhase(TerminologyManager.COMBAT_COMMAND_TIME, _attackTarget.GetIsPlayer());
    //        BattleLog.Instance.AddOnScreenBattleLog($"<color={BattleLog.KEYWORD_COLOR_CODE}>{_attackTarget.GetCharacterName()}</color>進入<color={BattleLog.SPECIAL_COLOR_CODE}>【 {TerminologyManager.COMBAT_COMMAND_TIME} 】</color>。");
    //    }

    //    switch (_attackTargetSkillType)
    //    {
    //        case Skill.SkillType.none:

    //            _skillCountdownTime = (GetAttackAnimationLength(_attacker, _attackerCharacterPartB, _attackerSkillEffectPartB) + 1.0f) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
    //            _attacker.SetSkillCountdownTime(_skillCountdownTime);
    //            StartCoroutine(CountdownForEventCutoff(_skillCountdownTime, _attacker, AnimationEvent.OnAttackPartB_Cutoff));
    //            StartCoroutine(CountdownForEventCutoff(_skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished));

    //            ShowCommandPhaseCountdownTimer(true, _attacker, _skillCountdownTime);
    //            ShowCommandPhaseCountdownTimer(true, _attackTarget, _skillCountdownTime);
    //            _atlSlotListPanel.GoToEndAtCurrentAtlSlot(_skillCountdownTime);

    //            if (_attackerCharacterPartB != NO_ANIMATION)
    //            {
    //                yield return StartCoroutine(PlayCharacterAnimation(_attacker, _attackerCharacterPartB));
    //            }

    //            if (_attackerSkillEffectPartB != NO_ANIMATION)
    //            {
    //                yield return StartCoroutine(PlaySkillEffectAnimation(_attacker, _attackerSkillEffectPartB));
    //            }

    //            _attacker.ApplyBattleResultData(_attackerBattleResultData);
    //            _attackTarget.ApplyBattleResultData(_attackTargetBattleResultData);

    //            this.cameraEffect.Shake();
    //            AudioManager.Instance.PlaySoundEffect(AUDIO_ID_HIT);
    //            yield return StartCoroutine(PlayCharacterAnimation(_attackTarget, GETTING_HIT_ANIMATION_NAME, _attackTargetBattleResultData));
    //            yield return StartCoroutine(WaitForPopUpDisplayInfoCompleted());

    //            BattleLogicManager.OnCharacterAttackFinished(_attacker, _attackTarget);

    //            break;

    //        case Skill.SkillType.repulse:

    //            yield return StartCoroutine(PlayShowingSkillInformation(_attackTarget));

    //            if (_attackerCharacterPartB != NO_ANIMATION)
    //            {
    //                StartCoroutine(PlayCharacterAnimation(_attacker, _attackerCharacterPartB + "_" + REPULSE_ANIMATION_NAME));
    //            }

    //            if (_attackerSkillEffectPartB != NO_ANIMATION)
    //            {
    //                StartCoroutine(PlaySkillEffectAnimation(_attacker, _attackerSkillEffectPartB + "_" + REPULSE_ANIMATION_NAME));
    //            }

    //            _skillCountdownTime = (GetAttackAnimationLength(_attacker, _attackerCharacterPartB, _attackerSkillEffectPartB)) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
    //            StartCoroutine(CountdownForEventCutoff(_skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished));

    //            ShowCommandPhaseCountdownTimer(true, _attacker, _skillCountdownTime);
    //            ShowCommandPhaseCountdownTimer(true, _attackTarget, _skillCountdownTime);
    //            _atlSlotListPanel.GoToEndAtCurrentAtlSlot(_skillCountdownTime);

    //            yield return StartCoroutine(PlayCharacterAnimation(_attackTarget, REPULSE_ANIMATION_NAME));
    //            yield return StartCoroutine(PlaySkillEffectAnimation(_attackTarget, REPULSE_ANIMATION_NAME));

    //            _attacker.ApplyBattleResultData(_attackerBattleResultData);
    //            _attackTarget.ApplyBattleResultData(_attackTargetBattleResultData);

    //            if (_attacker.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.LightAssaulter
    //                || _attacker.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.HeavyAssaulter)
    //            {
    //                _skillCountdownTime = 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
    //                _winner.SetSkillCountdownTime(_skillCountdownTime);
    //                StartCoroutine(CountdownForEventCutoff(_skillCountdownTime, _attacker, AnimationEvent.OnRepulseWin_Cutoff));

    //                this.cameraEffect.Shake();
    //                AudioManager.Instance.PlaySoundEffect(AUDIO_ID_HIT);

    //                string _animationName = GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_" + ((_attacker.GetIsPlayer()) ? "Left" : "Right");

    //                ShowPopUpDisplayInfo(_attacker, _attackerBattleResultData);
    //                ShowPopUpDisplayInfo(_attackTarget, _attackTargetBattleResultData);

    //                if (_winner == _attacker)
    //                {
    //                    yield return StartCoroutine(PlayCharacterAnimation(_attackTarget, _animationName, _attackTargetBattleResultData));
    //                }
    //                else if (_winner == _attackTarget)
    //                {
    //                    yield return StartCoroutine(PlayCharacterAnimation(_attacker, _animationName, _attackTargetBattleResultData));
    //                }
    //            }
    //            else
    //            {
    //                ShowPopUpDisplayInfo(_attacker, _attackerBattleResultData);
    //                ShowPopUpDisplayInfo(_attackTarget, _attackTargetBattleResultData);
    //            }

    //            yield return StartCoroutine(WaitForPopUpDisplayInfoCompleted());
    //            BattleLogicManager.OnCharacterAttackFinished(_attacker, _attackTarget);

    //            break;

    //        case Skill.SkillType.backend:

    //            yield return StartCoroutine(PlayShowingSkillInformation(_attackTarget));

    //            _skillCountdownTime = (GetAttackAnimationLength(_attacker, _attackerCharacterPartB, _attackerSkillEffectPartB)) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
    //            StartCoroutine(CountdownForEventCutoff(_skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished));

    //            ShowCommandPhaseCountdownTimer(true, _attacker, _skillCountdownTime);
    //            ShowCommandPhaseCountdownTimer(true, _attackTarget, _skillCountdownTime);
    //            _atlSlotListPanel.GoToEndAtCurrentAtlSlot(_skillCountdownTime);

    //            if (_attackerCharacterPartB != NO_ANIMATION)
    //            {
    //                yield return StartCoroutine(PlayCharacterAnimation(_attacker, _attackerCharacterPartB));
    //            }

    //            if (_attackerSkillEffectPartB != NO_ANIMATION)
    //            {
    //                if (_attackerSkillEffectPartB != "HittingEffect")
    //                {
    //                    yield return StartCoroutine(PlaySkillEffectAnimation(_attacker, _attackerSkillEffectPartB));
    //                }
    //            }

    //            BattleLog.Instance.AddOnScreenBattleLog(_log);

    //            SkillAnimation _attackTargetBackendSkillAnimation = DatabaseManager.Instance.GetSkillAnimation(_attackTargetSubskillData.Id);
    //            string _attackTargetBackendSkillAnimationCharacterPartA = _attackTargetBackendSkillAnimation.CharacterPartA;
    //            string _attackTargetBackendSkillAnimationSkillEffectPartA = _attackTargetBackendSkillAnimation.SkillEffectPartA;

    //            _attacker.ApplyBattleResultData(_attackerBattleResultData);
    //            _attackTarget.ApplyBattleResultData(_attackTargetBattleResultData);

    //            if (_winner == _attacker)
    //            {
    //                if (_attackerSkillEffectPartB == "HittingEffect")
    //                {
    //                    yield return StartCoroutine(PlaySkillEffectAnimation(_attacker, _attackerSkillEffectPartB));
    //                }

    //                _skillCountdownTime = 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
    //                _attacker.SetSkillCountdownTime(_skillCountdownTime);
    //                StartCoroutine(CountdownForEventCutoff(_skillCountdownTime, _attacker, AnimationEvent.OnAttackPartB_Cutoff));

    //                this.cameraEffect.Shake();
    //                AudioManager.Instance.PlaySoundEffect(AUDIO_ID_HIT);
    //                yield return StartCoroutine(PlayCharacterAnimation(_loser, GETTING_HIT_ANIMATION_NAME, _attackDamage, _stressValueDamage, _statePointDamage));
    //                yield return StartCoroutine(WaitForPopUpDisplayInfoCompleted());
    //                BattleLogicManager.OnCharacterAttackFinished(_attacker, _attackTarget);
    //            }
    //            else if (_winner == _attackTarget)
    //            {
    //                _skillCountdownTime = (GetAttackAnimationLength(_attackTarget, _attackTargetBackendSkillAnimationCharacterPartA, _attackTargetBackendSkillAnimationSkillEffectPartA) + 1.0f) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
    //                _attackTarget.SetSkillCountdownTime(_skillCountdownTime);
    //                StartCoroutine(CountdownForEventCutoff(_skillCountdownTime, _attackTarget, AnimationEvent.OnDefenseWin_Cutoff));

    //                if (_attackTargetSubskillData.IsDefendingSkill)
    //                {
    //                    AudioManager.Instance.PlaySoundEffect(AUDIO_ID_DEFEND);
    //                }
    //                else if (_attackTargetSubskillData.IsEvadingSkill)
    //                {
    //                    AudioManager.Instance.PlaySoundEffect(AUDIO_ID_DODGE);
    //                }

    //                if (_attackTargetBackendSkillAnimationCharacterPartA != NO_ANIMATION)
    //                {
    //                    yield return StartCoroutine(PlayCharacterAnimation(_attackTarget, _attackTargetBackendSkillAnimationCharacterPartA));
    //                }

    //                if (_attackTargetBackendSkillAnimationSkillEffectPartA != NO_ANIMATION)
    //                {
    //                    yield return StartCoroutine(PlaySkillEffectAnimation(_attackTarget, _attackTargetBackendSkillAnimationSkillEffectPartA));
    //                }

    //                yield return new WaitForSeconds(1.0f);
    //                BattleLogicManager.OnCharacterAttackFinished(_attacker, _attackTarget);
    //            }

    //            break;
    //    }

    //    if (CheckHasBattleEnded())
    //    {
    //        yield break;
    //    }

    //    _attacker.GetOwnContainer().SetActive(false);
    //    _attacker.ShowCharacterObject();
    //    _attacker.PlayCharacterAnimation(IDLE_ANIMATION_NAME);
    //    _attackTarget.PlayCharacterAnimation(IDLE_ANIMATION_NAME);

    //    _attacker.Reset();
    //    _attackTarget.Reset();

    //    if (battleFlowRound.GetCurrentATL().GetATLNumber() == GameConfiguration.Instance.GetBattleConfiguration().GetNumberOfATLSlots())
    //    {
    //        CharacterSkill _attackerCurrentSkill = _attacker.GetCurrentSkill();
    //        if (_attackerCurrentSkill != null)
    //        {
    //            if (_attackerCurrentSkill.GetSkillData().skillType == Skill.SkillType.derived)
    //            {
    //                battleFlowRound.AddExtraATL();
    //            }
    //        }
    //    }
    //}
}
