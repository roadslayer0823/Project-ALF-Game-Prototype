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

        skills = _skillList.ToArray();
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
        return skills;
    }
}
