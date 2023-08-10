using System;
using System.Collections;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;
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
    private const string ANIMATION_TYPE_IS_RANGED = "ranged";

    public const string SET_CHARACTER = "set-character";
    public const string ON_DEFEND_PART_A = "on-defend-part-a";
    public const string ON_DEFEND_PART_A_CUTOFF = "on-defend-part-a-cutoff";

    public void Initialize( Action<bool> onBattleEndedCallback )
    {
        this.onBattleEndedCallback = onBattleEndedCallback;
    }

    public IEnumerator RunBattleAnimation( BattleFlowRound battleFlowRound, BattleFlowATL battleFlowATL )
    {
        GameCharacter _attacker = battleFlowATL.GetSelectedCharacter();
        GameCharacter _attackTarget = battleFlowATL.GetAttackTarget();
        CharacterSkill _characterSkill = battleFlowATL.GetSelectedSkill();
        Subskill _subskill = _characterSkill.GetSubskillData();
        SkillAnimation _skillAnimation = _characterSkill.GetSkillAnimation( _subskill.GetId() );

        string _animationType = _skillAnimation.GetAnimationType().ToString();
        string _characterPartA = _skillAnimation.GetCharacterPartA();
        string _characterPartB = _skillAnimation.GetCharacterPartB();
        string _skillEffectPartA = _skillAnimation.GetSkillEffectPartA();
        string _skillEffectPartB = _skillAnimation.GetSkillEffectPartB();

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

        BattleLogicManager.ExecuteSkillOnUse( _characterSkill, _attacker, _attackTarget );
        _attackTarget.TriggerEvent( SET_CHARACTER );

        float _attackAnimationLength = 0;
        if (_characterPartA != NO_ANIMATION)
        {
            _attackAnimationLength += _attacker.GetCharacterAnimator().GetAnimationClip( _characterPartA ).length;
        }
        if (_skillEffectPartA != NO_ANIMATION)
        {
            _attackAnimationLength += _attacker.GetSkillEffectAnimator().GetAnimationClip( _skillEffectPartA ).length;
        }

        _attackTarget.TriggerEvent( ON_DEFEND_PART_A );
        StartCoroutine( CountdownForAttackPartA( _attackAnimationLength * GameConfiguration.Battle.ACTION_CUTOFF_TIME_PERCENTAGE, _attackTarget ) );

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

                BattleLogicManager.ExecuteSkillOnHittingTarget( _characterSkill, _attacker, _attackTarget );
                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME ) );

                break;

            case GameCharacter.CharacterActionType.Repulse:

                BattleLogicManager.ExecuteSkillOnUse( _characterSkill, _attackTarget, _attacker );

                BattleFlowATL _nextATL = battleFlowRound.GetNextATL( _attackTarget );
                battleFlowRound.GoToTargetATL( _nextATL );

                if (_characterPartB != NO_ANIMATION)
                {
                    StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB + "_"+ REPULSE_ANIMATION_NAME ) );
                }

                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );
                yield return StartCoroutine( PlaySkillEffectAnimation( _attackTarget, REPULSE_ANIMATION_NAME ) );
                yield return StartCoroutine( PlaySkillEffectAnimation( REPULSE_ANIMATION_NAME ) );

                int _randomNumber = UnityEngine.Random.Range( 0, 3 );
                if (_randomNumber == 1)
                {
                    BattleLogicManager.ExecuteSkillOnHittingTarget( _nextATL.GetSelectedSkill(), _attackTarget, _attacker );
                    yield return StartCoroutine( PlayCharacterAnimation( _attacker, GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_Right" ) );
                }
                else if (_randomNumber == 2)
                {
                    BattleLogicManager.ExecuteSkillOnHittingTarget( _characterSkill, _attacker, _attackTarget );
                    yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME + "_" + REPULSE_ANIMATION_NAME + "_Right" ) );
                }
                else
                {
                    yield return new WaitForSeconds( 0.5f );
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

    private IEnumerator CountdownForAttackPartA( float delay, GameCharacter gameCharacter )
    {
        yield return new WaitForSeconds( delay );
        gameCharacter.TriggerEvent( ON_DEFEND_PART_A_CUTOFF );
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
