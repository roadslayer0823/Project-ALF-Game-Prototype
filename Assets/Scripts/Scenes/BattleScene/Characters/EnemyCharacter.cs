using System.Collections.Generic;
using UnityEngine;
using AnimationEvent = BattleAnimationManager.AnimationEvent;

public class EnemyCharacter : GameCharacter
{
    public override void OnBattleFlowATLInitialized( BattleFlowATL battleFlowATL )
    {
        if (battleFlowATL.GetSelectedCharacter() == this)
        {
            List<CharacterSkill> _activeSkillList = new List<CharacterSkill>();
            for (int i = 0; i < base.skills.Length; i++)
            {
                CharacterSkill _skill = base.skills[ i ];
                if (_skill.GetSkillData().skillType == DatabaseManager.Skill.SkillType.active)
                {
                    _activeSkillList.Add( _skill );
                }
            }

            battleFlowATL.SetSelectedSkill( _activeSkillList[ Random.Range( 0, _activeSkillList.Count ) ] );
        }
    }

    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        BattleFlowManager _battleFlowManager = battleGameManager.GetBattleFlowManager();

        switch ( animationEvent )
        {
            case AnimationEvent.SetCharacter:
                break;

            case AnimationEvent.OnAttackPartB:
            case AnimationEvent.OnRepulseWin:

                if (base.GetCurrentSkill().GetCharacterSubskillData().GetDerivedSkill() != null)
                {
                    base.SetCurrentCharacterActionType( CharacterActionType.Derive );
                }

                break;

            case AnimationEvent.OnDefendPartA:

                if (base.GetCurrentAttacker().GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().IsInterceptable
                    && _battleFlowManager.GetCurrentRound().GetNextATL( this ) != null)
                {
                    //base.SetCurrentCharacterActionType( CharacterActionType.Repulse );
                }

                break;

            case AnimationEvent.OnAttackPartB_Cutoff:
            case AnimationEvent.OnDefendPartA_Cutoff:
                break;
        }
    }
}
