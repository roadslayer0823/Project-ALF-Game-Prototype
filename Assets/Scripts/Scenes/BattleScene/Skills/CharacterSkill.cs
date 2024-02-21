using System.Collections.Generic;
using System.Linq;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class CharacterSkill
{
    private GameCharacter owner = null;
    private Skill skillData = null;
    private List<CharacterSubskill> characterSubskillList = null;
    private int selectedSkillLevel = 1;

    private List<ObservedSkillData> observedSkillDataList = null;
    private GameCharacter observedSkillCaster = null;
    private CharacterSkill observedSkill = null;

    public const int SPEED_MINIMUM_SPECIAL_VALUE = 3;
    public const int STRENGTH_MINIMUM_SPECIAL_VALUE = 2;

    public CharacterSkill( Skill skillData, GameCharacter owner )
    {
        this.skillData = skillData;
        this.owner = owner;
        this.observedSkillDataList = new List<ObservedSkillData>();
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

        //Sort the skill based on skill level
        this.characterSubskillList = this.characterSubskillList.OrderBy(subskill => subskill.GetSubskillData().Level).ToList();
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
        for (int i = 0; i < this.characterSubskillList.Count; i++)
        {
            CharacterSubskill _characterSubskill = this.characterSubskillList[i];

            if (_characterSubskill.GetSubskillData().Level == this.selectedSkillLevel && _characterSubskill.GetSubskillData().IsAvailable)
            {
                return _characterSubskill;
            }
        }

        return null;
    }

    public void SetSelectedSkillLevel(int selectedSkillLevel)
    {
        this.selectedSkillLevel = selectedSkillLevel;
    }

    public int GetMinumumSkillLevel()
    {
        int _totalSubskill = this.characterSubskillList.Count;
        int _skillLevel = 1;

        for (int i = 0; i < _totalSubskill; i++)
        {
            CharacterSubskill _characterSubskill = this.characterSubskillList[i];
            if (_characterSubskill.GetSubskillData().Level == _skillLevel && _characterSubskill.GetSubskillData().IsAvailable)
            {
                return _skillLevel;
            }
            else if (_characterSubskill.GetSubskillData().Level != _skillLevel && _characterSubskill.GetSubskillData().IsAvailable)
            {
                return _skillLevel = _characterSubskill.GetSubskillData().Level;
            }
        }

        return 0;
    }

    public int GetMaximumSkillLevel()
    {
        int _totalSubskill = this.characterSubskillList.Count;
        int _skillLevel = this.characterSubskillList[_totalSubskill -1].GetSubskillData().Level;

        for (int i = _totalSubskill - 1; i >= 0; i--)
        {
            CharacterSubskill _characterSubskill = this.characterSubskillList[i];
            if (_characterSubskill.GetSubskillData().Level == _skillLevel && _characterSubskill.GetSubskillData().IsAvailable)
            {
                return _skillLevel;
            }
            else if (_characterSubskill.GetSubskillData().Level != _skillLevel && _characterSubskill.GetSubskillData().IsAvailable)
            {
                return _skillLevel = _characterSubskill.GetSubskillData().Level;
            }
        }

        return 0;
    }

    public bool IsSkillLevelAvailable(int skillLevel)
    {
        for (int i = 0; i < this.characterSubskillList.Count; i++)
        {
            CharacterSubskill _characterSubskill = this.characterSubskillList[i];
            if (_characterSubskill.GetSubskillData().Level == skillLevel && _characterSubskill.GetSubskillData().IsAvailable)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsSkillAvailable( bool canDefend, bool canEvade, bool canObserve )
    {
        Subskill _subskill = GetCharacterSubskillData().GetSubskillData();

        return ( ( canDefend && _subskill.IsDefendingSkill )
               || ( canEvade && _subskill.IsEvadingSkill )
               || ( canObserve && _subskill.IsObservingSkill ) );
    }

#region Observed Skill Data

    public float AddObservedSkillData( CharacterSkill characterSkill, float observedRate )
    {
        Skill.SkillType _skillType = characterSkill.GetSkillData().skillType;

        if (_skillType != Skill.SkillType.active
            && _skillType != Skill.SkillType.backend)
        {
            return 0.0f;
        }

        int _featureId = characterSkill.GetCharacterSubskillData().GetSubskillData().FeatureId;
        int _numberOfSkillDataMatched = 0;
        ObservedSkillData _firstSkillDataMatched = null;
        ObservedSkillData _skillDataFound = null;
        for (int i = 0; i < this.observedSkillDataList.Count; i++)
        {
            ObservedSkillData _observedSkillData = this.observedSkillDataList[ i ];

            if (_observedSkillData.GetFeatureId() == _featureId)
            {
                _skillDataFound = _observedSkillData;
                break;
            }

            if (_observedSkillData.GetSkillType() == _skillType)
            {
                if (_firstSkillDataMatched == null)
                {
                    _firstSkillDataMatched = _observedSkillData;
                }

                _numberOfSkillDataMatched++;
            }
        }

        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();

        if (_skillDataFound == null)
        {
            int _maximumObservedSkills = 0;
            if (_skillType == Skill.SkillType.active)
            {
                _maximumObservedSkills = _battleConfiguration.GetMaximumObservedActiveSkills();
            }
            else if (_skillType == Skill.SkillType.backend)
            {
                _maximumObservedSkills = _battleConfiguration.GetMaximumObservedBackendSkills();
            }

            if (_numberOfSkillDataMatched >= _maximumObservedSkills)
            {
                this.observedSkillDataList.Remove( _firstSkillDataMatched );
            }

            _skillDataFound = new ObservedSkillData( characterSkill, _battleConfiguration.GetMaximumObservedRate() );
            this.observedSkillDataList.Add( _skillDataFound );
        }

        return _skillDataFound.IncreaseObservedRate( observedRate );
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

    public void CleanUpObservedSkillDataList()
    {
        this.observedSkillDataList.RemoveAll( ( x ) => ( x.GetCurrentObservedRate() <= 0 ) );
    }

    public void ClearObservedSkillDataList()
    {
        this.observedSkillDataList.Clear();
    }

    public List<ObservedSkillData> GetObservedSkillDataList()
    {
        return this.observedSkillDataList;
    }

    public void SetObservedSkill( GameCharacter observedSkillCaster, CharacterSkill observedSkill )
    {
        this.observedSkillCaster = observedSkillCaster;
        this.observedSkill = observedSkill;
    }

    public GameCharacter GetObservedSkillCaster()
    {
        return this.observedSkillCaster;
    }

    public CharacterSkill GetObservedSkill()
    {
        return this.observedSkill;
    }

#endregion
}
