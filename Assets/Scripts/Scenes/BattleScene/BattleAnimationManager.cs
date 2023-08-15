using System;
using System.Collections;
using UnityEngine;
using SkillAnimation = DatabaseManager.SkillAnimation;

public class BattleAnimationManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background = null;
    [SerializeField] private Sprite backgroundPartA = null;
    [SerializeField] private Sprite backgroundPartB = null;
    [SerializeField] private Animator skillEffectAnimator = null;

    private bool isAnimationEventTriggered = false;
    private Action<bool> onBattleEndedCallback = null;

    private const string NO_ANIMATION = "-";
    private const string IDLE_ANIMATION_NAME = "Idle";
    private const string GETTING_HIT_ANIMATION_NAME = "GettingHit";
    private const string REPULSE_ANIMATION_NAME = "Repulse";
    private const string DERIVE_ANIMATION_NAME = "Derive";
    private const string ANIMATION_TYPE_IS_RANGED = "ranged";

    public enum AnimationEvent
    {
        None,
        SetCharacter,
        OnAttackPartB,
        OnAttackPartB_Cutoff,
        OnDefendPartA,
        OnDefendPartA_Cutoff,
        OnRepulseWin
    }

    public void Initialize( Action<bool> onBattleEndedCallback )
    {
        this.onBattleEndedCallback = onBattleEndedCallback;
    }

    public IEnumerator RunBattleAnimation( BattleFlowRound battleFlowRound, BattleFlowATL battleFlowATL )
    {
        GameCharacter _attacker = battleFlowATL.GetSelectedCharacter();
        GameCharacter _attackTarget = battleFlowATL.GetAttackTarget();
        CharacterSkill _attackerSkill = battleFlowATL.GetSelectedSkill();
        SkillAnimation _skillAnimation = DatabaseManager.Instance.GetSkillAnimation( _attackerSkill.GetCharacterSubskillData().GetSubskillData().Id );

        string _animationType = _skillAnimation.animationType.ToString();
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
        BattleLogicManager.ExecuteSkillOnUse( _attackerSkill, _attacker, _attackTarget );
        _attacker.TriggerEvent( AnimationEvent.SetCharacter );
        _attackTarget.TriggerEvent( AnimationEvent.SetCharacter );
        _attackTarget.TriggerEvent( AnimationEvent.OnDefendPartA );

        StartCoroutine( CountdownForEventCutoff( GetAttackAnimationLength( _attacker, _characterPartA, _skillEffectPartA ) * GameConfiguration.Battle.Instance.GetActionCutoffTimePercentage(),
                                                 _attackTarget, AnimationEvent.OnDefendPartA_Cutoff ) );

        if (_characterPartA != NO_ANIMATION)
        {
            yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartA ) );
        }

        if (_skillEffectPartA != NO_ANIMATION)
        {
            yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartA ) );
        }

        // Hide the attacker for Part B if the attacker's animation type is ranged.
        if (_animationType == ANIMATION_TYPE_IS_RANGED)
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

        _attacker.GetOpponentContainer().SetActive( true );
        _attacker.TriggerEvent( AnimationEvent.OnAttackPartB );
        StartCoroutine( CountdownForEventCutoff( GetAttackAnimationLength( _attacker, _characterPartB, _skillEffectPartB ) * GameConfiguration.Battle.Instance.GetActionCutoffTimePercentage(),
                                                 _attacker, AnimationEvent.OnAttackPartB_Cutoff ) );

        CharacterSkill _derivedSkill = null;

        switch ( _attackTarget.GetCurrentCharacterActionType() )
        {
            case GameCharacter.CharacterActionType.None:

                if (_characterPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB ) );
                }

                if (_skillEffectPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartB ) );
                }

                BattleLogicManager.ExecuteSkillOnHittingTarget( _attackerSkill, _attacker, _attackTarget );
                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME ) );

                if (_attacker.GetCurrentCharacterActionType() == GameCharacter.CharacterActionType.Derive)
                {
                    _derivedSkill = _attackerSkill.GetCharacterSubskillData().GetDerivedSkill();
                    battleFlowRound.GoToTargetATL( battleFlowRound.GetNextATL( _attacker ), false );
                    BattleLogicManager.ExecuteSkillOnUse( _derivedSkill, _attacker, _attackTarget );

                    if (_characterPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB ) );
                    }

                    if (_skillEffectPartB != NO_ANIMATION)
                    {
                        yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartB ) );
                    }

                    BattleLogicManager.ExecuteSkillOnHittingTarget( _derivedSkill, _attacker, _attackTarget );
                    yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME ) );
                }

                break;

            case GameCharacter.CharacterActionType.Repulse:

                BattleFlowATL _attackTargetNextATL = battleFlowRound.GetNextATL( _attackTarget );
                battleFlowRound.GoToTargetATL( _attackTargetNextATL, false );

                CharacterSkill _repulseSkill = _attackTargetNextATL.GetSelectedSkill().GetCharacterSubskillData().GetRepulseSkill();
                BattleLogicManager.ExecuteSkillOnUse( _repulseSkill, _attackTarget, _attacker );

                if (_characterPartB != NO_ANIMATION)
                {
                    StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB + "_" + REPULSE_ANIMATION_NAME ) );
                }

                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );
                yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );
                yield return StartCoroutine( PlaySkillEffectAnimation( REPULSE_ANIMATION_NAME ) );

                GameCharacter _winner = BattleLogicManager.GetWinnerByComparingSkillAttributes( BattleLogicManager.SkillAttribute.Strength,
                                                                                                _attacker, _attackerSkill,
                                                                                                _attackTarget, _repulseSkill );
                GameCharacter _loser = null;

                if (_winner != null)
                {
                    if (_winner == _attacker)
                    {
                        _loser = _attackTarget;
                        _winner.SetCurrentSkill( _attackerSkill, _loser );
                    }
                    else if (_winner == _attackTarget)
                    {
                        _loser = _attacker;
                        _winner.SetCurrentSkill( _repulseSkill, _loser );
                    }

                    _winner.TriggerEvent( AnimationEvent.OnRepulseWin );
                    BattleLogicManager.ExecuteSkillOnHittingTarget( _winner.GetCurrentSkill(), _winner, _loser );
                    yield return StartCoroutine( PlayCharacterAnimation( _loser, GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_Right" ) );

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
                        BattleLogicManager.ExecuteSkillOnUse( _derivedSkill, _winner, _loser );
                        yield return StartCoroutine( PlayCharacterAnimation( _winner, DERIVE_ANIMATION_NAME ) );
                        yield return StartCoroutine( PlaySkillEffectAnimation( REPULSE_ANIMATION_NAME ) );
                        BattleLogicManager.ExecuteSkillOnHittingTarget( _derivedSkill, _winner, _loser );
                        yield return StartCoroutine( PlayCharacterAnimation( _loser, GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_Right" ) );
                    }
                }

                break;
        }

        if (BattleLogicManager.IsGameCharacterDead( _attackTarget ))
        {
            _attackTarget.gameObject.SetActive( false );

            bool _isVictory = ( _attackTarget is EnemyCharacter );
            this.onBattleEndedCallback?.Invoke( _isVictory );

            yield break;
        }
        else if (BattleLogicManager.IsGameCharacterDead( _attacker ))
        {
            _attacker.gameObject.SetActive( false );

            bool _isVictory = ( _attacker is EnemyCharacter );
            this.onBattleEndedCallback?.Invoke( _isVictory );

            yield break;
        }

        _attacker.GetOwnContainer().SetActive( false );
        _attacker.ShowCharacterObject();
        _attacker.PlayCharacterAnimation( IDLE_ANIMATION_NAME );
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
