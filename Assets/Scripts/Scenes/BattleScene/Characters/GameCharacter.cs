using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Skill = DatabaseManager.Skill;
using Character = DatabaseManager.Character;
using AnimationEvent = BattleAnimationManager.AnimationEvent;

public class GameCharacter : MonoBehaviour
{
    [SerializeField] private SortingGroup sortingGroup = null;
    [SerializeField] private Animator characterAnimator = null;
    [SerializeField] private Animator skillEffectAnimator = null;

    protected string id = null;
    protected string characterName = null;
    protected float maximumHealthPoint = 0.0f;
    protected float remainingHealthPoint = 0.0f;
    protected float maximumStatePoint = 0.0f;
    protected float remainingStatePoint = 0.0f;
    protected CharacterSkill[] skills = null;
    protected List<CharacterSkill> selectedActiveSkillList = null;
    protected List<CharacterSkill> selectedBackendSkillList = null;
    protected CharacterActionType currentCharacterActionType = CharacterActionType.None;
    protected CharacterSkill currentSkill = null;

    protected GameObject ownContainer = null;
    protected GameObject opponentContainer = null;
    protected GameCharacterInfoBox gameCharacterInfoBox = null;

    protected Action<AnimationEvent,GameCharacter> onEventTriggeredCallback = null;
    protected Action<string> onCharacterAnimationTriggeredCallback = null;
    protected Action<string> onSkillEffectAnimationTriggeredCallback = null;
    protected Action onCharacterInfoUpdated = null;

    public enum CharacterActionType
    {
        None,
        Repulse,
        Defend,
        Evade,
        Counter,
        Derive
    }

    public void Initialize( Character characterData, GameObject ownContainer, GameObject opponentContainer, Action<AnimationEvent,GameCharacter> onEventTriggeredCallback = null )
    {
        base.gameObject.name = "Character: " + characterData.DisplayName;
        this.ownContainer = ownContainer;
        this.opponentContainer = opponentContainer;
        this.onEventTriggeredCallback = onEventTriggeredCallback;

        this.id = characterData.Id;
        this.characterName = characterData.DisplayName;
        this.maximumHealthPoint = characterData.MaximumHealthPoint;
        this.remainingHealthPoint = this.maximumHealthPoint;
        this.maximumStatePoint = characterData.MaximumStatePoint;
        this.remainingStatePoint = this.maximumStatePoint;

        List<CharacterSkill> _skillList = new List<CharacterSkill>();
        string[] _skillIdArray = characterData.SkillIdArray;
        for (int i = 0; i < _skillIdArray.Length; i++)
        {
            _skillList.Add(new CharacterSkill(DatabaseManager.Instance.GetSkillDataById(_skillIdArray[i]), this));
        }

        this.skills = _skillList.ToArray();
        for (int i = 0; i < this.skills.Length; i++)
        {
            CharacterSkill _characterSkill = this.skills[i];
            _characterSkill.SetupCharacterSubskillList();
        }

        this.selectedActiveSkillList = new List<CharacterSkill>();
        this.selectedBackendSkillList = new List<CharacterSkill>();

        this.onCharacterInfoUpdated?.Invoke();
    }

    public virtual void OnBattleFlowATLInitialized( BattleFlowATL battleFlowATL )
    {
    }

    public virtual void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
    }

    public void AddRemainingHealthPoint( float amount )
    {
        this.remainingHealthPoint = Mathf.Clamp( this.remainingHealthPoint + amount, 0, this.maximumHealthPoint );
        this.onCharacterInfoUpdated?.Invoke();
    }

    public void MinusRemainingHealthPoint( float amount )
    {
        this.remainingHealthPoint -= amount;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public void AddRemainingStatePoint( float amount )
    {
        this.remainingStatePoint = Mathf.Clamp( this.remainingStatePoint + amount, 0, this.maximumStatePoint );
        this.onCharacterInfoUpdated?.Invoke();
    }

    public void MinusRemainingStatePoint( float amount )
    {
        this.remainingStatePoint -= amount;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public void SetRemainingStatePointToMaximum()
    {
        this.remainingStatePoint = this.maximumStatePoint;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public void AddSelectedSkill(CharacterSkill characterSkill)
    {
        if (characterSkill.GetSkillData().skillType == Skill.SkillType.active)
        {
            if (this.selectedActiveSkillList.Count < GameConfiguration.Battle.Instance.GetMaximumSelectedActiveSkills())
            {
                this.selectedActiveSkillList.Add(characterSkill);
            }
        }
        else
        {
            if (this.selectedBackendSkillList.Count < GameConfiguration.Battle.Instance.GetMaximumSelectedBackendSkills())
            {
                this.selectedBackendSkillList.Add(characterSkill);
            }
        }
    }

    public void RemoveSelectedSkill(CharacterSkill characterSkill)
    {
        if (characterSkill.GetSkillData().skillType == Skill.SkillType.active)
        {
            this.selectedActiveSkillList.Remove(characterSkill);
        }
        else
        {
            this.selectedBackendSkillList.Remove(characterSkill);
        }
    }

    public void PlayCharacterAnimation( string animationName, Action<string> onAnimationTriggeredCallback = null )
    {
        this.onCharacterAnimationTriggeredCallback = onAnimationTriggeredCallback;
        this.characterAnimator.Play( animationName, 0, 0.0f );
    }

    public void PlaySkillEffectAnimation( string animationName, Action<string> onAnimationTriggeredCallback = null )
    {
        this.onSkillEffectAnimationTriggeredCallback = onAnimationTriggeredCallback;
        this.skillEffectAnimator.Play( animationName, 0, 0.0f );
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

    public void SetGameCharacterInfoBox( GameCharacterInfoBox gameCharacterInfoBox )
    {
        this.gameCharacterInfoBox = gameCharacterInfoBox;
    }

    public void SetOnCharacterInfoUpdated( Action onCharacterInfoUpdated )
    {
        this.onCharacterInfoUpdated += onCharacterInfoUpdated;
    }

    public void ShowCharacterObject()
    {
        this.characterAnimator.gameObject.SetActive( true );
        this.gameCharacterInfoBox.gameObject.SetActive( true );
    }

    public void HideCharacterObject()
    {
        this.characterAnimator.gameObject.SetActive( false );
        this.gameCharacterInfoBox.gameObject.SetActive( false );
    }

    public void TriggerEvent( AnimationEvent animationEvent )
    {
        this.onEventTriggeredCallback?.Invoke( animationEvent, this );
    }

    public string GetId()
    {
        return this.id;
    }

    public string GetCharacterName()
    {
        return this.characterName;
    }

    public float GetMaximumHealthPoint()
    {
        return this.maximumHealthPoint;
    }

    public float GetRemainingHealthPoint()
    {
        return this.remainingHealthPoint;
    }

    public float GetMaximumStatePoint()
    {
        return this.maximumStatePoint;
    }

    public float GetRemainingStatePoint()
    {
        return this.remainingStatePoint;
    }

    public CharacterSkill[] GetSkills()
    {
        return skills;
    }

    public List<CharacterSkill> GetSelectedActiveSkillList()
    {
        return this.selectedActiveSkillList;
    }

    public void SetSelectedActiveSkillList(List<CharacterSkill> selectedActiveSkillList)
    {
        this.selectedActiveSkillList = selectedActiveSkillList;
    }

    public List<CharacterSkill> GetSelectedBackendSkillList()
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

    public void SetCurrentCharacterActionType( CharacterActionType currentCharacterActionType )
    {
        this.currentCharacterActionType = currentCharacterActionType;
    }

    public CharacterActionType GetCurrentCharacterActionType()
    {
        return this.currentCharacterActionType;
    }

    public void SetCurrentSkill( CharacterSkill currentSkill )
    {
        this.currentSkill = currentSkill;
    }

    public CharacterSkill GetCurrentSkill()
    {
        return this.currentSkill;
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
