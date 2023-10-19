using UnityEngine;
using Skill = DatabaseManager.Skill;

public class ObservedSkillData
{
    private int featureId = 0;
    private string skillName = "";
    private Skill.SkillType skillType = Skill.SkillType.none;
    private float maximumObservedRate = 0.0f;
    private float currentObservedRate = 0.0f;
    private int roundNumber = 0;

    public ObservedSkillData( int featureId, string skillName, Skill.SkillType skillType, float maximumObservedRate )
    {
        this.featureId = featureId;
        this.skillName = skillName;
        this.skillType = skillType;
        this.maximumObservedRate = maximumObservedRate;
        this.currentObservedRate = 0.0f;
        this.roundNumber = 0;
    }

    public float IncreaseObservedRate( float amount )
    {
        this.currentObservedRate = Mathf.Clamp( this.currentObservedRate + amount, 0.0f, this.maximumObservedRate );
        return this.currentObservedRate;
    }

    public float DecreaseObservedRate( float amount )
    {
        this.currentObservedRate = Mathf.Clamp( this.currentObservedRate - amount, 0.0f, this.maximumObservedRate );
        return this.currentObservedRate;
    }

    public int IncreaseRoundNumber()
    {
        this.roundNumber++;
        return this.roundNumber;
    }

    public int GetFeatureId()
    {
        return this.featureId;
    }

    public string GetSkillName()
    {
        return this.skillName;
    }

    public Skill.SkillType GetSkillType()
    {
        return this.skillType;
    }

    public float GetCurrentObservedRate()
    {
        return this.currentObservedRate;
    }

    public int GetRoundNumber()
    {
        return this.roundNumber;
    }
}
