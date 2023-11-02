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
    [SerializeField] private SpriteRenderer background = null;
    [SerializeField] private Sprite backgroundPartA = null;
    [SerializeField] private Sprite backgroundPartB = null;
    [SerializeField] private Animator skillEffectAnimator = null;
    [SerializeField] private Animator skillEffectUiAnimator = null;

    [SerializeField] private Camera targetCamera = null;
    [SerializeField] private CameraEffects cameraEffect = null;
    [SerializeField] private SpriteRenderer darkLayer = null;

    private bool isAnimationEventTriggered = false;
    private Action<bool> onBattleEndedCallback = null;

    private GameObject targetCameraObject = null;
    private Vector3 cameraPosition = Vector3.zero;
    private float cameraOrthographicSize = 0.0f;

    private List<GameCharacter> gameCharacterList = null;
    private GameCharacter currentCaster = null;

    private const string NO_ANIMATION = "-";
    private const string IDLE_ANIMATION_NAME = "Idle";
    private const string GETTING_HIT_ANIMATION_NAME = "GettingHit";
    private const string REPULSE_ANIMATION_NAME = "Repulse";
    private const string DERIVE_ANIMATION_NAME = "Derive";

    private const float SKILL_TIME_STOP_DURATION = 1.2f;
    private const float SKILL_TIME_STOP_DARKNESS = 0.8f;

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
        OnSkillBeingObserved,
        OnActiveSkillStarted,
        OnActiveSkillFinished
    }

    public void Initialize( Action<bool> onBattleEndedCallback )
    {
        this.onBattleEndedCallback = onBattleEndedCallback;

        this.targetCameraObject = this.targetCamera.gameObject;
        this.cameraPosition = this.targetCamera.transform.position;
        this.cameraOrthographicSize = this.targetCamera.orthographicSize;
    }

    public IEnumerator RunBattleAnimation( BattleGameManager battleGameManager, BattleFlowRound battleFlowRound, BattleFlowATL battleFlowATL )
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

            if (_attacker is PlayerCharacter)
            {
                ChangeToBackgroundPartA();
            }
            else if (_attacker is EnemyCharacter)
            {
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
                currentCaster = _attacker;

                BattleLog.Instance.AddOnScreenBattleLog( _log );

                _attacker.TriggerEvent( AnimationEvent.SetCharacter );
                _attackTarget.TriggerEvent( AnimationEvent.SetCharacter );

                yield return StartCoroutine( PlaySkillTimeStopAnimationIfNeeded( _attacker.GetCurrentSkill() ) );

                _skillCountdownTime = ( GetAttackAnimationLength( _attacker, _attackerCharacterPartA, _attackerSkillEffectPartA ) + 1.0f ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage();
                _attackTarget.SetSkillCountdownTime( _skillCountdownTime );
                _attackTarget.TriggerEvent( AnimationEvent.OnDefensePartA );
                StartCoroutine( CountdownForEventCutoff( _skillCountdownTime, _attackTarget, AnimationEvent.OnDefensePartA_Cutoff ) );

                if (_attacker.GetCurrentSkill().GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
                {
                    _attackTarget.TriggerEvent( AnimationEvent.OnActiveSkillStarted );
                }
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

            if (_attacker is PlayerCharacter)
            {
                ChangeToBackgroundPartB();
            }
            else if (_attacker is EnemyCharacter)
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

                    if (CheckHasBattleEnded( battleGameManager ))
                    {
                        yield break;
                    }

                    if (_attacker.GetCurrentSkill().GetSkillData().skillType == Skill.SkillType.derived)
                    {
                        yield return StartCoroutine( RunDerivedSkill( _attacker, _attackTarget, battleFlowRound ) );
                    }

                    if (CheckHasBattleEnded( battleGameManager ))
                    {
                        yield break;
                    }

                    break;

                case Skill.SkillType.repulse:

                    BattleFlowATL _attackTargetNextATL = battleFlowRound.GetNextATL( _attackTarget );
                    battleFlowRound.GoToTargetATL( _attackTargetNextATL, false );

                    CharacterSkill _repulseSkill = _attackTarget.GetCurrentSkill();
                    BattleLogicManager.ExecuteCasterSkillOnUse( _attackTarget, _attacker, out _log );
                    currentCaster = _attackTarget;

                    BattleLog.Instance.AddOnScreenBattleLog( _log );

                    yield return StartCoroutine( PlaySkillTimeStopAnimationIfNeeded( _attackTarget.GetCurrentSkill() ) );

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

                    yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );
                    yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );

                    BattleLogicManager.CompareCharacterSkillAttributes( BattleLogicManager.ActionType.Repulse,
                                                                        _attacker, _attackTarget,
                                                                        out _winner, out _loser );

                    bool _hasAttackDamage = false;
                    bool _hasStressValueDamage = false;
                    bool _hasStatePointDamage = false;
                    if (_winner != null)
                    {
                        BattleLog.Instance.AddOnScreenBattleLog( $"迎擊結果為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _winner.GetCharacterName() }</color>勝利。" );

                        RangeType _attackTargetRangeType = _repulseSkill.GetCharacterSubskillData().GetSubskillData().Range;

                        if (_attackerRangeType == RangeType.melee)
                        {
                            if (_attackTargetRangeType == RangeType.melee)
                            {
                                _hasAttackDamage = true;
                                _hasStressValueDamage = true;
                                _hasStatePointDamage = true;
                                OnHitWithNoDamage( _winner, _loser );
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
                                    OnHitWithNoDamage( _winner, _loser );
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
                                    OnHitWithNoDamage( _winner, _loser );
                                }
                            }
                            else if (_attackTargetRangeType == RangeType.ranged)
                            {
                                _hasAttackDamage = true;
                                _hasStressValueDamage = true;
                                _hasStatePointDamage = true;
                            }
                        }

                        BattleLogicManager.ExecuteCasterSkillOnHit( _winner, _loser, _hasAttackDamage, _hasStressValueDamage, _hasStatePointDamage, out _attackDamage, out _stressValueDamage, out _statePointDamage, out _log );
                        BattleLog.Instance.AddOnScreenBattleLog( _log );
                    }
                    else
                    {
                        BattleLog.Instance.AddOnScreenBattleLog( $"迎擊結果為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attacker.GetCharacterName() }</color>和<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _attackTarget.GetCharacterName() }</color>打平。" );

                        OnHitWithNoDamage( _attacker, _attackTarget );
                        OnHitWithNoDamage( _attackTarget, _attacker );
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
                                                                                     + ( ( _attacker is PlayerCharacter ) ? "Left" : "Right" ),
                                                                                     _attackDamage, _stressValueDamage, _statePointDamage ) );
                    }

                    yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );

                    if (_hasAttackDamage)
                    {
                        if (CheckHasBattleEnded( battleGameManager ))
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
                                yield return StartCoroutine( RunDerivedSkill( _winner, _loser, battleFlowRound ) );

                                if (CheckHasBattleEnded( battleGameManager ))
                                {
                                    yield break;
                                }
                            }
                        }
                    }

                    break;

                case Skill.SkillType.backend:

                    yield return StartCoroutine( PlaySkillTimeStopAnimationIfNeeded( _attackTarget.GetCurrentSkill() ) );

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
                    currentCaster = _attackTarget;

                    BattleLog.Instance.AddOnScreenBattleLog( _log );

                    SkillAnimation _attackTargetBackendSkillAnimation = DatabaseManager.Instance.GetSkillAnimation( _attackTargetSubskillData.Id );
                    string _attackTargetBackendSkillAnimationCharacterPartA = _attackTargetBackendSkillAnimation.CharacterPartA;
                    string _attackTargetBackendSkillAnimationSkillEffectPartA = _attackTargetBackendSkillAnimation.SkillEffectPartA;

                    _winner = null;
                    _loser = null;

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

                        if (CheckHasBattleEnded( battleGameManager ))
                        {
                            yield break;
                        }

                        if (_attacker.GetCurrentSkill().GetSkillData().skillType == Skill.SkillType.derived)
                        {
                            yield return StartCoroutine( RunDerivedSkill( _attacker, _attackTarget, battleFlowRound ) );
                        }

                        if (CheckHasBattleEnded( battleGameManager ))
                        {
                            yield break;
                        }
                    }
                    else if (_winner == _attackTarget)
                    {
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

                if (_attacker is PlayerCharacter)
                {
                    yield return StartCoroutine( PlayAnimation( skillEffectUiAnimator, "Player_Ariku_Counterattack" ) );
                }
                else if (_attacker is EnemyCharacter)
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

    private IEnumerator RunDerivedSkill( GameCharacter attacker, GameCharacter attackTarget, BattleFlowRound battleFlowRound )
    {
        attackTarget.Reset();
        attackTarget.PlayCharacterAnimation( IDLE_ANIMATION_NAME );

        bool _isAbleToUseSkill = BattleLogicManager.IsAbleToUseSkill( attacker );

        string _log = "";

        if (_isAbleToUseSkill)
        {
            battleFlowRound.GoToTargetATL( battleFlowRound.GetNextATL( attacker ), false );
            BattleLogicManager.ExecuteCasterSkillOnUse( attacker, attackTarget, out _log );
            currentCaster = attacker;

            BattleLog.Instance.AddOnScreenBattleLog( _log );
        }

        attacker.GetSortingGroup().sortingOrder = 3;
        attackTarget.GetSortingGroup().sortingOrder = 1;

        float _attackDamage = 0;
        float _stressValueDamage = 0;
        float _statePointDamage = 0;

        Subskill _attackerSubskillData = attacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        if (_attackerSubskillData.Range == RangeType.melee)
        {
            if (_isAbleToUseSkill)
            {
                SkillAnimation _skillAnimation = DatabaseManager.Instance.GetSkillAnimation( _attackerSubskillData.Id );
                string _characterPartB = _skillAnimation.CharacterPartB;
                string _skillEffectPartB = _skillAnimation.SkillEffectPartB;

                yield return StartCoroutine( PlaySkillTimeStopAnimationIfNeeded( attacker.GetCurrentSkill() ) );

                if (_characterPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlayCharacterAnimation( attacker, _characterPartB ) );
                }

                if (_skillEffectPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlaySkillEffectAnimation( attacker, _skillEffectPartB ) );
                }

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

            if (_isAbleToUseSkill)
            {
                yield return StartCoroutine( PlaySkillTimeStopAnimationIfNeeded( attacker.GetCurrentSkill() ) );
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

                BattleLogicManager.ExecuteCasterSkillOnHit( attacker, attackTarget, true, out _attackDamage, out _stressValueDamage, out _statePointDamage, out _log );
                BattleLog.Instance.AddOnScreenBattleLog( _log );

                this.cameraEffect.Shake();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HIT );
                yield return null;
                StartCoroutine( ShowPopUpDisplayInfo( attackTarget, _attackDamage, _stressValueDamage, _statePointDamage ) );
                yield return new WaitForSeconds( 0.7f );
            }
        }

        if (_isAbleToUseSkill)
        {
            yield return StartCoroutine( WaitForPopUpDisplayInfoCompleted() );
        }
        else
        {
            OnCasterBeingUnableToUseSkill( attacker );
            yield return new WaitForSeconds( 1.0f );
        }
    }

    private void OnHitWithNoDamage( GameCharacter caster, GameCharacter target )
    {
        float _damageTaken = 0;
        float _stressValueIncreased = 0;
        float _statePointReduced = 0;

        string _log = "";
        BattleLogicManager.ExecuteCasterSkillOnHit( caster, target, false, out _damageTaken, out _stressValueIncreased, out _statePointReduced, out _log );
        BattleLog.Instance.AddOnScreenBattleLog( _log );

        StartCoroutine( ShowPopUpDisplayInfo( target, _damageTaken, _stressValueIncreased, _statePointReduced ) );
    }

    private void OnCasterBeingUnableToUseSkill( GameCharacter caster )
    {
        caster.SetIsAbleToUseSkill( false );

        CharacterSkill _casterSkill = caster.GetCurrentSkill();
        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ caster.GetCharacterName() }</color>"
                                                 + $"因<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.STATE_POINT }</color>"
                                                 + $"為<color={BattleLog.KEYWORD_COLOR_CODE}>{ GameConfiguration.Instance.GetBattleConfiguration().GetMinimumCurrentStatePoint() }</color>"
                                                 + $"而無法使用<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _casterSkill.GetCharacterSubskillData().GetSubskillData().DisplayName }</color>"
                                                 + $"（{ TerminologyManager.GetSkillTypeText( _casterSkill.GetSkillData().skillType ) }）。" );
    }

    private IEnumerator PlayCharacterAnimation( GameCharacter gameCharacter, string animationName,
                                                float damageTaken = 0, float stressValueIncreased = 0, float statePointReduced = 0 )
    {
        gameCharacter.PlayCharacterAnimation( animationName, OnAnimationEventTriggered );

        if (animationName.Contains( GETTING_HIT_ANIMATION_NAME ))
        {
            StartCoroutine( ShowPopUpDisplayInfo( gameCharacter, damageTaken, stressValueIncreased, statePointReduced ) );
        }

        yield return StartCoroutine( WaitForAnimationEventTriggered() );
    }

    private IEnumerator ShowPopUpDisplayInfo( GameCharacter gameCharacter,
                                              float damageTaken = 0, float stressValueIncreased = 0, float statePointReduced = 0 )
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
    }

    private IEnumerator ZoomInCameraToTarget( GameCharacter target, float duration )
    {
        float _cameraX = 0.0f;
        float _cameraY = 0.0f;
        float _cameraSize = 0.0f;
        if (target is PlayerCharacter)
        {
            _cameraX = 2.0f;
            _cameraY = -1.2f;
            _cameraSize = 3.5f;
        }
        else if (target is EnemyCharacter)
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
        LeanTween.value( 0.0f, darkness, duration * 0.9f ).setEaseOutCirc().setOnUpdate( SetDarkLayerAlphaValue ).setOnComplete( () =>
        {
            LeanTween.value( darkness, 0.0f, duration * 0.1f ).setEaseOutCirc().setOnUpdate( SetDarkLayerAlphaValue );
        } );

        yield return new WaitForSeconds( duration );
    }

    private void SetDarkLayerAlphaValue( float value )
    {
        Color _color = this.darkLayer.color;
        _color.a = value;
        this.darkLayer.color = _color;
    }

    private bool CheckHasBattleEnded( BattleGameManager battleGameManager )
    {
        bool _hasPlayerCharacterSurvived = false;
        bool _hasEnemyCharacterSurvived = false;
        List<PlayerCharacter> _playerCharacterList = battleGameManager.GetPlayerCharacterList();
        List<EnemyCharacter> _enemyCharacterList = battleGameManager.GetEnemyCharacterList();

        for (int i = 0; i < _playerCharacterList.Count; i++)
        {
            PlayerCharacter _playerCharacter = _playerCharacterList[ i ];
            if (BattleLogicManager.IsGameCharacterDead( _playerCharacter ))
            {
                _playerCharacter.gameObject.SetActive( false );
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
                _enemyCharacter.gameObject.SetActive( false );
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

    private IEnumerator PlaySkillTimeStopAnimationIfNeeded( CharacterSkill characterSkill )
    {
        if (CheckHasTimeStop( characterSkill ))
        {
            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_HINTS );
            yield return StartCoroutine( FadeDarkLayer( SKILL_TIME_STOP_DARKNESS, SKILL_TIME_STOP_DURATION ) );
        }
    }

    private bool CheckHasTimeStop( CharacterSkill characterSkill )
    {
        Subskill _subskillData = characterSkill.GetCharacterSubskillData().GetSubskillData();

        if (_subskillData.Speed > 2)
        {
            return true;
        }

        if (_subskillData.Strength > 1)
        {
            return true;
        }

        if (_subskillData.Accuracy > 1)
        {
            return true;
        }

        if (_subskillData.Evasion > 1)
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
}
