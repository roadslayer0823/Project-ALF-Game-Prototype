using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill
{
    private SkillDatabase.SkillData skillData = null;

    public CharacterSkill( SkillDatabase.SkillData skillData )
    {
        this.skillData = skillData;
    }

    public SkillDatabase.SkillData GetSkillData()
    {
        return this.skillData;
    }
}
