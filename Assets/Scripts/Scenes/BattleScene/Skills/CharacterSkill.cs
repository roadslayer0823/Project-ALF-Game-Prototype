using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill : MonoBehaviour
{
    private int id = 0;
    private SkillDatabase.SkillData skillData = null;

    public CharacterSkill( int id, SkillDatabase.SkillData skillData )
    {
        this.id = id;
        this.skillData = skillData;
    }

    public int GetId()
    {
        return this.id;
    }

    public SkillDatabase.SkillData GetSkillData()
    {
        return this.skillData;
    }
}
