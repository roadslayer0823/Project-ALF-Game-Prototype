using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class CharacterSkill
{
    private GameCharacter owner = null;
    private Skill skillData = null;
    private List<CharacterSubskill> characterSubskillList = null;
    private int selectedSkillLevel = 1;

    private List<ObservedSkillData> observedSkillDataList = null;

    public CharacterSkill( Skill skillData, GameCharacter owner )
    {
        this.skillData = skillData;
        this.owner = owner;
    }

    public void SetupCharacterSubskillList()
    {
        List<Subskill> _subskillList = DatabaseManager.Instance.GetSubskillList();
        this.characterSubskillList = new List<CharacterSubskill>();

        foreach (Subskill subskill in _subskillList)
        {
            if (subskill.SkillId == this.skillData.Id)
            {
                this.characterSubskillList.Add(new CharacterSubskill(subskill, owner));
            }
        }
    }

    public Skill GetSkillData()
    {
        return this.skillData;
    }

    public List<CharacterSubskill> GetCharacterSubskillList()
    {
        return this.characterSubskillList;
    }

    public CharacterSubskill GetCharacterSubskillData()
    {
        int _maxLevel = this.characterSubskillList.Count;

        if (this.selectedSkillLevel > _maxLevel)
        {
            return this.characterSubskillList[_maxLevel - 1];
        }
        else
        {
            foreach (CharacterSubskill characterSubskill in this.characterSubskillList)
            {
                if (characterSubskill.GetSubskillData().Level == this.selectedSkillLevel)
                {
                    return characterSubskill;
                }
            }
        }

        return null;
    }

    public void SetSelectedSkillLevel(int selectedSkillLevel)
    {
        this.selectedSkillLevel = selectedSkillLevel;
    }

    public int GetMaximumSkillLevel()
    {
        return this.characterSubskillList.Count;
    }

    public bool IsSkillAvailable( bool canDefend, bool canEvade, bool canObserve )
    {
        Subskill _subskill = GetCharacterSubskillData().GetSubskillData();

        return ( ( canDefend && _subskill.IsDefendingSkill )
               || ( canEvade && _subskill.IsEvadingSkill )
               || ( canObserve && _subskill.IsObservingSkill ) );
    }

#region Observed Skill Data

    public void AddObservedSkillData( int featureId )
    {
        this.observedSkillDataList.Add( new ObservedSkillData( featureId, GameConfiguration.Instance.GetBattleConfiguration().GetMaximumObservedRate() ) );
    }

    public ObservedSkillData GetObservedSkillData( int featureId )
    {
        for (int i = 0; i < this.observedSkillDataList.Count; i++)
        {
            ObservedSkillData _observedSkillData = this.observedSkillDataList[ i ];
            if (_observedSkillData.GetFeatureId() == featureId)
            {
                return _observedSkillData;
            }
        }

        return null;
    }

    public void ClearObservedSkillDataList()
    {
        this.observedSkillDataList.Clear();
    }

#endregion
}
