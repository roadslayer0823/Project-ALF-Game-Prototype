using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
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
    protected float currentHealthPoint = 0.0f;
    protected float originalStatePoint = 0.0f;
    protected float maximumStatePoint = 0.0f;
    protected float currentStatePoint = 0.0f;
    protected float maximumStressValue = 0.0f;
    protected float currentStressValue = 0.0f;
    protected CharacterSkill[] skills = null;
    protected List<CharacterSkill> selectedActiveSkillList = null;
    protected List<CharacterSkill> selectedBackendSkillList = null;
    protected CharacterActionType currentCharacterActionType = CharacterActionType.None;

    protected GameObject ownContainer = null;
    protected GameObject opponentContainer = null;
    protected GameCharacterInfoBox gameCharacterInfoBox = null;

    protected Action<AnimationEvent,GameCharacter> onEventTriggeredCallback = null;
    protected Action<string> onCharacterAnimationTriggeredCallback = null;
    protected Action<string> onSkillEffectAnimationTriggeredCallback = null;
    protected Action onCharacterInfoUpdated = null;

    private CharacterSkill currentSkill = null;
    private GameCharacter currentAttacker = null;
    private float skillCountdownTime = 0.0f;
    private int counterAttacks = 0;
    private int breakStatusRemainingATLs = 0;
    private bool isBreakStatusCausedByStatePoint = false;
    private bool isBreakStatusCausedByStressValue = false;

    public enum CharacterActionType
    {
        None,
        Repulse,
        Derive,
        Counter,
        Backend
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
        this.currentHealthPoint = this.maximumHealthPoint;
        this.originalStatePoint = characterData.MaximumStatePoint;
        this.maximumStatePoint = this.originalStatePoint;
        this.currentStatePoint = this.maximumStatePoint;
        this.maximumStressValue = characterData.MaximumStressValue;
        this.currentStressValue = 0.0f;

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

    public virtual void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
    }

    public void AddCurrentHealthPoint( float amount )
    {
        if (amount > 0)
        {
            this.currentHealthPoint = Mathf.Clamp( this.currentHealthPoint + amount, 0.0f, this.maximumHealthPoint );
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void MinusCurrentHealthPoint( float amount )
    {
        if (amount > 0)
        {
            this.currentHealthPoint -= amount;
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void AddCurrentStatePoint( float amount )
    {
        if (amount > 0)
        {
            this.currentStatePoint = Mathf.Clamp( this.currentStatePoint + amount, 0.0f, this.maximumStatePoint );
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void MinusCurrentStatePoint( float amount, bool onHit )
    {
        if (amount > 0)
        {
            this.currentStatePoint -= amount;

            if (BattleLogicManager.IsGameCharacterInBreakStatus( this, onHit ))
            {
                this.isBreakStatusCausedByStatePoint = true;

                if (!GetIsInBreakStatus())
                {
                    EnterIntoBreakStatus( 1 );
                }
            }

            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void SetCurrentStatePointToMaximum()
    {
        this.currentStatePoint = this.maximumStatePoint;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public void AddMaximumStatePoint( float amount )
    {
        if (amount > 0)
        {
            this.maximumStatePoint += amount;
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void MinusMaximumStatePoint( float amount )
    {
        if (amount > 0)
        {
            this.maximumStatePoint -= amount;

            float _lowestMaximumStatePoint = GameConfiguration.Instance.GetBattleConfiguration().GetLowestMaximumStatePoint();
            if (this.maximumStatePoint < _lowestMaximumStatePoint)
            {
                this.maximumStatePoint = _lowestMaximumStatePoint;
            }

            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void AddCurrentStressValue( float amount )
    {
        if (amount > 0)
        {
            if (!this.isBreakStatusCausedByStressValue)
            {
                this.currentStressValue += amount;

                if (BattleLogicManager.IsGameCharacterInBreakStatus( this ))
                {
                    this.currentStressValue = this.maximumStressValue;
                    this.isBreakStatusCausedByStressValue = true;

                    if (!GetIsInBreakStatus())
                    {
                        EnterIntoBreakStatus( 1 );
                    }
                }
            }

            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void MinusCurrentStressValue( float amount )
    {
        if (amount > 0)
        {
            this.currentStressValue -= amount;
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void MinusBreakStatusRemainingATLs()
    {
        if (this.breakStatusRemainingATLs > 0)
        {
            this.breakStatusRemainingATLs--;

            if (!GetIsInBreakStatus())
            {
                if (this.isBreakStatusCausedByStressValue)
                {
                    this.isBreakStatusCausedByStressValue = false;
                    this.currentStressValue = 0.0f;
                }

                if (this.isBreakStatusCausedByStatePoint)
                {
                    this.isBreakStatusCausedByStatePoint = false;
                    this.maximumStatePoint = this.originalStatePoint;
                    this.currentStatePoint = this.maximumStatePoint;
                }

                this.onCharacterInfoUpdated?.Invoke();
            }
        }
    }

    public void AddSelectedSkill(CharacterSkill characterSkill)
    {
        if (characterSkill.GetSkillData().skillType == Skill.SkillType.active)
        {
            if (this.selectedActiveSkillList.Count < GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedActiveSkills())
            {
                this.selectedActiveSkillList.Add(characterSkill);
            }
        }
        else
        {
            if (this.selectedBackendSkillList.Count < GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedBackendSkills())
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

    public bool IsAbleToRepulse( BattleFlowATL nextATL )
    {
        return IsAbleToRepulse( nextATL, out _ );
    }

    public bool IsAbleToRepulse( BattleFlowATL nextATL, out CharacterSkill repulseSkill )
    {
        repulseSkill = null;

        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        Subskill _attackerSubskillData = this.currentAttacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        if (!_attackerSubskillData.IsInterceptable)
        {
            return false;
        }

        if (nextATL == null)
        {
            return false;
        }

        repulseSkill = nextATL.GetSelectedSkill().GetCharacterSubskillData().GetRepulseSkill();

        if (repulseSkill == null)
        {
            return false;
        }

        if (( int )_attackerSubskillData.EffectType > ( int )repulseSkill.GetCharacterSubskillData().GetSubskillData().EffectType)
        {
            return false;
        }

        return true;
    }

    public bool IsAbleToDerive()
    {
        return IsAbleToDerive( out _ );
    }

    public bool IsAbleToDerive( out CharacterSkill derivedSkill )
    {
        derivedSkill = null;

        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        derivedSkill = this.currentSkill.GetCharacterSubskillData().GetDerivedSkill();

        if (derivedSkill == null)
        {
            return false;
        }

        return true;
    }

    public bool IsAbleToCounter()
    {
        return IsAbleToCounter( out _ );
    }

    public bool IsAbleToCounter( out CharacterSkill counterSkill )
    {
        counterSkill = null;

        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        if (BattleLogicManager.HasGameCharacterReachedCounterAttackLimit( this ))
        {
            return false;
        }

        counterSkill = this.currentSkill.GetCharacterSubskillData().GetCounterSkill();

        if (counterSkill == null)
        {
            return false;
        }

        return true;
    }

    public bool IsAbleToDefend()
    {
        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        return true;
    }

    public void EnterIntoBreakStatus( int numberOfATLs )
    {
        this.breakStatusRemainingATLs = numberOfATLs;
        TriggerEvent( AnimationEvent.OnBeingInBreakStatus );
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

    public float GetCurrentHealthPoint()
    {
        return this.currentHealthPoint;
    }

    public float GetMaximumStatePoint()
    {
        return this.maximumStatePoint;
    }

    public float GetCurrentStatePoint()
    {
        return this.currentStatePoint;
    }

    public float GetMaximumStressValue()
    {
        return this.maximumStressValue;
    }

    public float GetCurrentStressValue()
    {
        return this.currentStressValue;
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

    public void SetCurrentSkill( CharacterSkill currentSkill, GameCharacter attackTarget = null )
    {
        this.currentSkill = currentSkill;

        if (attackTarget != null)
        {
            attackTarget.SetCurrentAttacker( this );
        }

        this.onCharacterInfoUpdated?.Invoke();
    }

    public CharacterSkill GetCurrentSkill()
    {
        return this.currentSkill;
    }

    public void SetCurrentAttacker( GameCharacter currentAttacker )
    {
        this.currentAttacker = currentAttacker;
    }

    public GameCharacter GetCurrentAttacker()
    {
        return this.currentAttacker;
    }

    public void SetSkillCountdownTime( float skillCountdownTime )
    {
        this.skillCountdownTime = skillCountdownTime;
    }

    public float GetSkillCountdownTime()
    {
        return this.skillCountdownTime;
    }

    public void IncreaseCounterAttacks()
    {
        this.counterAttacks++;
    }

    public void ResetCounterAttacks()
    {
        this.counterAttacks = 0;
    }

    public int GetCounterAttacks()
    {
        return this.counterAttacks;
    }

    public int GetBreakStatusRemainingATLs()
    {
        return this.breakStatusRemainingATLs;
    }

    public GameObject GetOwnContainer()
    {
        return this.ownContainer;
    }

    public GameObject GetOpponentContainer()
    {
        return this.opponentContainer;
    }

    public bool GetIsInBreakStatus()
    {
        return ( this.breakStatusRemainingATLs > 0 );
    }

    public bool GetIsBreakStatusCausedByStatePoint()
    {
        return this.isBreakStatusCausedByStatePoint;
    }

    public bool GetIsBreakStatusCausedByStressValue()
    {
        return this.isBreakStatusCausedByStressValue;
    }

    public void Reset()
    {
        this.currentCharacterActionType = CharacterActionType.None;
        SetCurrentSkill( null );
        SetCurrentAttacker( null );
    }
}
