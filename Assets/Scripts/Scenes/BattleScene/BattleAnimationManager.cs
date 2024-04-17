using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzeGames.Effects;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using SkillAnimation = DatabaseManager.SkillAnimation;

public class BattleAnimationManager : MonoBehaviour
{
    [Header( "Settings" )]
    [SerializeField] private float backgroundDarknessDuration = 0.6f;
    [SerializeField] private float skillNormalDarkness = 0.5f;
    [SerializeField] private float skillTimeStopDarkness = 0.8f;

    [Header( "References" )]
    [SerializeField] private SpriteRenderer background = null;
    [SerializeField] private Sprite backgroundPartA = null;
    [SerializeField] private Sprite backgroundPartB = null;
    [SerializeField] private Animator skillEffectAnimator = null;
    [SerializeField] private Animator skillEffectUiAnimator = null;

    [SerializeField] private Camera targetCamera = null;
    [SerializeField] private CameraEffects cameraEffect = null;
    [SerializeField] private SpriteRenderer darkLayer = null;

    [SerializeField] private SkillPromptPanelV2 skillPromptPanel = null;
    [SerializeField] private DebugBattleDialogMenu debugBattleDialogMenu = null;

    private BattleGameManager battleGameManager = null;
    private bool isAnimationEventTriggered = false;
    private Action<bool> onBattleEndedCallback = null;

    private GameObject targetCameraObject = null;
    private Vector3 cameraPosition = Vector3.zero;
    private float cameraOrthographicSize = 0.0f;

    private List<GameCharacter> gameCharacterList = null;
    private GameCharacter currentCaster = null;

    private bool isDebugMode = false;
    private DebugBattleDialogMenu.ResultType debugModeResultType = DebugBattleDialogMenu.ResultType.None;

    // Animations
    private const string NO_ANIMATION = "-";
    private const string IDLE_ANIMATION_NAME = "Idle";
    private const string GETTING_HIT_ANIMATION_NAME = "GettingHit";
    private const string REPULSE_ANIMATION_NAME = "Repulse";
    private const string DERIVE_ANIMATION_NAME = "Derive";

    // Audios
    private const string AUDIO_ID_ATTACK = "attack";
    private const string AUDIO_ID_COUNTER = "counter";
    private const string AUDIO_ID_DEFEND = "defend";
    private const string AUDIO_ID_DODGE = "dodge";
    private const string AUDIO_ID_FIREBALL = "fireball";
    private const string AUDIO_ID_HINTS = "hints";
    private const string AUDIO_ID_HIT = "hit";

    public enum AnimationEvent
    {
        None,
        SetCharacter,
        OnAttackPartB,
        OnAttackPartB_Cutoff,
        OnDefensePartA,
        OnDefensePartA_Cutoff,
        OnRepulseWin,
        OnRepulseWin_Cutoff,
        OnDefenseWin,
        OnDefenseWin_Cutoff,
        OnBeingInBreakStatus,
        OnObservingSkillSelected,
        OnSkillBeingObserved,
        OnActiveSkillStarted,
        OnActiveSkillFinished,
        OnCombatCommandTimeStarted,
        OnPartA,
        OnPartB,
        OnSkillBeingUsed
    }

    public void Initialize( BattleGameManager battleGameManager, Action<bool> onBattleEndedCallback )
    {
        this.battleGameManager = battleGameManager;
        this.onBattleEndedCallback = onBattleEndedCallback;

        this.targetCameraObject = this.targetCamera.gameObject;
        this.cameraPosition = this.targetCamera.transform.position;
        this.cameraOrthographicSize = this.targetCamera.orthographicSize;

        this.debugBattleDialogMenu.Initialize( OnDebugBattleDialogMenuResult );
    }

    private void OnDebugBattleDialogMenuResult( DebugBattleDialogMenu.ResultType resultType )
    {
        this.debugModeResultType = resultType;
    }

    public IEnumerator RunBattleAnimation( BattleFlowRound battleFlowRound, BattleFlowATL battleFlowATL )
    {
        GameCharacter _attacker = battleFlowATL.GetSelectedCharacter();

        if (_attacker.GetIsInBreakStatus())
        {
            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attacker.GetCharacterName() }</color>正在處於<color={ BattleLog.KEYWORD_COLOR_CODE }>崩潰狀態</color>。" );
            _attacker.MinusBreakStatusRemainingATLs();
            yield break;
        }

        GameCharacter _attackTarget = battleFlowATL.GetAttackTarget();
        _attacker.SetCurrentSkill( battleFlowATL.GetSelectedSkill() );

        //ATLSlotListPanelV2 _atlSlotListPanel = this.battleGameManager.GetBattleUiManager().GetATLSlotListPanelV2();
        ATLSlotListPanelV3 _atlSlotListPanel = this.battleGameManager.GetBattleUiManager().GetATLSlotListPanelV3();
        float _skillAnimationLength = 0.0f;
        float _skillCountdownTime = 0.0f;
        bool _hasCounterAttack = false;
        bool _isAbleToUseSkill = false;
        string _log = "";

        do
        {
            _hasCounterAttack = false;

            this.gameCharacterList = new List<GameCharacter>()
            {
                _attacker,
                _attackTarget
            };

            Subskill _attackerSubskillData = _attacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
            SkillAnimation _skillAnimation = DatabaseManager.Instance.GetSkillAnimation( _attackerSubskillData.Id );
            RangeType _attackerRangeType = _attackerSubskillData.Range;

            string _attackerCharacterPartA = _skillAnimation.CharacterPartA;
            string _attackerCharacterPartB = _skillAnimation.CharacterPartB;
            string _attackerSkillEffectPartA = _skillAnimation.SkillEffectPartA;
            string _attackerSkillEffectPartB = _skillAnimation.SkillEffectPartB;

            _attacker.GetSortingGroup().sortingOrder = 3;
            _attackTarget.GetSortingGroup().sortingOrder = 1;

            if (_attacker.GetIsPlayer())
            {
                ChangeToBackgroundPartA();
            }
            else
            {
                _attackerRangeType = RangeType.melee;
                _attackerCharacterPartA = "Attack_Part_A";
                _attackerCharacterPartB = "Attack_Part_B";
                _attackerSkillEffectPartA = "-";
                _attackerSkillEffectPartB = "HittingEffect";

                ChangeToBackgroundPartB();
            }

            _attacker.GetOwnContainer().SetActive( true );
            _attacker.ShowCharacterObject();
            _attacker.GetOpponentContainer().SetActive( false );

            yield return new WaitForSeconds( 0.1f );

            _isAbleToUseSkill = BattleLogicManager.IsAbleToUseSkill( _attacker );

            if (_isAbleToUseSkill)
            {
                _attackTarget.SetCurrentAttacker( _attacker );
                BattleLogicManager.ExecuteCasterSkillOnUse( _attacker, _attackTarget, out _log );
                ShowSkillInfo( _attacker, _attackTarget );
                this.currentCaster = _attacker;

                BattleLog.Instance.AddOnScreenBattleLog( _log );

                _attacker.TriggerEvent( AnimationEvent.SetCharacter );
                _attackTarget.TriggerEvent( AnimationEvent.SetCharacter );

                yield return StartCoroutine( PlayShowingSkillInformation( _attacker ) );

                _skillAnimationLength = GetAttackAnimationLength( _attacker, _attackerCharacterPartA, _attackerSkillEffectPartA ) + 1.0f;
                _skillCountdownTime = _skillAnimationLength * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                _attackTarget.SetSkillCountdownTime( _skillCountdownTime );
                _attackTarget.TriggerEvent( AnimationEvent.OnDefensePartA );
                StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnDefensePartA_Cutoff ) );

                if (_attacker.GetCurrentSkill().GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
                {
                    _attackTarget.TriggerEvent( AnimationEvent.OnActiveSkillStarted );
                }

                _atlSlotListPanel.GoToATL( battleFlowATL.GetATLNumber(), _skillAnimationLength, _attacker.GetCurrentSkill() );
            }
            else
            {
                OnCasterBeingUnableToUseSkill( _attacker );
            }

            yield return StartCoroutine( ZoomInCameraToTarget( _attacker, 1.0f ) );

            if (!_isAbleToUseSkill)
            {
                this.targetCamera.transform.position = cameraPosition;
                this.targetCamera.orthographicSize = cameraOrthographicSize;

                _attacker.Reset();
                _attackTarget.Reset();

                yield break;
            }

            if (_attackerCharacterPartA != NO_ANIMATION)
            {
                yield return StartCoroutine( PlayCharacterAnimation( _attacker, _attackerCharacterPartA ) );
            }

            if (_attackerSkillEffectPartA != NO_ANIMATION)
            {
                if (_attackerSkillEffectPartA == "Fireball_Part_A")
                {
                    AudioManager.Instance.PlaySoundEffect( AUDIO_ID_FIREBALL );
                }

                yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartA ) );
            }

            // Hide the attacker for Part B if the attacker's range type is ranged.
            if (_attackerRangeType == RangeType.ranged)
            {
                _attacker.HideCharacterObject();
            }

            if (_attacker.GetIsPlayer())
            {
                ChangeToBackgroundPartB();
            }
            else
            {
                ChangeToBackgroundPartA();
            }

            this.targetCamera.transform.position = cameraPosition;
            this.targetCamera.orthographicSize = cameraOrthographicSize;

            _attacker.GetOpponentContainer().SetActive( true );
            _attackTarget.ShowCharacterObject();

            GameCharacter _winner = null;
            GameCharacter _loser = null;
            CharacterSkill _derivedSkill = null;
            float _attackDamage = 0;
            float _stressValueDamage = 0;
            float _statePointDamage = 0;

            if (_attackTarget.GetIsInBreakStatus())
            {
                _attackTarget.Reset();
            }

            Skill.SkillType _attackTargetSkillType = Skill.SkillType.none;
            Subskill _attackTargetSubskillData = null;

            if (_attackTarget.GetCurrentSkill() != null)
            {
                _attackTargetSkillType = _attackTarget.GetCurrentSkill().GetSkillData().skillType;
                _attackTargetSubskillData = _attackTarget.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

                if (_attackTargetSkillType != Skill.SkillType.none)
                {
                    if (BattleLogicManager.IsAbleToUseSkill( _attackTarget ))
                    {
                        if (_attackTargetSkillType == Skill.SkillType.backend)
                        {
                            if (!_attackTargetSubskillData.IsDefendingSkill && !_attackTargetSubskillData.IsEvadingSkill)
                            {
                                _attackTargetSkillType = Skill.SkillType.none;
                            }
                        }
                    }
                    else
                    {
                        _attackTargetSkillType = Skill.SkillType.none;
                        OnCasterBeingUnableToUseSkill( _attackTarget );
                    }
                }
            }

            switch ( _attackTargetSkillType )
            {
                case Skill.SkillType.none:

                    _skillCountdownTime = ( GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) + 1.0f ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                    _attacker.SetSkillCountdownTime( _skillCountdownTime );
                    _attacker.TriggerEvent( AnimationEvent.OnAttackPartB );
                    StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attacker, AnimationEvent.OnAttackPartB_Cutoff ) );
                    StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                    if (_attackerCharacterPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlayCharacterAnimation( _attacker, _attackerCharacterPartB ) );
                    }

                    if (_attackerSkillEffectPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartB ) );
                    }

                    BattleLogicManager.ExecuteCasterSkillOnHit( _attacker, _attackTarget, true, out _attackDamage, out _stressValueDamage, out _statePointDamage, out _log );
                    BattleLog.Instance.AddOnScreenBattleLog( _log );

                    this.cameraEffect.Shake();
                    AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                    yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME, _attackDamage, _stressValueDamage, _statePointDamage ) );
                    yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );

                    BattleLogicManager.OnCharacterAttackFinished( _attacker, _attackTarget );

                    if (CheckHasBattleEnded())
                    {
                        yield break;
                    }

                    if (_attacker.GetCurrentSkill().GetSkillData().skillType == Skill.SkillType.derived)
                    {
                        yield return StartCoroutine( RunDerivedSkill( _attacker, _attackTarget, battleFlowRound, _atlSlotListPanel ) );
                    }

                    if (CheckHasBattleEnded())
                    {
                        yield break;
                    }

                    break;

                case Skill.SkillType.repulse:

                    yield return StartCoroutine( PlayShowingSkillInformation( _attackTarget ) );

                    if (_attackerCharacterPartB != NO_ANIMATION)
                    {
                        StartCoroutine( PlayCharacterAnimation( _attacker, _attackerCharacterPartB + "_" + REPULSE_ANIMATION_NAME ) );
                    }

                    if (_attackerSkillEffectPartB != NO_ANIMATION)
                    {
                        StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartB + "_" + REPULSE_ANIMATION_NAME ) );
                    }

                    _skillCountdownTime = ( GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                    StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                    BattleFlowATL _attackTargetNextATL = battleFlowRound.GetNextATL( _attackTarget );
                    battleFlowRound.GoToTargetATL( _attackTargetNextATL, false );

                    _attacker.SetCurrentAttacker( _attackTarget );
                    BattleLogicManager.ExecuteCasterSkillOnUse( _attackTarget, _attacker, out _log );
                    ShowSkillInfo( _attacker, _attackTarget );
                    this.currentCaster = _attackTarget;

                    BattleLog.Instance.AddOnScreenBattleLog( _log );

                    CharacterSkill _repulseSkill = _attackTarget.GetCurrentSkill();

                    _atlSlotListPanel.GoToATL( _attackTargetNextATL.GetATLNumber(), GetAttackAnimationLength( _attackTarget, REPULSE_ANIMATION_NAME, REPULSE_ANIMATION_NAME ), _repulseSkill );
                    yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );
                    yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );

                    if (this.isDebugMode)
                    {
                        this.debugBattleDialogMenu.Show( true, true, true );
                        yield return new WaitUntil( () => this.debugModeResultType != DebugBattleDialogMenu.ResultType.None );

                        if (this.debugModeResultType == DebugBattleDialogMenu.ResultType.AttackerWins)
                        {
                            _winner = _attacker;
                            _loser = _attackTarget;
                        }
                        else if (this.debugModeResultType == DebugBattleDialogMenu.ResultType.DefenderWins)
                        {
                            _winner = _attackTarget;
                            _loser = _attacker;
                        }

                        this.debugModeResultType = DebugBattleDialogMenu.ResultType.None;
                    }
                    else
                    {
                        BattleLogicManager.CompareCharacterSkillAttributes( BattleLogicManager.ActionType.Repulse,
                                                                            _attacker, _attackTarget,
                                                                            out _winner, out _loser );
                    }

                    bool _hasAttackDamage = false;
                    bool _hasStressValueDamage = false;
                    bool _hasStatePointDamage = false;

                    if (_winner != null)
                    {
                        BattleLog.Instance.AddOnScreenBattleLog( $"迎擊結果為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _winner.GetCharacterName() }</color>勝利。" );

                        RangeType _attackTargetRangeType = _repulseSkill.GetCharacterSubskillData().GetSubskillData().Range;

                        if (_attackTarget is EnemyCharacter)
                        {
                            _attackTargetRangeType = RangeType.melee;
                        }

                        if (_attackerRangeType == RangeType.melee)
                        {
                            if (_attackTargetRangeType == RangeType.melee)
                            {
                                _hasAttackDamage = true;
                                _hasStressValueDamage = true;
                                _hasStatePointDamage = true;
                                OnHitWithNoDamage( _loser, _winner, false );
                            }
                            else if (_attackTargetRangeType == RangeType.ranged)
                            {
                                if (_winner == _attackTarget)
                                {
                                    _hasAttackDamage = true;
                                    _hasStressValueDamage = true;
                                    _hasStatePointDamage = true;
                                }
                                else
                                {
                                    OnHitWithNoDamage( _loser, _winner, false );
                                }
                            }
                        }
                        else if (_attackerRangeType == RangeType.ranged)
                        {
                            if (_attackTargetRangeType == RangeType.melee)
                            {
                                if (_winner == _attacker)
                                {
                                    _hasAttackDamage = true;
                                    _hasStressValueDamage = true;
                                    _hasStatePointDamage = true;
                                }
                                else
                                {
                                    OnHitWithNoDamage( _loser, _winner, false );
                                }
                            }
                            else if (_attackTargetRangeType == RangeType.ranged)
                            {
                                _hasAttackDamage = true;
                                _hasStressValueDamage = true;
                                _hasStatePointDamage = true;
                            }
                        }

                        BattleLogicManager.ExecuteCasterSkillOnHit( _winner, _loser, _hasAttackDamage, _hasStressValueDamage, _hasStatePointDamage, true, out _attackDamage, out _stressValueDamage, out _statePointDamage, out _log );
                        BattleLog.Instance.AddOnScreenBattleLog( _log );
                    }
                    else
                    {
                        BattleLog.Instance.AddOnScreenBattleLog( $"迎擊結果為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attacker.GetCharacterName() }</color>和<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attackTarget.GetCharacterName() }</color>打平。" );

                        OnHitWithNoDamage( _attacker, _attackTarget, true );
                        OnHitWithNoDamage( _attackTarget, _attacker, true );
                    }

                    if (_hasAttackDamage)
                    {
                        _skillCountdownTime = 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                        _winner.SetSkillCountdownTime( _skillCountdownTime );
                        _winner.TriggerEvent( AnimationEvent.OnRepulseWin );
                        StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attacker, AnimationEvent.OnRepulseWin_Cutoff ) );

                        this.cameraEffect.Shake();
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                        yield return StartCoroutine( PlayCharacterAnimation( _loser, GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_"
                                                                                     + ( ( _attacker.GetIsPlayer() ) ? "Left" : "Right" ),
                                                                                     _attackDamage, _stressValueDamage, _statePointDamage ) );
                    }

                    yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
                    BattleLogicManager.OnCharacterAttackFinished( _attacker, _attackTarget );

                    if (_hasAttackDamage)
                    {
                        if (CheckHasBattleEnded())
                        {
                            yield break;
                        }

                        _derivedSkill = _winner.GetCurrentSkill().GetCharacterSubskillData().GetSelectedDerivedSkill();
                    }

                    if (_winner != null)
                    {
                        if (_winner == _attacker || _attackerRangeType == RangeType.melee)
                        {
                            if (_winner.GetCurrentSkill().GetSkillData().skillType == Skill.SkillType.derived)
                            {
                                yield return StartCoroutine( RunDerivedSkill( _winner, _loser, battleFlowRound, _atlSlotListPanel ) );

                                if (CheckHasBattleEnded())
                                {
                                    yield break;
                                }
                            }
                        }
                    }

                    break;

                case Skill.SkillType.backend:

                    yield return StartCoroutine( PlayShowingSkillInformation( _attackTarget ) );

                    _skillCountdownTime = ( GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                    StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                    if (_attackerCharacterPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlayCharacterAnimation( _attacker, _attackerCharacterPartB ) );
                    }

                    if (_attackerSkillEffectPartB != NO_ANIMATION)
                    {
                        if (_attackerSkillEffectPartB != "HittingEffect")
                        {
                            yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartB ) );
                        }
                    }

                    BattleLogicManager.ExecuteCasterSkillOnUse( _attackTarget, _attacker, out _log );
                    ShowSkillInfo( _attacker, _attackTarget );
                    this.currentCaster = _attackTarget;

                    BattleLog.Instance.AddOnScreenBattleLog( _log );

                    SkillAnimation _attackTargetBackendSkillAnimation = DatabaseManager.Instance.GetSkillAnimation( _attackTargetSubskillData.Id );
                    string _attackTargetBackendSkillAnimationCharacterPartA = _attackTargetBackendSkillAnimation.CharacterPartA;
                    string _attackTargetBackendSkillAnimationSkillEffectPartA = _attackTargetBackendSkillAnimation.SkillEffectPartA;

                    _winner = null;
                    _loser = null;

                    if (this.isDebugMode)
                    {
                        this.debugBattleDialogMenu.Show( true, true, false );
                        yield return new WaitUntil( () => this.debugModeResultType != DebugBattleDialogMenu.ResultType.None );

                        if (this.debugModeResultType == DebugBattleDialogMenu.ResultType.AttackerWins)
                        {
                            _winner = _attacker;
                            _loser = _attackTarget;
                        }
                        else if (this.debugModeResultType == DebugBattleDialogMenu.ResultType.DefenderWins)
                        {
                            _winner = _attackTarget;
                            _loser = _attacker;
                        }

                        this.debugModeResultType = DebugBattleDialogMenu.ResultType.None;
                    }
                    else
                    {
                        if (_attackTargetSubskillData.IsDefendingSkill)
                        {
                            BattleLogicManager.CompareCharacterSkillAttributes( BattleLogicManager.ActionType.Defend,
                                                                                _attacker, _attackTarget,
                                                                                out _winner, out _loser );
                        }
                        else if (_attackTargetSubskillData.IsEvadingSkill)
                        {
                            BattleLogicManager.CompareCharacterSkillAttributes( BattleLogicManager.ActionType.Evade,
                                                                                _attacker, _attackTarget,
                                                                                out _winner, out _loser );
                        }
                    }

                    BattleLogicManager.ExecuteCasterSkillOnHit( _winner, _loser, _winner == _attacker, out _attackDamage, out _stressValueDamage, out _statePointDamage, out _log );
                    BattleLog.Instance.AddOnScreenBattleLog( _log );

                    if (_winner == _attacker)
                    {
                        if (_attackerSkillEffectPartB == "HittingEffect")
                        {
                            yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartB ) );
                        }

                        _skillCountdownTime = 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                        _attacker.SetSkillCountdownTime( _skillCountdownTime );
                        _attacker.TriggerEvent( AnimationEvent.OnAttackPartB );
                        StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attacker, AnimationEvent.OnAttackPartB_Cutoff ) );

                        this.cameraEffect.Shake();
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                        yield return StartCoroutine( PlayCharacterAnimation( _loser, GETTING_HIT_ANIMATION_NAME, _attackDamage, _stressValueDamage, _statePointDamage ) );
                        yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
                        BattleLogicManager.OnCharacterAttackFinished( _attacker, _attackTarget );

                        if (CheckHasBattleEnded())
                        {
                            yield break;
                        }

                        if (_attacker.GetCurrentSkill().GetSkillData().skillType == Skill.SkillType.derived)
                        {
                            yield return StartCoroutine( RunDerivedSkill( _attacker, _attackTarget, battleFlowRound, _atlSlotListPanel ) );
                        }

                        if (CheckHasBattleEnded())
                        {
                            yield break;
                        }
                    }
                    else if (_winner == _attackTarget)
                    {
                        BattleLogicManager.ExecuteCasterSkillOnHit( _attacker, _attackTarget, false, false, false, false, out _, out _, out _, out _log );
                        BattleLog.Instance.AddOnScreenBattleLog( _log );

                        _skillCountdownTime = ( GetAttackAnimationLength( _attackTarget, _attackTargetBackendSkillAnimationCharacterPartA, _attackTargetBackendSkillAnimationSkillEffectPartA ) + 1.0f ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                        _attackTarget.SetSkillCountdownTime( _skillCountdownTime );
                        _attackTarget.TriggerEvent( AnimationEvent.OnDefenseWin );
                        StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnDefenseWin_Cutoff ) );

                        if (_attackTargetSubskillData.IsDefendingSkill)
                        {
                            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DEFEND );
                        }
                        else if (_attackTargetSubskillData.IsEvadingSkill)
                        {
                            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DODGE );
                        }

                        if (_attackTargetBackendSkillAnimationCharacterPartA != NO_ANIMATION)
                        {
                            yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, _attackTargetBackendSkillAnimationCharacterPartA ) );
                        }

                        if (_attackTargetBackendSkillAnimationSkillEffectPartA != NO_ANIMATION)
                        {
                            yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, _attackTargetBackendSkillAnimationSkillEffectPartA ) );
                        }

                        yield return new WaitForSeconds( 1.0f );
                        BattleLogicManager.OnCharacterAttackFinished( _attacker, _attackTarget );

                        if (_winner.GetCurrentSkill().GetSkillData().skillType == Skill.SkillType.counter
                            && !BattleLogicManager.HasGameCharacterReachedCounterAttackLimit( _winner ))
                        {
                            if (BattleLogicManager.IsAbleToUseSkill( _winner ))
                            {
                                _hasCounterAttack = true;
                                _winner.IncreaseCounterAttacks();
                            }
                            else
                            {
                                OnCasterBeingUnableToUseSkill( _winner );
                            }
                        }
                    }

                    break;
            }

            _attacker.GetOwnContainer().SetActive( false );
            _attacker.ShowCharacterObject();
            _attacker.PlayCharacterAnimation( IDLE_ANIMATION_NAME );
            _attackTarget.PlayCharacterAnimation( IDLE_ANIMATION_NAME );

            if (_hasCounterAttack)
            {
                _attacker = _winner;
                _attackTarget = _loser;
                _attackTarget.Reset();

                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_COUNTER );

                if (_attacker.GetIsPlayer())
                {
                    yield return StartCoroutine( PlayAnimation( skillEffectUiAnimator, "Player_Ariku_Counterattack" ) );
                }
                else
                {
                    yield return StartCoroutine( PlayAnimation( skillEffectUiAnimator, "Enemy_Enemy_Counterattack" ) );
                }
            }
            else
            {
                _attacker.Reset();
                _attackTarget.Reset();
            }
        }
        while ( _hasCounterAttack );
    }

    public IEnumerator RunBattleAnimationV2( BattleFlowRound_V2 battleFlowRound, BattleFlowATL_V2 battleFlowATL )
    {
        //ATLSlotListPanelV2 _atlSlotListPanel = this.battleGameManager.GetBattleUiManager().GetATLSlotListPanelV2();
        ATLSlotListPanelV3 _atlSlotListPanel = this.battleGameManager.GetBattleUiManager().GetATLSlotListPanelV3();

        BattleResultData _battleResultData = null;
        BattleResultData.BattleResultData_GameCharacter _attackerBattleResultData = null;
        BattleResultData.BattleResultData_GameCharacter _attackTargetBattleResultData = null;

        PlayerCharacter _playerCharacter = this.battleGameManager.GetPlayerCharacter();
        _playerCharacter.TriggerEvent( AnimationEvent.SetCharacter );
        _playerCharacter.MinusBreakStatusRemainingATLs();

        EnemyCharacter _enemyCharacter = this.battleGameManager.GetEnemyCharacter();
        _enemyCharacter.TriggerEvent( AnimationEvent.SetCharacter );
        _enemyCharacter.MinusBreakStatusRemainingATLs();

        if (BattleLogicManagerV2.ShouldCombatCommandTimeBeSkipped( _playerCharacter, _enemyCharacter ))
        {
            _atlSlotListPanel.GoToATL( battleFlowATL.GetATLNumber(), 0.1f );
        }
        else
        {
            this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COMBAT_COMMAND_TIME, true );
            this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COMBAT_COMMAND_TIME, false );
            BattleLog.Instance.AddOnScreenBattleLog( $"雙方進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.COMBAT_COMMAND_TIME } 】</color>。" );

            _playerCharacter.TriggerEvent( AnimationEvent.OnCombatCommandTimeStarted );
            _enemyCharacter.TriggerEvent( AnimationEvent.OnCombatCommandTimeStarted );

            battleFlowATL.StartAttackOpportunityCountdownTimer( this.skillPromptPanel );
            _atlSlotListPanel.GoToATL( battleFlowATL.GetATLNumber(), battleFlowATL.GetAttackOpportunityDuration() );
            yield return new WaitUntil( () => ( !battleFlowATL.GetIsDuringAttackOpportunityPeriod() || ( _playerCharacter.GetAssignedSkill() != null && _enemyCharacter.GetAssignedSkill() != null ) ) );
            _atlSlotListPanel.GoToMiddleAtCurrentAtlSlot( 0.1f );
            this.skillPromptPanel.HideCommandPhase( true );
            this.skillPromptPanel.HideCommandPhase( false );
        }

        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>判定先後手方</color>" );

        var ( _attacker, _attackTarget ) = BattleLogicManagerV2.DetermineLeadAndImproviser( _playerCharacter, _enemyCharacter );

        if (_attacker == null || _attackTarget == null)
        {
            BattleLog.Instance.AddOnScreenBattleLog( "沒有先後手方。當前 ATL 結束。" );
            yield break;
        }

        // 有“先手方”和“後手方”。

        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>判定結果</color>為"
                                                 + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attacker.GetCharacterName() }</color>成为<color={ BattleLog.SPECIAL_COLOR_CODE }>“先手方”</color>，"
                                                 + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attackTarget.GetCharacterName() }</color>成为<color={ BattleLog.SPECIAL_COLOR_CODE }>“后手方”</color>。" );

        // “先手方”發動技能。
        _attacker.ApplyAssignedSkillAsCurrentSkill();
        _attacker.PlayCharacterAnimation( "Idle" );
        _attackTarget.PlayCharacterAnimation( "Idle" );

        float _skillAnimationLength = 0.0f;
        float _skillCountdownTime = 0.0f;
        bool _isAbleToUseSkill = false;
        string _log = "";

        this.gameCharacterList = new List<GameCharacter>()
        {
            _attacker,
            _attackTarget
        };

        Subskill _attackerSubskillData = _attacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        SkillAnimation _skillAnimation = DatabaseManager.Instance.GetSkillAnimation( _attackerSubskillData.Id );
        RangeType _attackerRangeType = _attackerSubskillData.Range;

        string _attackerCharacterPartA = _skillAnimation.CharacterPartA;
        string _attackerCharacterPartB = _skillAnimation.CharacterPartB;
        string _attackerSkillEffectPartA = _skillAnimation.SkillEffectPartA;
        string _attackerSkillEffectPartB = _skillAnimation.SkillEffectPartB;

        _attacker.GetSortingGroup().sortingOrder = 3;
        _attackTarget.GetSortingGroup().sortingOrder = 1;

        if (_attacker.GetIsPlayer())
        {
            // TODO: Currently, the player character has ranged attack animations only.
            // TODO: Added a temporary fix here to handle the situation that the skill's range type is melee-or-ranged.
            _attackerRangeType = RangeType.ranged;
            ChangeToBackgroundPartA();
        }
        else
        {
            // TODO: Currently, the enemy character has melee attack animations only.
            _attackerRangeType = RangeType.melee;
            _attackerCharacterPartA = "Attack_Part_A";
            _attackerCharacterPartB = "Attack_Part_B";
            _attackerSkillEffectPartA = "-";
            _attackerSkillEffectPartB = "HittingEffect";

            ChangeToBackgroundPartB();
        }

        _attacker.GetOwnContainer().SetActive( true );
        _attacker.ShowCharacterObject();
        _attacker.GetOpponentContainer().SetActive( false );

        yield return new WaitForSeconds( 0.1f );

        _isAbleToUseSkill = BattleLogicManager.IsAbleToUseSkill( _attacker );

        if (_isAbleToUseSkill)
        {
            _attacker.TriggerEvent( AnimationEvent.OnSkillBeingUsed );

            _battleResultData = new BattleResultData();
            BattleLogicManagerV2.ExecuteCasterSkillOnUse( ref _battleResultData, _attacker, _attackTarget );

            _attackerBattleResultData = _battleResultData.GetGameCharacterResultData( _attacker );
            _attacker.ApplyBattleResultData( _attackerBattleResultData );

            //StartCoroutine( ShowPopUpDisplayInfo( _attacker, statePointReduced: _attackerBattleResultData.statePointCost, maximumStatePointIncreased: _attackerBattleResultData.maximumStatePointIncrease ) );
            _attacker.ShowPopUpDisplayInfoV2( maxStatePointUp: _attackerBattleResultData.maximumStatePointIncrease/*, statePointDamage: _attackerBattleResultData.statePointCost*/ );

            _attackTarget.SetCurrentAttacker( _attacker );
            this.currentCaster = _attacker;

            _attacker.TriggerEvent( AnimationEvent.OnPartA );
            _attackTarget.TriggerEvent( AnimationEvent.OnPartA );

            yield return StartCoroutine( PlayShowingSkillInformation( _attacker ) );

            Skill.SkillType _attackerSkillType = _attacker.GetCurrentSkill().GetSkillData().skillType;
            if (_attackerSkillType == Skill.SkillType.derived)
            {
                _battleResultData = BattleLogicManagerV2.DetermineResultForPartB( _attacker, _attackTarget, out _, out _ );
                _attackerBattleResultData = _battleResultData.GetGameCharacterResultData( _attacker );
                _attackTargetBattleResultData = _battleResultData.GetGameCharacterResultData( _attackTarget );

                yield return StartCoroutine( RunDerivedSkill( _attacker, _attackTarget, battleFlowRound, _atlSlotListPanel, _battleResultData, _attackerBattleResultData, _attackTargetBattleResultData ) );

                if (CheckHasBattleEnded())
                {
                    yield break;
                }

                _attacker.GetOwnContainer().SetActive( false );
                _attacker.ShowCharacterObject();
                _attacker.PlayCharacterAnimation( IDLE_ANIMATION_NAME );
                _attackTarget.PlayCharacterAnimation( IDLE_ANIMATION_NAME );

                _attacker.Reset();
                _attackTarget.Reset();

                yield break;
            }
            else if (_attackerSkillType == Skill.SkillType.counter)
            {
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_COUNTER );

                if (_attacker.GetIsPlayer())
                {
                    yield return StartCoroutine( PlayAnimation( skillEffectUiAnimator, "Player_Ariku_Counterattack" ) );
                }
                else
                {
                    yield return StartCoroutine( PlayAnimation( skillEffectUiAnimator, "Enemy_Enemy_Counterattack" ) );
                }
            }
            else
            {
                _attackTarget.SetIsInRepulseCommandTime( true );

                _skillAnimationLength = GetAttackAnimationLength( _attacker, _attackerCharacterPartA, _attackerSkillEffectPartA ) + 1.0f;
                _skillCountdownTime = _skillAnimationLength * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                _attackTarget.SetSkillCountdownTime( _skillCountdownTime );
                StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnDefensePartA_Cutoff ) );

                this.skillPromptPanel.ShowCommandPhase( TerminologyManager.REPULSE_COMMAND_TIME, _attackTarget.GetIsPlayer(), _skillCountdownTime );
                ShowCommandPhaseCountdownTimer( true, _attackTarget, _skillCountdownTime );
                BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attackTarget.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.REPULSE_COMMAND_TIME } 】</color>。" );
            }
        }
        else
        {
            OnCasterBeingUnableToUseSkill( _attacker );
        }

        yield return StartCoroutine( ZoomInCameraToTarget( _attacker, 1.0f ) );

        if (!_isAbleToUseSkill)
        {
            this.battleGameManager.GetBattleUiManager().ShowMessage( "<color=#FF0000>無法使用技能</color>" );
            yield return new WaitForSeconds( 1.2f );

            this.targetCamera.transform.position = cameraPosition;
            this.targetCamera.orthographicSize = cameraOrthographicSize;

            _attacker.Reset();
            _attackTarget.Reset();

            yield break;
        }

        if (_attackerCharacterPartA != NO_ANIMATION)
        {
            yield return StartCoroutine( PlayCharacterAnimation( _attacker, _attackerCharacterPartA ) );
        }

        if (_attackerSkillEffectPartA != NO_ANIMATION)
        {
            if (_attackerSkillEffectPartA == "Fireball_Part_A")
            {
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_FIREBALL );
            }

            yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartA ) );
        }

        // Hide the attacker for Part B if the attacker's range type is ranged.
        if (_attackerRangeType == RangeType.ranged)
        {
            _attacker.HideCharacterObject();
        }

        if (_attacker.GetIsPlayer())
        {
            ChangeToBackgroundPartB();
        }
        else
        {
            ChangeToBackgroundPartA();
        }

        this.targetCamera.transform.position = cameraPosition;
        this.targetCamera.orthographicSize = cameraOrthographicSize;

        _attacker.GetOpponentContainer().SetActive( true );
        _attackTarget.ShowCharacterObject();

        // “後手方”發動技能。
        _attackTarget.ApplyAssignedSkillAsCurrentSkill();

        float _attackDamage = 0;
        float _stressValueDamage = 0;
        float _statePointDamage = 0;

        if (_attackTarget.GetIsInBreakStatus())
        {
            _attackTarget.Reset();
        }

        Skill.SkillType _attackTargetSkillType = Skill.SkillType.none;
        Subskill _attackTargetSubskillData = null;

        // “後手方”有已按下的技能。
        if (_attackTarget.GetCurrentSkill() != null)
        {
            _attackTargetSkillType = _attackTarget.GetCurrentSkill().GetSkillData().skillType;
            _attackTargetSubskillData = _attackTarget.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

            if (_attackTargetSkillType != Skill.SkillType.none)
            {
                if (BattleLogicManager.IsAbleToUseSkill( _attackTarget ))
                {
                    if (_attackTargetSkillType == Skill.SkillType.backend)
                    {
                        if (!_attackTargetSubskillData.IsDefendingSkill && !_attackTargetSubskillData.IsEvadingSkill)
                        {
                            _attackTargetSkillType = Skill.SkillType.none;
                        }
                    }
                }
                else
                {
                    this.battleGameManager.GetBattleUiManager().ShowMessage( "<color=#FF0000>無法使用技能</color>" );
                    _attackTargetSkillType = Skill.SkillType.none;
                    OnCasterBeingUnableToUseSkill( _attackTarget );
                }
            }
        }

        // 判定 Part B 結果及結算。
        _battleResultData = BattleLogicManagerV2.DetermineResultForPartB( _attacker, _attackTarget, out GameCharacter _winner, out GameCharacter _loser );
        _attackerBattleResultData = _battleResultData.GetGameCharacterResultData( _attacker );
        _attackTargetBattleResultData = _battleResultData.GetGameCharacterResultData( _attackTarget );

        // 結算“後手方”已按下的技能的以太值和最大以太值提升。
        _attackTarget.TriggerEvent( AnimationEvent.OnSkillBeingUsed );
        //StartCoroutine( ShowPopUpDisplayInfo( _attackTarget, statePointReduced: _attackTargetBattleResultData.statePointCost, maximumStatePointIncreased: _attackTargetBattleResultData.maximumStatePointIncrease ) );
        _attackTarget.ShowPopUpDisplayInfoV2( maxStatePointUp: _attackTargetBattleResultData.maximumStatePointIncrease/*, statePointDamage: _attackTargetBattleResultData.statePointCost*/ );
        this.currentCaster = _attackTarget;

        this.battleGameManager.GetBattleVisualEffectManager().ApplyBlurShaderAtPartB();
        _attacker.TriggerEvent( AnimationEvent.OnPartB );
        _attackTarget.TriggerEvent( AnimationEvent.OnPartB );

        this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COMBAT_COMMAND_TIME, _attacker.GetIsPlayer() );
        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attacker.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.COMBAT_COMMAND_TIME } 】</color>。" );

        if (_attackTarget.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.SuccessfulResister
            || _attackTarget.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.SuccessfulDefender
            || _attackTarget.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.SuccessfulEvader)
        {
            this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COUNTER_COMMAND_TIME, _attackTarget.GetIsPlayer() );
            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attackTarget.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.COUNTER_COMMAND_TIME } 】</color>。" );
        }
        else
        {
            this.skillPromptPanel.ShowCommandPhase( TerminologyManager.COMBAT_COMMAND_TIME, _attackTarget.GetIsPlayer() );
            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attackTarget.GetCharacterName() }</color>進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 { TerminologyManager.COMBAT_COMMAND_TIME } 】</color>。" );
        }

        switch ( _attackTargetSkillType )
        {
            case Skill.SkillType.none:

                _skillCountdownTime = GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();

                _attacker.SetSkillCountdownTime( _skillCountdownTime );
                //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attacker, AnimationEvent.OnAttackPartB_Cutoff ) );
                //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                ShowCommandPhaseCountdownTimer( true, _attacker, _skillCountdownTime );
                ShowCommandPhaseCountdownTimer( true, _attackTarget, _skillCountdownTime );
                _atlSlotListPanel.GoToEndAtCurrentAtlSlot( _skillCountdownTime );

                if (_attackerCharacterPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlayCharacterAnimation( _attacker, _attackerCharacterPartB ) );
                }

                if (_attackerSkillEffectPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartB ) );
                }

                _attacker.ApplyBattleResultData( _attackerBattleResultData );
                _attackTarget.ApplyBattleResultData( _attackTargetBattleResultData );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME, _attackTargetBattleResultData ) );
                yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );

                BattleLogicManager.OnCharacterAttackFinished( _attacker, _attackTarget );

                break;

            case Skill.SkillType.repulse:

                yield return StartCoroutine( PlayShowingSkillInformation( _attackTarget ) );

                if (_attackerCharacterPartB != NO_ANIMATION)
                {
                    StartCoroutine( PlayCharacterAnimation( _attacker, _attackerCharacterPartB + "_" + REPULSE_ANIMATION_NAME ) );
                }

                if (_attackerSkillEffectPartB != NO_ANIMATION)
                {
                    StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartB + "_" + REPULSE_ANIMATION_NAME ) );
                }

                _skillCountdownTime = ( GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                ShowCommandPhaseCountdownTimer( true, _attacker, _skillCountdownTime );
                ShowCommandPhaseCountdownTimer( true, _attackTarget, _skillCountdownTime );
                _atlSlotListPanel.GoToEndAtCurrentAtlSlot( _skillCountdownTime );

                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );
                yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );

                _attacker.ApplyBattleResultData( _attackerBattleResultData );
                _attackTarget.ApplyBattleResultData( _attackTargetBattleResultData );

                bool _hasAssault = false;
                if (_winner != null)
                {
                    if (_winner.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.LightAssaulter
                        || _winner.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.HeavyAssaulter)
                    {
                        _hasAssault = true;

                        _skillCountdownTime = 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                        _winner.SetSkillCountdownTime( _skillCountdownTime );
                        //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _winner, AnimationEvent.OnRepulseWin_Cutoff ) );

                        this.cameraEffect.Shake();
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );

                        string _animationName = GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_" + ( ( _attacker.GetIsPlayer() ) ? "Left" : "Right" );

                        ShowPopUpDisplayInfo( _attacker, _attackerBattleResultData );
                        ShowPopUpDisplayInfo( _attackTarget, _attackTargetBattleResultData );

                        if (_winner == _attacker)
                        {
                            yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, _animationName, _attackTargetBattleResultData ) );
                        }
                        else if (_winner == _attackTarget)
                        {
                            yield return StartCoroutine( PlayCharacterAnimation( _attacker, _animationName, _attackTargetBattleResultData ) );
                        }
                    }
                }

                if (!_hasAssault)
                {
                    ShowPopUpDisplayInfo( _attacker, _attackerBattleResultData );
                    ShowPopUpDisplayInfo( _attackTarget, _attackTargetBattleResultData );
                }

                yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
                BattleLogicManager.OnCharacterAttackFinished( _attacker, _attackTarget );

                break;

            case Skill.SkillType.backend:

                yield return StartCoroutine( PlayShowingSkillInformation( _attackTarget ) );

                _skillCountdownTime = ( GetAttackAnimationLength( _attacker, _attackerCharacterPartB, _attackerSkillEffectPartB ) ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnActiveSkillFinished ) );

                ShowCommandPhaseCountdownTimer( true, _attacker, _skillCountdownTime );
                ShowCommandPhaseCountdownTimer( true, _attackTarget, _skillCountdownTime );
                _atlSlotListPanel.GoToEndAtCurrentAtlSlot( _skillCountdownTime );

                if (_attackerCharacterPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlayCharacterAnimation( _attacker, _attackerCharacterPartB ) );
                }

                if (_attackerSkillEffectPartB != NO_ANIMATION)
                {
                    if (_attackerSkillEffectPartB != "HittingEffect")
                    {
                        yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartB ) );
                    }
                }

                BattleLog.Instance.AddOnScreenBattleLog( _log );

                SkillAnimation _attackTargetBackendSkillAnimation = DatabaseManager.Instance.GetSkillAnimation( _attackTargetSubskillData.Id );
                string _attackTargetBackendSkillAnimationCharacterPartA = _attackTargetBackendSkillAnimation.CharacterPartA;
                string _attackTargetBackendSkillAnimationSkillEffectPartA = _attackTargetBackendSkillAnimation.SkillEffectPartA;

                _attacker.ApplyBattleResultData( _attackerBattleResultData );
                _attackTarget.ApplyBattleResultData( _attackTargetBattleResultData );

                if (_winner == _attacker)
                {
                    if (_attackerSkillEffectPartB == "HittingEffect")
                    {
                        yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _attackerSkillEffectPartB ) );
                    }

                    _skillCountdownTime = 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                    _attacker.SetSkillCountdownTime( _skillCountdownTime );
                    //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attacker, AnimationEvent.OnAttackPartB_Cutoff ) );

                    this.cameraEffect.Shake();
                    AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                    yield return StartCoroutine( PlayCharacterAnimation( _loser, GETTING_HIT_ANIMATION_NAME, _attackDamage, _stressValueDamage, _statePointDamage ) );
                    yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
                    BattleLogicManager.OnCharacterAttackFinished( _attacker, _attackTarget );
                }
                else if (_winner == _attackTarget)
                {
                    _skillCountdownTime = ( GetAttackAnimationLength( _attackTarget, _attackTargetBackendSkillAnimationCharacterPartA, _attackTargetBackendSkillAnimationSkillEffectPartA ) + 1.0f ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                    _attackTarget.SetSkillCountdownTime( _skillCountdownTime );
                    //StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnDefenseWin_Cutoff ) );

                    if (_attackTargetSubskillData.IsDefendingSkill)
                    {
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DEFEND );
                    }
                    else if (_attackTargetSubskillData.IsEvadingSkill)
                    {
                        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DODGE );
                    }

                    if (_attackTargetBackendSkillAnimationCharacterPartA != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, _attackTargetBackendSkillAnimationCharacterPartA ) );
                    }

                    if (_attackTargetBackendSkillAnimationSkillEffectPartA != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, _attackTargetBackendSkillAnimationSkillEffectPartA ) );
                    }

                    yield return new WaitForSeconds( 1.0f );
                    BattleLogicManager.OnCharacterAttackFinished( _attacker, _attackTarget );
                }

                break;
        }

        this.battleGameManager.GetBattleVisualEffectManager().TurnOffBlurShader();

        if (CheckHasBattleEnded())
        {
            yield break;
        }

        _attacker.GetOwnContainer().SetActive( false );
        _attacker.ShowCharacterObject();
        _attacker.PlayCharacterAnimation( IDLE_ANIMATION_NAME );
        _attackTarget.PlayCharacterAnimation( IDLE_ANIMATION_NAME );

        _attacker.Reset();
        _attackTarget.Reset();

        if (battleFlowRound.GetCurrentATL().GetATLNumber() == GameConfiguration.Instance.GetBattleConfiguration().GetNumberOfATLSlots())
        {
            CharacterSkill _attackerAssignedSkill = _attacker.GetAssignedSkill();
            if (_attackerAssignedSkill != null)
            {
                if (_attackerAssignedSkill.GetSkillData().skillType == Skill.SkillType.derived)
                {
                    battleFlowRound.AddExtraATL();
                }
            }

            CharacterSkill _attackTargetAssignedSkill = _attackTarget.GetAssignedSkill();
            if (_attackTargetAssignedSkill != null)
            {
                if (_attackTargetAssignedSkill.GetSkillData().skillType == Skill.SkillType.derived)
                {
                    battleFlowRound.AddExtraATL();
                }
            }
        }
    }

    private void ShowCommandPhaseCountdownTimer( bool isActiveSkill, GameCharacter gameCharacter, float countdownTime )
    {
        StartCoroutine( RunCommandPhaseCountdownTimer( isActiveSkill, gameCharacter, countdownTime ) );
    }

    private IEnumerator RunCommandPhaseCountdownTimer( bool isActiveSkill, GameCharacter gameCharacter, float countdownTime )
    {
        bool _isPlayer = gameCharacter.GetIsPlayer();

        this.skillPromptPanel.SetCommandPhaseProgressBar( 1.0f, isActiveSkill, _isPlayer );

        float _startTime = Time.realtimeSinceStartup;
        float _remainingTime = 0.0f;

        do
        {
            yield return null;

            _remainingTime = countdownTime - ( Time.realtimeSinceStartup - _startTime );

            float _remainingTimePercentage = _remainingTime / countdownTime;
            skillPromptPanel.SetCommandPhaseProgressBar( _remainingTimePercentage, isActiveSkill, _isPlayer );
        }
        while (_remainingTime > 0);

        gameCharacter.SetIsInRepulseCommandTime( false );
        this.skillPromptPanel.HideCommandPhase( _isPlayer );
    }

    //private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, BattleFlowRound battleFlowRound, ATLSlotListPanelV2 atlSlotListPanel )
    private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, BattleFlowRound battleFlowRound, ATLSlotListPanelV3 atlSlotListPanel )
    {
        attackTarget.Reset();
        attackTarget.PlayCharacterAnimation( IDLE_ANIMATION_NAME );

        bool _isAbleToUseSkill = BattleLogicManager.IsAbleToUseSkill( attacker );

        string _log = "";
        BattleFlowATL _targetATL = null;

        if (_isAbleToUseSkill)
        {
            _targetATL = battleFlowRound.GetNextATL( attacker );
            battleFlowRound.GoToTargetATL( _targetATL, false );
            BattleLogicManager.ExecuteCasterSkillOnUse( attacker, attackTarget, out _log );
            ShowSkillInfo( attacker, attackTarget );
            this.currentCaster = attacker;

            BattleLog.Instance.AddOnScreenBattleLog( _log );

            yield return StartCoroutine( RunDerivedSkill( attacker, attackTarget, atlSlotListPanel, _targetATL.GetATLNumber(), _isAbleToUseSkill ) );
        }
    }

    //private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, BattleFlowRound_V2 battleFlowRound, ATLSlotListPanelV2 atlSlotListPanel,
    private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, BattleFlowRound_V2 battleFlowRound, ATLSlotListPanelV3 atlSlotListPanel,
                                         BattleResultData battleResultData, BattleResultData.BattleResultData_GameCharacter attackerBattleResultData, BattleResultData.BattleResultData_GameCharacter attackTargetBattleResultData )
    {
        attackTarget.Reset();
        attackTarget.PlayCharacterAnimation( IDLE_ANIMATION_NAME );

        bool _isAbleToUseSkill = BattleLogicManager.IsAbleToUseSkill( attacker );

        if (_isAbleToUseSkill)
        {
            BattleResultData _battleResultData = new BattleResultData();
            BattleLogicManagerV2.ExecuteCasterSkillOnUse( ref _battleResultData, attacker, attackTarget );

            BattleResultData.BattleResultData_GameCharacter _attackerBattleResultData = _battleResultData.GetGameCharacterResultData( attacker );
            attacker.ApplyBattleResultData( _attackerBattleResultData );

            //StartCoroutine( ShowPopUpDisplayInfo( attacker, statePointReduced: _attackerBattleResultData.statePointCost, maximumStatePointIncreased: _attackerBattleResultData.maximumStatePointIncrease ) );
            attacker.ShowPopUpDisplayInfoV2(/* statePointDamage: _attackerBattleResultData.statePointCost,*/ maxStatePointUp: _attackerBattleResultData.maximumStatePointIncrease );

            ShowSkillInfo( attacker, attackTarget );
            this.currentCaster = attacker;
        }

        yield return StartCoroutine( RunDerivedSkill( attacker, attackTarget, atlSlotListPanel, battleFlowRound.GetCurrentATL().GetATLNumber(), battleResultData, attackerBattleResultData, attackTargetBattleResultData ) );
    }

    //private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, ATLSlotListPanelV2 atlSlotListPanel, int atlNumber, bool isAbleToUseSkill )
    private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, ATLSlotListPanelV3 atlSlotListPanel, int atlNumber, bool isAbleToUseSkill )
    {
        attacker.GetSortingGroup().sortingOrder = 3;
        attackTarget.GetSortingGroup().sortingOrder = 1;

        float _attackDamage = 0;
        float _stressValueDamage = 0;
        float _statePointDamage = 0;
        string _log = "";

        CharacterSkill _attackerSkill = attacker.GetCurrentSkill();
        Subskill _attackerSubskillData = _attackerSkill.GetCharacterSubskillData().GetSubskillData();
        RangeType _attackerRangeType = _attackerSubskillData.Range;

        if (attacker is EnemyCharacter)
        {
            _attackerRangeType = RangeType.melee;
        }

        if (_attackerRangeType == RangeType.melee)
        {
            if (isAbleToUseSkill)
            {
                yield return StartCoroutine( RunMeleeDerivedSkillAnimation( attacker, _attackerSkill, DatabaseManager.Instance.GetSkillAnimation( _attackerSubskillData.Id ), atlSlotListPanel, atlNumber ) );

                BattleLogicManager.ExecuteCasterSkillOnHit( attacker, attackTarget, true, out _attackDamage, out _stressValueDamage, out _statePointDamage, out _log );
                BattleLog.Instance.AddOnScreenBattleLog( _log );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return StartCoroutine( PlayCharacterAnimation( attackTarget, GETTING_HIT_ANIMATION_NAME, _attackDamage, _stressValueDamage, _statePointDamage ) );
            }
        }
        else
        {
            attacker.ShowCharacterObject();
            ChangeToBackgroundPartA();
            attacker.GetOpponentContainer().SetActive( false );

            if (isAbleToUseSkill)
            {
                yield return StartCoroutine( RunRangedDerivedSkillAnimation( attacker, _attackerSkill, attackTarget, atlSlotListPanel, atlNumber ) );
                BattleLogicManager.ExecuteCasterSkillOnHit( attacker, attackTarget, true, out _attackDamage, out _stressValueDamage, out _statePointDamage, out _log );
                BattleLog.Instance.AddOnScreenBattleLog( _log );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return null;
                //StartCoroutine( ShowPopUpDisplayInfo( attackTarget, _attackDamage, _stressValueDamage, _statePointDamage ) );
                attackTarget.ShowPopUpDisplayInfoV2( healthPointDamage: _attackDamage, stressValueDamage: _stressValueDamage, statePointDamage: _statePointDamage );
                yield return new WaitForSeconds( 0.7f );
            }
        }

        if (isAbleToUseSkill)
        {
            yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
        }
        else
        {
            OnCasterBeingUnableToUseSkill( attacker );
            yield return new WaitForSeconds( 1.0f );
        }
    }

    //private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, ATLSlotListPanelV2 atlSlotListPanel, int atlNumber,
    private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, ATLSlotListPanelV3 atlSlotListPanel, int atlNumber,
                                         BattleResultData battleResultData, BattleResultData.BattleResultData_GameCharacter attackerBattleResultData, BattleResultData.BattleResultData_GameCharacter attackTargetBattleResultData )
    {
        attacker.GetSortingGroup().sortingOrder = 3;
        attackTarget.GetSortingGroup().sortingOrder = 1;

        CharacterSkill _attackerSkill = attacker.GetCurrentSkill();
        Subskill _attackerSubskillData = _attackerSkill.GetCharacterSubskillData().GetSubskillData();
        RangeType _attackerRangeType = _attackerSubskillData.Range;

        if (attacker is EnemyCharacter)
        {
            _attackerRangeType = RangeType.melee;
        }

        if (_attackerRangeType == RangeType.melee)
        {
            if (battleResultData != null)
            {
                yield return StartCoroutine( RunMeleeDerivedSkillAnimation( attacker, _attackerSkill, DatabaseManager.Instance.GetSkillAnimation( _attackerSubskillData.Id ), atlSlotListPanel, atlNumber ) );

                attacker.ApplyBattleResultData( attackerBattleResultData );
                attackTarget.ApplyBattleResultData( attackTargetBattleResultData );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return StartCoroutine( PlayCharacterAnimation( attackTarget, GETTING_HIT_ANIMATION_NAME, attackTargetBattleResultData ) );
            }
        }
        else
        {
            attacker.ShowCharacterObject();
            ChangeToBackgroundPartA();
            attacker.GetOpponentContainer().SetActive( false );

            if (battleResultData != null)
            {
                yield return StartCoroutine( RunRangedDerivedSkillAnimation( attacker, _attackerSkill, attackTarget, atlSlotListPanel, atlNumber ) );

                attacker.ApplyBattleResultData( attackerBattleResultData );
                attackTarget.ApplyBattleResultData( attackTargetBattleResultData );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return null;
                ShowPopUpDisplayInfo( attackTarget, attackTargetBattleResultData );
                yield return new WaitForSeconds( 0.7f );
            }
        }

        if (battleResultData != null)
        {
            yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
        }
        else
        {
            OnCasterBeingUnableToUseSkill( attacker );
            yield return new WaitForSeconds( 1.0f );
        }
    }

    //private IEnumerator RunMeleeDerivedSkillAnimation( GameCharacter attacker, CharacterSkill attackerSkill, SkillAnimation skillAnimation, ATLSlotListPanelV2 atlSlotListPanel, int atlNumber )
    private IEnumerator RunMeleeDerivedSkillAnimation( GameCharacter attacker, CharacterSkill attackerSkill, SkillAnimation skillAnimation, ATLSlotListPanelV3 atlSlotListPanel, int atlNumber )
    {
        string _characterPartB = skillAnimation.CharacterPartB;
        string _skillEffectPartB = skillAnimation.SkillEffectPartB;

        if (attacker.GetIsPlayer())
        {
            ChangeToBackgroundPartB();
        }
        else
        {
            _characterPartB = "Attack_Part_B";
            _skillEffectPartB = "HittingEffect";

            ChangeToBackgroundPartA();
        }

        attacker.GetOpponentContainer().SetActive( true );
        atlSlotListPanel.GoToATL( atlNumber, GetAttackAnimationLength( attacker, _characterPartB, _skillEffectPartB ), attackerSkill );

        yield return StartCoroutine( PlayShowingSkillInformation( attacker ) );

        if (_characterPartB != NO_ANIMATION)
        {
            yield return StartCoroutine( PlayCharacterAnimation( attacker, _characterPartB ) );
        }

        if (_skillEffectPartB != NO_ANIMATION)
        {
            yield return StartCoroutine( PlaySkillEffectAnimation( attacker, _skillEffectPartB ) );
        }
    }

    //private IEnumerator RunRangedDerivedSkillAnimation( GameCharacter attacker, CharacterSkill attackerSkill, GameCharacter attackTarget, ATLSlotListPanelV2 atlSlotListPanel, int atlNumber )
    private IEnumerator RunRangedDerivedSkillAnimation( GameCharacter attacker, CharacterSkill attackerSkill, GameCharacter attackTarget, ATLSlotListPanelV3 atlSlotListPanel, int atlNumber )
    {
        atlSlotListPanel.GoToATL( atlNumber, 4.5f, attackerSkill );

        yield return StartCoroutine( PlayShowingSkillInformation( attacker ) );
        yield return StartCoroutine( PlayCharacterAnimation( attacker, "Attack" ) );
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_FIREBALL );
        yield return StartCoroutine( PlaySkillEffectAnimation( attacker, DERIVE_ANIMATION_NAME + "_Part_A" ) );
        attacker.HideCharacterObject();

        ChangeToBackgroundPartB();
        attacker.GetOpponentContainer().SetActive( true );
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
        StartCoroutine( PlayCharacterAnimation( attackTarget, GETTING_HIT_ANIMATION_NAME ) );
        yield return StartCoroutine( PlaySkillEffectAnimation( attacker, DERIVE_ANIMATION_NAME + "_Part_B" ) );
        attacker.ShowCharacterObject();

        ChangeToBackgroundPartA();
        attacker.GetOpponentContainer().SetActive( false );
        yield return StartCoroutine( PlayCharacterAnimation( attacker, DERIVE_ANIMATION_NAME ) );
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_FIREBALL );
        yield return StartCoroutine( PlaySkillEffectAnimation( attacker, DERIVE_ANIMATION_NAME + "_Part_C" ) );
        attacker.HideCharacterObject();

        ChangeToBackgroundPartB();
        attacker.GetOpponentContainer().SetActive( true );
        StartCoroutine( PlayCharacterAnimation( attackTarget, GETTING_HIT_ANIMATION_NAME ) );
        yield return StartCoroutine( PlaySkillEffectAnimation( attacker, DERIVE_ANIMATION_NAME + "_Part_D" ) );
    }

    private void OnHitWithNoDamage( GameCharacter caster, GameCharacter target, bool isBreakStatusAvailable )
    {
        float _damageTaken = 0;
        float _stressValueIncreased = 0;
        float _statePointReduced = 0;

        string _log = "";
        BattleLogicManager.ExecuteCasterSkillOnHit( caster, target, false, true, true, isBreakStatusAvailable, out _damageTaken, out _stressValueIncreased, out _statePointReduced, out _log );
        BattleLog.Instance.AddOnScreenBattleLog( _log );

        //StartCoroutine( ShowPopUpDisplayInfo( target, _damageTaken, _stressValueIncreased, _statePointReduced ) );
        target.ShowPopUpDisplayInfoV2( healthPointDamage: _damageTaken, stressValueDamage: _stressValueIncreased, statePointDamage: _statePointReduced );
    }

    private void OnCasterBeingUnableToUseSkill( GameCharacter caster )
    {
        caster.SetIsAbleToUseSkill( false );

        CharacterSkill _casterSkill = caster.GetCurrentSkill();
        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ caster.GetCharacterName() }</color>"
                                                 + $"因當前<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.STATE_POINT }</color>"
                                                 + $"為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ GameConfiguration.Instance.GetBattleConfiguration().GetMinimumCurrentStatePoint() }</color>"
                                                 + $"而無法使用<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _casterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName }</color>"
                                                 + $"（{ TerminologyManager.GetSkillTypeText( _casterSkill.GetSkillData().skillType ) }）。" );
    }

    private IEnumerator PlayCharacterAnimation( GameCharacter gameCharacter, string animationName, BattleResultData.BattleResultData_GameCharacter battleResultData )
    {
        float _damageTaken = 0.0f;
        if (battleResultData.actualHealthPointDamage > 0)
        {
            _damageTaken = battleResultData.actualHealthPointDamage;
        }
        else if (battleResultData.virtualHealthPointDamage > 0)
        {
            _damageTaken = battleResultData.virtualHealthPointDamage;
        }

        yield return StartCoroutine( PlayCharacterAnimation( gameCharacter, animationName, _damageTaken, battleResultData.stressValueDamage, battleResultData.statePointDamage ) );
    }

    private IEnumerator PlayCharacterAnimation( GameCharacter gameCharacter, string animationName,
                                                float damageTaken = 0, float stressValueIncreased = 0, float statePointReduced = 0 )
    {
        gameCharacter.PlayCharacterAnimation( animationName, OnAnimationEventTriggered );

        if (animationName.Contains( GETTING_HIT_ANIMATION_NAME ))
        {
            //StartCoroutine( ShowPopUpDisplayInfo( gameCharacter, damageTaken, stressValueIncreased, statePointReduced ) );
            gameCharacter.ShowPopUpDisplayInfoV2( healthPointDamage: damageTaken, stressValueDamage: stressValueIncreased, statePointDamage: statePointReduced );
        }

        yield return StartCoroutine( WaitForAnimationEventTriggered() );
    }

    private void ShowPopUpDisplayInfo( GameCharacter gameCharacter, BattleResultData.BattleResultData_GameCharacter battleResultData )
    {
        float _damageTaken = 0.0f;
        if (battleResultData.actualHealthPointDamage > 0)
        {
            _damageTaken = battleResultData.actualHealthPointDamage;
        }
        else if (battleResultData.virtualHealthPointDamage > 0)
        {
            _damageTaken = battleResultData.virtualHealthPointDamage;
        }

        //StartCoroutine( ShowPopUpDisplayInfo( gameCharacter, _damageTaken, battleResultData.stressValueDamage, battleResultData.statePointDamage ) );
        gameCharacter.ShowPopUpDisplayInfoV2( healthPointDamage: _damageTaken, stressValueDamage: battleResultData.stressValueDamage, statePointDamage: battleResultData.statePointDamage );
    }

    private IEnumerator ShowPopUpDisplayInfo( GameCharacter gameCharacter,
                                              float damageTaken = 0, float stressValueIncreased = 0, float statePointReduced = 0, float maximumStatePointIncreased = 0 )
    {
        if (damageTaken > 0)
        {
            yield return null;
            gameCharacter.ShowPopUpDisplayInfo( "-" + damageTaken.ToString() + " HP", Color.red );
        }

        if (statePointReduced > 0)
        {
            yield return new WaitForSeconds( 0.5f );
            gameCharacter.ShowPopUpDisplayInfo( "-" + statePointReduced.ToString() + " SP", Color.cyan );
        }

        if (stressValueIncreased > 0)
        {
            yield return new WaitForSeconds( 0.5f );
            gameCharacter.ShowPopUpDisplayInfo( "+" + stressValueIncreased.ToString() + " SV", Color.magenta );
        }

        if (maximumStatePointIncreased > 0)
        {
            yield return new WaitForSeconds( 0.5f );
            gameCharacter.ShowPopUpDisplayInfo( "+" + maximumStatePointIncreased.ToString() + " MaxSP", Color.yellow );
        }
    }

    private IEnumerator PlaySkillEffectAnimation( GameCharacter gameCharacter, string animationName )
    {
        gameCharacter.PlaySkillEffectAnimation( animationName, OnAnimationEventTriggered );
        yield return StartCoroutine( WaitForAnimationEventTriggered() );
    }

    private IEnumerator PlayAnimation( Animator animator, string animationName )
    {
        animator.Play( animationName );
        yield return StartCoroutine( WaitForAnimationEventTriggered() );
    }

    private IEnumerator WaitForAnimationEventTriggered()
    {
        this.isAnimationEventTriggered = false;
        yield return new WaitUntil( () => this.isAnimationEventTriggered );
    }

    private IEnumerator WaitForPopUpDisplayInfoCompleted()
    {
        /*
        yield return new WaitForSeconds( 0.5f );

        yield return new WaitWhile( () =>
        {
            for (int i = 0; i < this.gameCharacterList.Count; i++)
            {
                if (this.gameCharacterList[i].HasPopUpDisplayInfo())
                {
                    return true;
                }
            }

            return false;
        } );
        */

        yield return new WaitForSeconds( 1.2f );
    }

    private IEnumerator ZoomInCameraToTarget( GameCharacter target, float duration )
    {
        float _cameraX = 0.0f;
        float _cameraY = 0.0f;
        float _cameraSize = 0.0f;

        if (target.GetIsPlayer())
        {
            _cameraX = 2.0f;
            _cameraY = -1.2f;
            _cameraSize = 3.5f;
        }
        else
        {
            _cameraX = -2.0f;
            _cameraY = -1.7f;
            _cameraSize = 4.0f;
        }

        LeanTween.moveX( this.targetCameraObject, _cameraX, duration ).setEaseOutCirc();
        LeanTween.moveY( this.targetCameraObject, _cameraY, duration ).setEaseOutCirc();
        LeanTween.value( this.targetCamera.orthographicSize, _cameraSize, duration ).setEaseOutCirc().setOnUpdate( ( float value ) => { this.targetCamera.orthographicSize = value; } );

        yield return new WaitForSeconds( duration );
    }

    private IEnumerator ZoomOutCameraToOriginal( float duration )
    {
        LeanTween.moveX( this.targetCameraObject, cameraPosition.x, duration ).setEaseOutCirc();
        LeanTween.moveY( this.targetCameraObject, cameraPosition.y, duration ).setEaseOutCirc();
        LeanTween.value( this.targetCamera.orthographicSize, cameraOrthographicSize, duration ).setEaseOutCirc().setOnUpdate( ( float value ) => { this.targetCamera.orthographicSize = value; } );

        yield return new WaitForSeconds( duration );
    }

    private IEnumerator FadeDarkLayer( float darkness, float duration )
    {
        LeanTween.value( 0.0f, darkness, duration * 0.8f ).setEaseOutCirc().setOnUpdate( SetDarkLayerAlphaValue ).setOnComplete( () =>
        {
            LeanTween.value( darkness, 0.0f, duration * 0.2f ).setEaseOutCirc().setOnUpdate( SetDarkLayerAlphaValue );
        } );

        yield return new WaitForSeconds( duration );
    }

    private void SetDarkLayerAlphaValue( float value )
    {
        Color _color = this.darkLayer.color;
        _color.a = value;
        this.darkLayer.color = _color;
    }

    private bool CheckHasBattleEnded()
    {
        bool _hasPlayerCharacterSurvived = false;
        bool _hasEnemyCharacterSurvived = false;
        List<PlayerCharacter> _playerCharacterList = this.battleGameManager.GetPlayerCharacterList();
        List<EnemyCharacter> _enemyCharacterList = this.battleGameManager.GetEnemyCharacterList();

        for (int i = 0; i < _playerCharacterList.Count; i++)
        {
            PlayerCharacter _playerCharacter = _playerCharacterList[ i ];
            if (BattleLogicManager.IsGameCharacterDead( _playerCharacter ))
            {
                if (_playerCharacter.gameObject.activeSelf)
                {
                    _playerCharacter.gameObject.SetActive( false );
                    BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _playerCharacter.GetCharacterName() }</color>被擊倒了。" );
                }
            }
            else
            {
                _hasPlayerCharacterSurvived = true;
                break;
            }
        }

        for (int i = 0; i < _enemyCharacterList.Count; i++)
        {
            EnemyCharacter _enemyCharacter = _enemyCharacterList[ i ];
            if (BattleLogicManager.IsGameCharacterDead( _enemyCharacter ))
            {
                if (_enemyCharacter.gameObject.activeSelf)
                {
                    _enemyCharacter.gameObject.SetActive( false );
                    BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _enemyCharacter.GetCharacterName() }</color>被擊倒了。" );
                }
            }
            else
            {
                _hasEnemyCharacterSurvived = true;
                break;
            }
        }

        // Victory
        if (_hasPlayerCharacterSurvived && !_hasEnemyCharacterSurvived)
        {
            this.onBattleEndedCallback?.Invoke( true );
            return true;
        }

        // Defeat
        if (!_hasPlayerCharacterSurvived && _hasEnemyCharacterSurvived)
        {
            this.onBattleEndedCallback?.Invoke( false );
            return true;
        }

        return false;
    }

    private IEnumerator PlayShowingSkillInformation( GameCharacter caster )
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HINTS );
        this.skillPromptPanel.ShowCasterCurrentSkillInfo( caster );

        if (CheckHasTimeStop( caster ))
        {
            yield return StartCoroutine( FadeDarkLayer( this.skillTimeStopDarkness, this.backgroundDarknessDuration ) );
        }
        else
        {
            StartCoroutine( FadeDarkLayer( this.skillNormalDarkness, this.backgroundDarknessDuration ) );
        }
    }

    private bool CheckHasTimeStop( GameCharacter caster )
    {
        CharacterSkill _skill = caster.GetCurrentSkill();
        Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();
        int _skillStatIncrement = caster.GetCurrentSkillStatIncrement();

        if (_subskillData.Speed + _skillStatIncrement >= CharacterSkill.SPEED_MINIMUM_SPECIAL_VALUE)
        {
            return true;
        }

        if (_subskillData.Strength + _skillStatIncrement >= CharacterSkill.STRENGTH_MINIMUM_SPECIAL_VALUE)
        {
            return true;
        }

        if (_subskillData.Accuracy + _skillStatIncrement > 1)
        {
            return true;
        }

        if (_subskillData.Evasion + _skillStatIncrement > 1)
        {
            return true;
        }

        if (_subskillData.EffectType == Subskill.EffectTypeEnum.wide)
        {
            return true;
        }

        return false;
    }

    private float GetAttackAnimationLength( GameCharacter attacker, string characterAnimationName, string skillEffectAnimationName )
    {
        float _attackAnimationLength = 0;
        if (characterAnimationName != NO_ANIMATION)
        {
            _attackAnimationLength += attacker.GetCharacterAnimator().GetAnimationClip( characterAnimationName ).length;
        }
        if (skillEffectAnimationName != NO_ANIMATION)
        {
            _attackAnimationLength += attacker.GetSkillEffectAnimator().GetAnimationClip( skillEffectAnimationName ).length;
        }

        return _attackAnimationLength;
    }

    private IEnumerator CountdownForEventCutoff( float delay, GameCharacter gameCharacter, AnimationEvent animationEvent )
    {
        yield return new WaitForSeconds( delay );
        gameCharacter.TriggerEvent( animationEvent );
    }

    public void OnAnimationEventTriggered( string parameterValue )
    {
        if (parameterValue == "attack" || parameterValue == "attack2")
        {
            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_ATTACK );

            if (parameterValue == "attack")
            {
                return;
            }
        }
        else if (parameterValue == "hit")
        {
            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
            return;
        }
        else if (parameterValue == "blur")
        {
            this.battleGameManager.GetBattleVisualEffectManager().ApplyBlurShaderAnimationAtRepulse();
            return;
        }

        this.isAnimationEventTriggered = true;
    }

    public void ChangeToBackgroundPartA()
    {
        this.background.sprite = this.backgroundPartA;
    }

    public void ChangeToBackgroundPartB()
    {
        this.background.sprite = this.backgroundPartB;
    }

    public GameCharacter GetCurrentCaster()
    {
        return this.currentCaster;
    }

    public void SetIsDebugMode( bool isDebugMode )
    {
        this.isDebugMode = isDebugMode;
    }

    private void ShowSkillInfo( GameCharacter attacker, GameCharacter attackTarget )
    {
        this.skillPromptPanel.ShowCasterCurrentSkillInfo( attacker );
    }
}
