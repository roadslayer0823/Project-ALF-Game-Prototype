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

    private bool isAnimationEventTriggered = false;
    private Action<bool> onBattleEndedCallback = null;

    private const string NO_ANIMATION = "-";
    private const string IDLE_ANIMATION_NAME = "Idle";
    private const string GETTING_HIT_ANIMATION_NAME = "GettingHit";
    private const string ANIMATION_TYPE_IS_RANGED = "ranged";

    public const string SET_CHARACTER = "set-character";
    public const string ON_DEFEND_PART_A = "on-defend-part-a";
    public const string ON_DEFEND_PART_A_CUTOFF = "on-defend-part-a-cutoff";

    public void Initialize( Action<bool> onBattleEndedCallback )
    {
        this.onBattleEndedCallback = onBattleEndedCallback;
    }

    public IEnumerator RunBattleAnimation( BattleFlowATL battleFlowATL )
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

        _attacker.GetSortingGroup().sortingOrder = 2;
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
        StartCoroutine( CountdownForAttackPartA( _attackAnimationLength * GameConfiguration.ACTION_CUTOFF_TIME_PERCENTAGE, _attackTarget ) );

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

        switch ( _attackTarget.GetCurrentCharacterActionType() )
        {
            case GameCharacter.CharacterActionType.None:

                _attacker.GetOpponentContainer().SetActive( true );

                if (_characterPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB ) );
                }

                if (_skillEffectPartB != NO_ANIMATION)
                {
                    yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartB ) );
                }

                _attackTarget.MinusRemainingHealthPoint( _subskill.AttackDamage );

                yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME ) );

                break;

            case GameCharacter.CharacterActionType.Repulse:
                break;
        }

        if (_attackTarget.GetRemainingHealthPoint() <= 0)
        {
            _attackTarget.gameObject.SetActive( false );

            bool _isVictory = ( _attackTarget is EnemyCharacter );
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

    private void OnAnimationEventTriggered( string parameterValue )
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
