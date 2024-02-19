using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class ObservedSkillData
{
    private CharacterSkill characterSkill = null;
    private Skill skillData = null;
    private Subskill subskillData = null;

    private float maximumObservedRate = 0.0f;
    private float currentObservedRate = 0.0f;
    private int roundNumber = 0;

    public ObservedSkillData( CharacterSkill characterSkill, float maximumObservedRate )
    {
        this.characterSkill = characterSkill;
        this.skillData = this.characterSkill.GetSkillData();
        this.subskillData = this.characterSkill.GetCharacterSubskillData().GetSubskillData();

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

    public CharacterSkill GetCharacterSkill()
    {
        return this.characterSkill;
    }

    public float GetCurrentObservedRate()
    {
        return this.currentObservedRate;
    }

    public int GetRoundNumber()
    {
        return this.roundNumber;
    }

    public int GetFeatureId()
    {
        return this.subskillData.FeatureId;
    }

    public string GetSkillName()
    {
        return this.subskillData.DisplayName;
    }

    public Skill.SkillType GetSkillType()
    {
        return this.skillData.skillType;
    }

    public string GetSkillIconFilePath()
    {
        return this.subskillData.IconFilePathOn;
    }
}
