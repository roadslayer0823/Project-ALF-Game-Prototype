using System.Collections;
using UnityEngine;

public class BattleAnimationManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background = null;
    [SerializeField] private Sprite backgroundPartA = null;
    [SerializeField] private Sprite backgroundPartB = null;

    private bool isAnimationEventTriggered = false;

    public IEnumerator RunBattleAnimation( BattleFlowATL battleFlowATL )
    {
        GameCharacter _attacker = battleFlowATL.GetSelectedCharacter();
        CharacterSkill _skill = battleFlowATL.GetSelectedSkill();
        bool _hasSkillEffectAnimation = false;

        _attacker.GetSortingGroup().sortingOrder = 2;
        battleFlowATL.GetAttackTarget().GetSortingGroup().sortingOrder = 1;

        if (_attacker is PlayerCharacter)
        {
            ChangeToBackgroundPartA();
            _hasSkillEffectAnimation = true;
        }
        else if (_attacker is EnemyCharacter)
        {
            ChangeToBackgroundPartB();
            _hasSkillEffectAnimation = false;
        }

        _attacker.GetOwnContainer().SetActive( true );
        _attacker.GetCharacterAnimator().gameObject.SetActive( true );
        _attacker.GetOpponentContainer().SetActive( false );
        yield return new WaitForSeconds( 0.1f );

        if (_attacker is PlayerCharacter)
        {
            _attacker.PlayCharacterAnimation( "Attack" );
        }

        if (_hasSkillEffectAnimation)
        {
            _attacker.PlaySkillEffectAnimation( "Fireball_Part_A", OnAnimationEventTriggered );
            this.isAnimationEventTriggered = false;
            yield return new WaitUntil( () => this.isAnimationEventTriggered );
        }

        if (_attacker is PlayerCharacter)
        {
            _attacker.GetCharacterAnimator().gameObject.SetActive( false );
            ChangeToBackgroundPartB();
        }
        else if (_attacker is EnemyCharacter)
        {
            ChangeToBackgroundPartA();
        }

        if (_hasSkillEffectAnimation)
        {
            _attacker.GetOpponentContainer().SetActive( true );
            _attacker.PlaySkillEffectAnimation( "Fireball_Part_B", OnAnimationEventTriggered );
        }

        if (_attacker is EnemyCharacter)
        {
            _attacker.PlayCharacterAnimation( "Attack_Part_A", OnAnimationEventTriggered );
            this.isAnimationEventTriggered = false;
            yield return new WaitUntil( () => this.isAnimationEventTriggered );
            _attacker.GetOpponentContainer().SetActive( true );
            _attacker.PlayCharacterAnimation( "Attack_Part_B", OnAnimationEventTriggered );
        }

        this.isAnimationEventTriggered = false;
        yield return new WaitUntil( () => this.isAnimationEventTriggered );
        battleFlowATL.GetAttackTarget().PlayCharacterAnimation( "GettingHit" );

        if (_attacker is EnemyCharacter)
        {
            _attacker.PlaySkillEffectAnimation( "HittingEffect" );
            yield return new WaitForSeconds( 0.3f );
        }

        yield return new WaitForSeconds( 0.5f );

        _attacker.GetOwnContainer().SetActive( false );
        _attacker.GetCharacterAnimator().gameObject.SetActive( true );
        _attacker.PlayCharacterAnimation( "Idle" );
    }

    private void OnAnimationEventTriggered( string parameterValue )
    {
        this.isAnimationEventTriggered = true;
    }

    private void ChangeToBackgroundPartA()
    {
        this.background.sprite = this.backgroundPartA;
    }

    private void ChangeToBackgroundPartB()
    {
        this.background.sprite = this.backgroundPartB;
    }
}
