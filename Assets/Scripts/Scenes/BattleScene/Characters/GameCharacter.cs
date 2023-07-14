using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour
{
    protected int id = 0;
    protected float maximumHealthPoint = 0.0f;
    protected float remainingHealthPoint = 0.0f;
    protected float maximumActionPoint = 0.0f;
    protected float remainingActionPoint = 0.0f;
    protected CharacterSkill[] skills = null;
    protected List<CharacterSkill> selectedActiveSkillList = null;
    protected List<CharacterSkill> selectedBackendSkillList = null;

    public void Initialize( CharacterDatabase.CharacterData characterData, SkillDatabase skillDatabase )
    {
        this.id = characterData.GetId();
        this.maximumHealthPoint = characterData.GetMaximumHealthPoint();
        this.remainingHealthPoint = this.maximumHealthPoint;
        this.maximumActionPoint = characterData.GetMaximumActionPoint();
        this.remainingActionPoint = this.maximumActionPoint;

        List<CharacterSkill> _skillList = new List<CharacterSkill>();
        int[] _skillIdArray = characterData.GetSkillIdArray();
        for (int i = 0; i < _skillIdArray.Length; i++)
        {
            _skillList.Add( new CharacterSkill( skillDatabase.GetSkillDataById( _skillIdArray[ i ] ) ) );
        }

        this.skills = _skillList.ToArray();
        this.selectedActiveSkillList = new List<CharacterSkill>();
        this.selectedBackendSkillList = new List<CharacterSkill>();
    }

    public void AddRemainingHealthPoint( float amount )
    {
        this.remainingHealthPoint += amount;
    }

    public void MinusRemainingHealthPoint( float amount )
    {
        this.remainingHealthPoint -= amount;
    }

    public void AddRemainingActionPoint( float amount )
    {
        this.remainingActionPoint += amount;
    }

    public void MinusRemainingActionPoint( float amount )
    {
        this.remainingActionPoint -= amount;
    }

    public void AddSelectedSkill( CharacterSkill skill )
    {
        if (skill.GetSkillData().GetSkillType() == SkillDatabase.SkillData.SkillType.Active)
        {
            if (this.selectedActiveSkillList.Count < GameConfiguration.BATTLE_MAXIMUM_SELECTED_ACTIVE_SKILLS)
            {
                this.selectedActiveSkillList.Add( skill );
            }
        }
        else
        {
            if (this.selectedBackendSkillList.Count < GameConfiguration.BATTLE_MAXIMUM_SELECTED_BACKEND_SKILLS)
            {
                this.selectedBackendSkillList.Add( skill );
            }
        }
    }

    public void RemoveSelectedSkill( CharacterSkill skill )
    {
        if (skill.GetSkillData().GetSkillType() == SkillDatabase.SkillData.SkillType.Active)
        {
            this.selectedActiveSkillList.Remove( skill );
        }
        else
        {
            this.selectedBackendSkillList.Remove( skill );
        }
    }

    public int GetId()
    {
        return this.id;
    }

    public float GetMaximumHealthPoint()
    {
        return this.maximumHealthPoint;
    }

    public float GetMaximumActionPoint()
    {
        return this.maximumActionPoint;
    }

    public CharacterSkill[] GetSkills()
    {
        return this.skills;
    }

    public List<CharacterSkill> GetSelectedActiveSkillList()
    {
        return this.selectedActiveSkillList;
    }

    public List<CharacterSkill> GetSelectedBackendSkillList()
    {
        return this.selectedBackendSkillList;
    }
}
