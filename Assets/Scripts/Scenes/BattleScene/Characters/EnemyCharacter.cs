using System.Collections.Generic;
using UnityEngine;

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
}
