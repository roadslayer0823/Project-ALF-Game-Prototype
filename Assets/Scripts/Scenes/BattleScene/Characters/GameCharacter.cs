using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static DatabaseManager;

public class GameCharacter : MonoBehaviour
{
    [SerializeField] private SortingGroup sortingGroup = null;
    [SerializeField] private Animator characterAnimator = null;
    [SerializeField] private Animator skillEffectAnimator = null;

    protected string id = null;
    protected float maximumHealthPoint = 0.0f;
    protected float remainingHealthPoint = 0.0f;
    protected float maximumStatePoint = 0.0f;
    protected float remainingStatePoint = 0.0f;
    protected Skill[] skills = null;
    protected List<Skill> selectedActiveSkillList = null;
    protected List<Skill> selectedBackendSkillList = null;
    protected GameObject ownContainer = null;
    protected GameObject opponentContainer = null;

    protected Action<string> onCharacterAnimationTriggeredCallback = null;
    protected Action<string> onSkillEffectAnimationTriggeredCallback = null;

    public void Initialize( Character characterData, GameObject ownContainer, GameObject opponentContainer )
    {
        base.gameObject.name = "Character: " + characterData.GetDisplayName();

        this.id = characterData.GetId();
        this.maximumHealthPoint = characterData.GetMaximumHealthPoint();
        this.remainingHealthPoint = this.maximumHealthPoint;
        this.maximumStatePoint = characterData.GetMaximumStatePoint();
        this.remainingStatePoint = this.maximumStatePoint;

        List<Skill> _skillList = new List<Skill>();
        string[] _skillIdArray = characterData.GetSkillIdArray();
        for (int i = 0; i < _skillIdArray.Length; i++)
        {
            _skillList.Add(DatabaseManager.Instance.GetSkillDataById(_skillIdArray[i]) );
        }

        this.skills = _skillList.ToArray();
        this.selectedActiveSkillList = new List<Skill>();
        this.selectedBackendSkillList = new List<Skill>();

        this.ownContainer = ownContainer;
        this.opponentContainer = opponentContainer;
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
        this.remainingStatePoint += amount;
    }

    public void MinusRemainingActionPoint( float amount )
    {
        this.remainingStatePoint -= amount;
    }

    public void AddSelectedSkill(Skill skill)
    {
        if (skill.GetSkillType() == Skill.SkillType.active)
        {
            if (this.selectedActiveSkillList.Count < GameConfiguration.BATTLE_MAXIMUM_SELECTED_ACTIVE_SKILLS)
            {
                this.selectedActiveSkillList.Add(skill);
            }
        }
        else
        {
            if (this.selectedBackendSkillList.Count < GameConfiguration.BATTLE_MAXIMUM_SELECTED_BACKEND_SKILLS)
            {
                this.selectedBackendSkillList.Add(skill);
            }
        }
    }

    public void RemoveSelectedSkill(Skill skill)
    {
        if (skill.GetSkillType() == Skill.SkillType.active)
        {
            this.selectedActiveSkillList.Remove(skill);
        }
        else
        {
            this.selectedBackendSkillList.Remove(skill);
        }
    }

    public void PlayCharacterAnimation( string animationName, Action<string> onAnimationTriggeredCallback = null )
    {
        this.onCharacterAnimationTriggeredCallback = onAnimationTriggeredCallback;
        this.characterAnimator.Play( animationName );
    }

    public void PlaySkillEffectAnimation( string animationName, Action<string> onAnimationTriggeredCallback = null )
    {
        this.onSkillEffectAnimationTriggeredCallback = onAnimationTriggeredCallback;
        this.skillEffectAnimator.Play( animationName );
    }

    public void OnCharacterAnimationTriggered( string parameterValue )
    {
        if (this.onCharacterAnimationTriggeredCallback != null)
        {
            this.onCharacterAnimationTriggeredCallback( parameterValue );
            this.onCharacterAnimationTriggeredCallback = null;
        }
    }

    public void OnSkillEffectAnimationTriggered( string parameterValue )
    {
        if (this.onSkillEffectAnimationTriggeredCallback != null)
        {
            this.onSkillEffectAnimationTriggeredCallback( parameterValue );
            this.onSkillEffectAnimationTriggeredCallback = null;
        }
    }

    public string GetId()
    {
        return this.id;
    }

    public float GetMaximumHealthPoint()
    {
        return this.maximumHealthPoint;
    }

    public float GetMaximumActionPoint()
    {
        return this.maximumStatePoint;
    }

    public Skill[] GetSkills()
    {
        return skills;
    }

    public List<Skill> GetSelectedActiveSkillList()
    {
        return this.selectedActiveSkillList;
    }

    public void SetSelectedActiveSkillList(List<Skill> selectedActiveSkillList)
    {
        this.selectedActiveSkillList = selectedActiveSkillList;
    }

    public List<Skill> GetSelectedBackendSkillList()
    {
        return this.selectedBackendSkillList;
    }

    public SortingGroup GetSortingGroup()
    {
        return this.sortingGroup;
    }

    public Animator GetCharacterAnimator()
    {
        return this.characterAnimator;
    }

    public Animator GetSkillEffectAnimator()
    {
        return this.skillEffectAnimator;
    }

    public GameObject GetOwnContainer()
    {
        return this.ownContainer;
    }

    public GameObject GetOpponentContainer()
    {
        return this.opponentContainer;
    }
}
