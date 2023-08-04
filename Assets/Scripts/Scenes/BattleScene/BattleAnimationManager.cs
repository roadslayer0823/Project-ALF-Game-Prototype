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

    public void Initialize( Action<bool> onBattleEndedCallback )
    {
        this.onBattleEndedCallback = onBattleEndedCallback;
    }

    public IEnumerator RunBattleAnimation( BattleFlowATL battleFlowATL )
    {
        GameCharacter _attacker = battleFlowATL.GetSelectedCharacter();
        GameCharacter _attackTarget = battleFlowATL.GetAttackTarget();
        CharacterSkill _characterSkill = battleFlowATL.GetSelectedSkill();
        Subskill _subskill = _characterSkill.GetSubskillByLevel(1);
        SkillAnimation _skillAnimation = _characterSkill.GetSkillAnimation(_subskill.GetId());

        string _animationType = _skillAnimation.GetAnimationType().ToString();
        string _characterPartA = _skillAnimation.GetCharacterPartA();
        string _characterPartB = _skillAnimation.GetCharacterPartB();
        string _skillEffectPartA = _skillAnimation.GetSkillEffectPartA();
        string _skillEffectPartB = _skillAnimation.GetSkillEffectPartB();

        _attacker.GetSortingGroup().sortingOrder = 2;
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

        if (_characterPartA != NO_ANIMATION)
        {
            yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartA ) );
        }

        if (_skillEffectPartA != NO_ANIMATION)
        {
            yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartA ) );
        }

        // Hide the attacker for Part B if the attacker's animation type is ranged.
        if (_animationType == "ranged")
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

        if (_characterPartB != NO_ANIMATION)
        {
            yield return StartCoroutine( PlayCharacterAnimation( _attacker, _characterPartB ) );
        }

        _attackTarget.MinusRemainingHealthPoint( _subskill.AttackDamage );

        if (_skillEffectPartB != NO_ANIMATION)
        {
            yield return StartCoroutine( PlaySkillEffectAnimation( _attacker, _skillEffectPartB ) );
        }

        yield return StartCoroutine( PlayCharacterAnimation( _attackTarget, GETTING_HIT_ANIMATION_NAME ) );

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
