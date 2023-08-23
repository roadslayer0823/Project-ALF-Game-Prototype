using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzeGames.Effects;
using SkillAnimation = DatabaseManager.SkillAnimation;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;

public class BattleAnimationManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background = null;
    [SerializeField] private Sprite backgroundPartA = null;
    [SerializeField] private Sprite backgroundPartB = null;
    [SerializeField] private Animator skillEffectAnimator = null;

    [SerializeField] private Camera targetCamera = null;
    [SerializeField] private CameraEffects cameraEffect = null;

    private bool isAnimationEventTriggered = false;
    private Action<bool> onBattleEndedCallback = null;

    private GameObject targetCameraObject = null;
    private Vector3 cameraPosition = Vector3.zero;
    private float cameraOrthographicSize = 0.0f;

    private const string NO_ANIMATION = "-";
    private const string IDLE_ANIMATION_NAME = "Idle";
    private const string GETTING_HIT_ANIMATION_NAME = "GettingHit";
    private const string REPULSE_ANIMATION_NAME = "Repulse";
    private const string DERIVE_ANIMATION_NAME = "Derive";
    private const string DEFEND_ANIMATION_NAME = "Defend";

    public enum AnimationEvent
    {
        None,
        SetCharacter,
        OnAttackPartB,
        OnAttackPartB_Cutoff,
        OnDefendPartA,
        OnDefendPartA_Cutoff,
        OnRepulseWin,
        OnRepulseWin_Cutoff
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
            yield break;
        }

        GameCharacter _attackTarget = battleFlowATL.GetAttackTarget();
        CharacterSkill _attackerSkill = battleFlowATL.GetSelectedSkill();
        SkillAnimation _skillAnimation = DatabaseManager.Instance.GetSkillAnimation( _attackerSkill.GetCharacterSubskillData().GetSubskillData().Id );

        RangeType _attackerRangeType = battleFlowATL.GetSelectedSkill().GetCharacterSubskillData().GetSubskillData().Range;
        string _characterPartA = _skillAnimation.CharacterPartA;
        string _characterPartB = _skillAnimation.CharacterPartB;
        string _skillEffectPartA = _skillAnimation.SkillEffectPartA;
        string _skillEffectPartB = _skillAnimation.SkillEffectPartB;

        _attacker.GetSortingGroup().sortingOrder = 3;
        _attackTarget.GetSortingGroup().sortingOrder = 1;

        _attacker.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.None );
        _attackTarget.SetCurrentCharacterActionType( GameCharacter.CharacterActionType.None );

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

        _attacker.SetCurrentSkill( _attackerSkill, _attackTarget );
        BattleLogicManager.ExecuteCasterSkillOnUse( _attacker, _attackTarget );
        _attacker.TriggerEvent( AnimationEvent.SetCharacter );
        _attackTarget.TriggerEvent( AnimationEvent.SetCharacter );
        _attackTarget.TriggerEvent( AnimationEvent.OnDefendPartA );

        StartCoroutine( CountdownForEventCutoff( ( GetAttackAnimationLength( _attacker, _characterPartA, _skillEffectPartA ) + 1.0f ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage(),
                                                 _attackTarget, AnimationEvent.OnDefendPartA_Cutoff ) );

        float _cameraX = 0.0f;
        float _cameraY = 0.0f;
        float _cameraSize = 0.0f;
        if (_attacker is PlayerCharacter)
        {
            _cameraX = 2.0f;
            _cameraY = -1.2f;
            _cameraSize = 3.5f;
        }
        else if (_attacker is EnemyCharacter)
        {
            _cameraX = -2.0f;
            _cameraY = -1.7f;
            _cameraSize = 4.0f;
        }

        LeanTween.moveX( this.targetCameraObject, _cameraX, 1.0f ).setEaseOutCirc();
        LeanTween.moveY( this.targetCameraObject, _cameraY, 1.0f ).setEaseOutCirc();
        LeanTween.value( this.targetCamera.orthographicSize, _cameraSize, 1.0f ).setEaseOutCirc().setOnUpdate( ( float value ) => { this.targetCamera.orthographicSize = value;  } );

        yield return new WaitForSeconds( 1.0f );

        if (_characterPartA != NO_ANIMATION)
        {
            yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartA ) );
        }

        if (_skillEffectPartA != NO_ANIMATION)
        {
            yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartA ) );
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

        GameCharacter _winner = null;
        GameCharacter _loser = null;
        CharacterSkill _derivedSkill = null;

        switch ( _attackTarget.GetCurrentCharacterActionType() )
        {
            case GameCharacter.CharacterActionType.None:

                _attacker.TriggerEvent( AnimationEvent.OnAttackPartB );
                StartCoroutine( CountdownForEventCutoff( ( GetAttackAnimationLength( _attacker, _characterPartB, _skillEffectPartB ) + 1.0f ) * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage(),
                                                         _attacker, AnimationEvent.OnAttackPartB_Cutoff ) );

                if (_characterPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB ) );
                }

                if (_skillEffectPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartB ) );
                }

                BattleLogicManager.ExecuteCasterSkillOnHit( _attacker, _attackTarget );
                this.cameraEffect.Shake();
                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME ) );
                yield return new WaitForSeconds( 1.0f );

                if (CheckHasBattleEnded( battleGameManager ))
                {
                    yield break;
                }

                if (_attacker.GetCurrentCharacterActionType() == GameCharacter.CharacterActionType.Derive)
                {
                    yield return StartCoroutine( RunDerivedSkill( _attackerSkill.GetCharacterSubskillData().GetDerivedSkill(),
                                                                  _attacker, _attackTarget, battleFlowRound ) );
                }

                if (CheckHasBattleEnded( battleGameManager ))
                {
                    yield break;
                }

                break;

            case GameCharacter.CharacterActionType.Repulse:

                BattleFlowATL _attackTargetNextATL = battleFlowRound.GetNextATL( _attackTarget );
                battleFlowRound.GoToTargetATL( _attackTargetNextATL, false );

                CharacterSkill _repulseSkill = _attackTargetNextATL.GetSelectedSkill().GetCharacterSubskillData().GetRepulseSkill();
                _attackTarget.SetCurrentSkill( _repulseSkill, _attackTarget );
                BattleLogicManager.ExecuteCasterSkillOnUse( _attackTarget, _attacker );

                if (_characterPartB != NO_ANIMATION)
                {
                    StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB + "_" + REPULSE_ANIMATION_NAME ) );
                }

                if (_skillEffectPartB != NO_ANIMATION)
                {
                    StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartB + "_" + REPULSE_ANIMATION_NAME ) );
                }

                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );
                yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );

                BattleLogicManager.CompareCharacterSkillAttributes( BattleLogicManager.ActionType.Repulse,
                                                                    _attacker, _attackTarget,
                                                                    out _winner, out _loser );

                if (_winner != null)
                {
                    _winner.TriggerEvent( AnimationEvent.OnRepulseWin );
                    StartCoroutine( CountdownForEventCutoff( 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage(),
                                                             _attacker, AnimationEvent.OnRepulseWin_Cutoff ) );

                    if (_attackerRangeType == RangeType.melee)
                    {
                        BattleLogicManager.ExecuteCasterSkillOnHit( _winner, _loser );
                        this.cameraEffect.Shake();
                        yield return StartCoroutine( PlayCharacterAnimation( _loser, GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_"
                                                                                     + ( ( _attacker is PlayerCharacter ) ? "Left" : "Right" ) ) );
                        yield return new WaitForSeconds( 1.0f );
                    }

                    if (CheckHasBattleEnded( battleGameManager ))
                    {
                        yield break;
                    }

                    _derivedSkill = _winner.GetCurrentSkill().GetCharacterSubskillData().GetDerivedSkill();
                }
                else
                {
                    yield return new WaitForSeconds( 0.5f );
                }

                if (_winner != null)
                {
                    if (_winner.GetCurrentCharacterActionType() == GameCharacter.CharacterActionType.Derive)
                    {
                        battleFlowRound.GoToTargetATL( battleFlowRound.GetNextATL( _winner ), false );
                        _winner.SetCurrentSkill( _derivedSkill );
                        BattleLogicManager.ExecuteCasterSkillOnUse( _winner, _loser );
                        yield return StartCoroutine( PlayCharacterAnimation( _winner, DERIVE_ANIMATION_NAME ) );
                        yield return StartCoroutine( PlaySkillEffectAnimation( REPULSE_ANIMATION_NAME ) );
                        BattleLogicManager.ExecuteCasterSkillOnHit( _winner, _loser );
                        this.cameraEffect.Shake();
                        yield return StartCoroutine( PlayCharacterAnimation( _loser, GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_Right" ) );
                        yield return new WaitForSeconds( 1.0f );

                        if (CheckHasBattleEnded( battleGameManager ))
                        {
                            yield break;
                        }
                    }
                }

                break;

            case GameCharacter.CharacterActionType.Backend:

                if (_characterPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB ) );
                }

                if (_skillEffectPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartB ) );
                }

                CharacterSkill _attackTargetSkill = _attackTarget.GetCurrentSkill();
                BattleLogicManager.ExecuteCasterSkillOnUse( _attackTarget, _attacker );

                Subskill _attackTargetSubskillData = _attackTargetSkill.GetCharacterSubskillData().GetSubskillData();
                _winner = null;
                _loser = null;

                if (_attackTargetSubskillData.IsDefendingSkill)
                {
                    if (_characterPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, DEFEND_ANIMATION_NAME ) );
                    }

                    if (_skillEffectPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, DEFEND_ANIMATION_NAME ) );
                    }

                    BattleLogicManager.CompareCharacterSkillAttributes( BattleLogicManager.ActionType.Defend,
                                                                        _attacker, _attackTarget,
                                                                        out _winner, out _loser );

                    bool _isAttackerWinner = ( _winner == _attacker );
                    BattleLogicManager.ExecuteCasterSkillOnHit( _winner, _loser, _isAttackerWinner );

                    if (_isAttackerWinner)
                    {
                        _attacker.TriggerEvent( AnimationEvent.OnRepulseWin );
                        StartCoroutine( CountdownForEventCutoff( 1.6f * GameConfiguration.Instance.GetBattleConfiguration().GetActionCutoffTimePercentage(),
                                                                 _attacker, AnimationEvent.OnRepulseWin_Cutoff ) );

                        this.cameraEffect.Shake();
                        yield return StartCoroutine( PlayCharacterAnimation( _loser, GETTING_HIT_ANIMATION_NAME ) );
                        yield return new WaitForSeconds( 1.0f );

                        if (CheckHasBattleEnded( battleGameManager ))
                        {
                            yield break;
                        }

                        if (_attacker.GetCurrentCharacterActionType() == GameCharacter.CharacterActionType.Derive)
                        {
                            yield return StartCoroutine( RunDerivedSkill( _attackerSkill.GetCharacterSubskillData().GetDerivedSkill(),
                                                                          _attacker, _attackTarget, battleFlowRound ) );
                        }

                        if (CheckHasBattleEnded( battleGameManager ))
                        {
                            yield break;
                        }
                    }
                }
                else if (_attackTargetSubskillData.IsEvadingSkill)
                {
                    BattleLogicManager.CompareCharacterSkillAttributes( BattleLogicManager.ActionType.Evade,
                                                                        _attacker, _attackTarget,
                                                                        out _winner, out _loser );
                }

                break;
        }

        _attacker.GetOwnContainer().SetActive( false );
        _attacker.ShowCharacterObject();
        _attacker.PlayCharacterAnimation( IDLE_ANIMATION_NAME );
        _attackTarget.PlayCharacterAnimation( IDLE_ANIMATION_NAME );

        _attacker.Reset();
        _attackTarget.Reset();
    }

    private IEnumerator RunDerivedSkill( CharacterSkill derivedSkill, GameCharacter attacker, GameCharacter attackTarget, BattleFlowRound battleFlowRound )
    {
        battleFlowRound.GoToTargetATL( battleFlowRound.GetNextATL( attacker ), false );
        attacker.SetCurrentSkill( derivedSkill );
        BattleLogicManager.ExecuteCasterSkillOnUse( attacker, attackTarget );

        SkillAnimation _skillAnimation = DatabaseManager.Instance.GetSkillAnimation( derivedSkill.GetCharacterSubskillData().GetSubskillData().Id );
        string _characterPartB = _skillAnimation.CharacterPartB;
        string _skillEffectPartB = _skillAnimation.SkillEffectPartB;

        if (_characterPartB != NO_ANIMATION)
        {
            yield return StartCoroutine( PlayCharacterAnimation( attacker, _characterPartB ) );
        }

        if (_skillEffectPartB != NO_ANIMATION)
        {
            yield return StartCoroutine( PlaySkillEffectAnimation( attacker, _skillEffectPartB ) );
        }

        BattleLogicManager.ExecuteCasterSkillOnHit( attacker, attackTarget );
        this.cameraEffect.Shake();
        yield return StartCoroutine( PlayCharacterAnimation( attackTarget, GETTING_HIT_ANIMATION_NAME ) );
        yield return new WaitForSeconds( 1.0f );
    }

    private IEnumerator PlayCharacterAnimation( GameCharacter gameCharacter, string animationName )
    {
        gameCharacter.PlayCharacterAnimation( animationName, OnAnimationEventTriggered );
        yield return StartCoroutine( WaitForAnimationEventTriggered() );
    }

    private IEnumerator PlaySkillEffectAnimation( GameCharacter gameCharacter, string animationName )
    {
        gameCharacter.PlaySkillEffectAnimation( animationName, OnAnimationEventTriggered );
        yield return StartCoroutine( WaitForAnimationEventTriggered() );
    }

    private IEnumerator PlaySkillEffectAnimation( string animationName )
    {
        skillEffectAnimator.Play( animationName );
        yield return StartCoroutine( WaitForAnimationEventTriggered() );
    }

    private IEnumerator WaitForAnimationEventTriggered()
    {
        this.isAnimationEventTriggered = false;
        yield return new WaitUntil( () => this.isAnimationEventTriggered );
    }

    private bool CheckHasBattleEnded( BattleGameManager battleGameManager )
    {
        bool _hasPlayerCharacterSurvived = false;
        bool _hasEnemyCharacterSurvived = false;
        List<PlayerCharacter> _playerCharacterList = battleGameManager.GetPlayerCharacterList();
        List<EnemyCharacter> _enemyCharacterList = battleGameManager.GetEnemyCharacterList();

        for (int i = 0; i < _playerCharacterList.Count; i++)
        {
            if (!BattleLogicManager.IsGameCharacterDead( _playerCharacterList[i]))
            {
                _hasPlayerCharacterSurvived = true;
                break;
            }
        }

        for (int i = 0; i < _enemyCharacterList.Count; i++)
        {
            if (!BattleLogicManager.IsGameCharacterDead( _enemyCharacterList[ i ] ))
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
}
