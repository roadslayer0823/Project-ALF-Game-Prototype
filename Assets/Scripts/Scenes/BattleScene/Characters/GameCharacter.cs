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
    [SerializeField] private Transform pivot = null;
    [SerializeField] private PopUpDisplayInfo popUpDisplayInfoPrefab = null;

    [SerializeField] private UnityEventHandler characterEventHandler = null;
    [SerializeField] private UnityEventHandler skillEffectEventHandler = null;

    protected string id = null;
    protected string characterName = null;
    protected float maximumHealthPoint = 0.0f;
    protected float currentHealthPoint = 0.0f;
    protected float virtualHealthPoint = 0.0f;
    protected float originalStatePoint = 0.0f;
    protected float maximumStatePoint = 0.0f;
    protected float minimumStatePoint = 0.0f;
    protected float currentStatePoint = 0.0f;
    protected float maximumStressValue = 0.0f;
    protected float currentStressValue = 0.0f;
    protected CharacterSkill[] skills = null;
    protected List<CharacterSkill> selectedActiveSkillList = null;
    protected List<CharacterSkill> selectedBackendSkillList = null;

    protected GameObject ownContainer = null;
    protected GameObject opponentContainer = null;
    protected GameCharacterInfoBox gameCharacterInfoBox = null;

    protected Action<AnimationEvent,GameCharacter> onEventTriggeredCallback = null;
    protected Action<string> onCharacterAnimationTriggeredCallback = null;
    protected Action<string> onSkillEffectAnimationTriggeredCallback = null;
    protected Action onCharacterInfoUpdated = null;

    private CharacterSkill currentSkill = null;
    private int currentSkillStatIncrement = 0;
    private GameCharacter currentAttacker = null;
    private float skillCountdownTime = 0.0f;
    private int counterAttacks = 0;
    private bool isAbleToUseSkill = false;
    private List<PopUpDisplayInfo> popUpDisplayInfoList = new List<PopUpDisplayInfo>();

    // Break Status
    private int breakStatusRemainingATLs = 0;
    private bool isBreakStatusCausedByStatePoint = false;
    private bool isBreakStatusCausedByStressValue = false;

    // Energy Marker
    private int energyMarkerRemainingATLs = 0;

    private const string AUDIO_ID_BREAK = "break";

    public enum CharacterActionType
    {
        None,
        Repulse,
        Derive,
        Counter,
        Defend,
        Evade
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
        this.virtualHealthPoint = this.maximumHealthPoint;
        this.originalStatePoint = characterData.MaximumStatePoint;
        this.maximumStatePoint = this.originalStatePoint;
        this.minimumStatePoint = GameConfiguration.Instance.GetBattleConfiguration().GetMinimumCurrentStatePoint();
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

        this.characterEventHandler.GetCallback().AddListener( OnCharacterAnimationTriggered );
        this.skillEffectEventHandler.GetCallback().AddListener( OnSkillEffectAnimationTriggered );
    }

    public virtual void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
    }

    public float AddCurrentHealthPoint( float amount )
    {
        return AddCurrentHealthPoint( amount, this.maximumHealthPoint );
    }

    public float RecoverCurrentHealthPoint( float amount )
    {
        return AddCurrentHealthPoint( amount, this.virtualHealthPoint );
    }

    public float AddCurrentHealthPoint( float amount, float maximumAmount )
    {
        if (amount > 0)
        {
            float _lastValue = this.currentHealthPoint;

            this.currentHealthPoint = Mathf.Clamp( this.currentHealthPoint + amount, 0.0f, maximumAmount );

            if (this.virtualHealthPoint < this.currentHealthPoint)
            {
                this.virtualHealthPoint = this.currentHealthPoint;
            }

            this.onCharacterInfoUpdated?.Invoke();

            return ( this.currentHealthPoint - _lastValue );
        }

        return 0.0f;
    }

    public void MinusCurrentHealthPoint( float amount )
    {
        if (amount > 0)
        {
            this.currentHealthPoint -= amount;
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void AddVirtualHealthPoint(float amount)
    {
        if (amount > 0)
        {
            this.virtualHealthPoint = Mathf.Clamp(this.virtualHealthPoint + amount, 0.0f, this.maximumHealthPoint);
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void MinusVirtualHealthPoint(float amount)
    {
        if (amount > 0)
        {
            this.virtualHealthPoint -= amount;
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void RecoverHealthPointToVirtualHP()
    {
        this.currentHealthPoint = this.virtualHealthPoint;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public float ClearVirtualHealthPoint()
    {
        float _difference = this.virtualHealthPoint - this.currentHealthPoint;

        this.virtualHealthPoint = this.currentHealthPoint;
        this.onCharacterInfoUpdated?.Invoke();

        return _difference;
    }

    private void SetCurrentStatePoint( float amount )
    {
        this.currentStatePoint = Mathf.Clamp( amount, this.minimumStatePoint, this.maximumStatePoint );
    }

    public void AddCurrentStatePoint( float amount )
    {
        if (amount > 0)
        {
            SetCurrentStatePoint( this.currentStatePoint + amount );
            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void MinusCurrentStatePoint( float amount, bool onHit )
    {
        if (amount > 0)
        {
            SetCurrentStatePoint( this.currentStatePoint - amount );

            if (BattleLogicManager.IsGameCharacterInBreakStatus( this, onHit ))
            {
                this.isBreakStatusCausedByStatePoint = true;

                if (!GetIsInBreakStatus())
                {
                    EnterIntoBreakStatus( 1 );
                }

                AudioManager.Instance.PlaySoundEffect(AUDIO_ID_BREAK);
            }

            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public void SetCurrentStatePointToMaximum()
    {
        this.currentStatePoint = this.maximumStatePoint;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public float AddMaximumStatePoint( float amount )
    {
        if (amount > 0)
        {
            float _lastValue = this.maximumStatePoint;

            this.maximumStatePoint += amount;
            this.onCharacterInfoUpdated?.Invoke();

            return ( this.maximumStatePoint - _lastValue );
        }

        return 0.0f;
    }

    public float MinusMaximumStatePoint( float amount )
    {
        if (amount > 0)
        {
            float _lastValue = this.maximumStatePoint;

            this.maximumStatePoint -= amount;

            float _lowestMaximumStatePoint = GameConfiguration.Instance.GetBattleConfiguration().GetLowestMaximumStatePoint();
            if (this.maximumStatePoint < _lowestMaximumStatePoint)
            {
                this.maximumStatePoint = _lowestMaximumStatePoint;
            }

            this.onCharacterInfoUpdated?.Invoke();

            return ( _lastValue - this.maximumStatePoint );
        }

        return 0.0f;
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

    public float MinusCurrentStressValue( float amount )
    {
        if (amount > 0)
        {
            float _lastValue = this.currentStressValue;

            this.currentStressValue -= amount;

            if (this.currentStressValue < 0)
            {
                this.currentStressValue = 0.0f;
            }

            this.onCharacterInfoUpdated?.Invoke();

            return ( _lastValue - this.currentStressValue );
        }

        return 0.0f;
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
        else if (characterSkill.GetSkillData().skillType == Skill.SkillType.backend)
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
        else if (characterSkill.GetSkillData().skillType == Skill.SkillType.backend)
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
        }
    }

    public void OnSkillEffectAnimationTriggered( string parameterValue )
    {
        if (this.onSkillEffectAnimationTriggeredCallback != null)
        {
            this.onSkillEffectAnimationTriggeredCallback( parameterValue );
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

    public bool IsAbleToRepulse( BattleGameManager battleGameManager )
    {
        return IsAbleToRepulse( battleGameManager, out _, out _ );
    }

    public bool IsAbleToRepulse( BattleGameManager battleGameManager, out CharacterSkill repulseSkill, out bool isSpecial )
    {
        repulseSkill = null;
        isSpecial = false;

        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        Subskill _attackerSubskillData = this.currentAttacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        if (!_attackerSubskillData.IsInterceptable)
        {
            return false;
        }

        BattleFlowATL _nextATL = battleGameManager.GetBattleFlowManager().GetCurrentRound().GetNextATL( this );
        if (_nextATL != null)
        {
            repulseSkill = _nextATL.GetSelectedSkill().GetCharacterSubskillData().GetSelectedRepulseSkill();
        }
        else
        {
            return false;
        }

        if (repulseSkill != null
            && ( ( int )_attackerSubskillData.EffectType > ( int )repulseSkill.GetCharacterSubskillData().GetSubskillData().EffectType ))
        {
            repulseSkill = null;
        }

        if (repulseSkill == null)
        {
            SkillSlotListPanel _skillSlotListPanel = battleGameManager.GetBattleUiManager().GetSkillSlotListPanel();
            SkillSlot[] _skillSlots = _skillSlotListPanel.GetSkillSlots();

            for (int i = 0; i < _skillSlots.Length; i++)
            {
                CharacterSkill _slotSkill = _skillSlots[ i ].GetSelectedSkill();
                if (_slotSkill != null)
                {
                    CharacterSkill _slotRepulseSkill = _slotSkill.GetCharacterSubskillData().GetSelectedRepulseSkill();
                    if (( int )_attackerSubskillData.EffectType <= ( int )_slotRepulseSkill.GetCharacterSubskillData().GetSubskillData().EffectType)
                    {
                        repulseSkill = _slotRepulseSkill;
                        isSpecial = true;
                        break;
                    }
                }
            }
        }

        if (repulseSkill == null)
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

        derivedSkill = this.currentSkill.GetCharacterSubskillData().GetSelectedDerivedSkill();

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

        counterSkill = this.currentSkill.GetCharacterSubskillData().GetSelectedCounterSkill();

        if (counterSkill == null)
        {
            return false;
        }

        return true;
    }

    public bool IsAbleToUseBackendSkill( CharacterSkill backendSkill )
    {
        if (this.GetIsInBreakStatus())
        {
            return false;
        }

        Subskill _backendSubskillData = backendSkill.GetCharacterSubskillData().GetSubskillData();
        if (_backendSubskillData.IsEvadingSkill)
        {
            if (this.currentStatePoint <= 0)
            {
                return false;
            }

            Subskill _attackerSubskillData = this.currentAttacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
            if (( int )_attackerSubskillData.EffectType > ( int )_backendSubskillData.EffectType)
            {
                return false;
            }
        }

        return true;
    }

    public void EnterIntoBreakStatus( int numberOfATLs )
    {
        this.breakStatusRemainingATLs = numberOfATLs;
        TriggerEvent( AnimationEvent.OnBeingInBreakStatus );
        BattleLogicManager.OnCharacterEnteredIntoBreakStatus( this );
    }

    public void ShowPopUpDisplayInfo( string text, Color textColor )
    {
        GameObject _popUpDisplayInfoObj = Instantiate( this.popUpDisplayInfoPrefab.gameObject );
        _popUpDisplayInfoObj.transform.position = this.pivot.position;

        PopUpDisplayInfo _popUpDisplayInfo = _popUpDisplayInfoObj.GetComponent<PopUpDisplayInfo>();
        _popUpDisplayInfo.Show( text, textColor, 5.0f, TMPro.FontStyles.Bold ).MoveUpAndFadeOut( 2.0f, 0.5f, 1.5f ).SetOnDestroyedCallback( OnPopUpDisplayInfoDestroyed );

        this.popUpDisplayInfoList.Add( _popUpDisplayInfo );
    }

    private void OnPopUpDisplayInfoDestroyed( PopUpDisplayInfo popUpDisplayInfo )
    {
        this.popUpDisplayInfoList.Remove( popUpDisplayInfo );
    }

    public bool HasPopUpDisplayInfo()
    {
        return ( this.popUpDisplayInfoList.Count > 0 );
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

    public float GetVirtualHealthPoint()
    {
        return this.virtualHealthPoint;
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

    public void SetCurrentSkill( CharacterSkill currentSkill )
    {
        this.currentSkill = currentSkill;
        this.isAbleToUseSkill = true;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public CharacterSkill GetCurrentSkill()
    {
        return this.currentSkill;
    }

    public void SetCurrentSkillStatIncrement( int currentSkillStatIncrement )
    {
        this.currentSkillStatIncrement = currentSkillStatIncrement;
    }

    public int GetCurrentSkillStatIncrement()
    {
        return this.currentSkillStatIncrement;
    }

    public void SetCurrentAttacker( GameCharacter currentAttacker )
    {
        this.currentAttacker = currentAttacker;
        this.currentSkillStatIncrement = 0;

        if (this.currentAttacker != null)
        {
            CharacterSkill _currentAttackerSkill = this.currentAttacker.GetCurrentSkill();
            for (int i = 0; i < this.selectedBackendSkillList.Count; i++)
            {
                CharacterSkill _backendSkill = this.selectedBackendSkillList[ i ];
                if (_backendSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
                {
                    ObservedSkillData _observedSkillData = _backendSkill.GetObservedSkillData( _currentAttackerSkill.GetCharacterSubskillData().GetSubskillData().FeatureId );
                    if (_observedSkillData != null)
                    {
                        this.currentSkillStatIncrement = Mathf.FloorToInt( _observedSkillData.GetCurrentObservedRate() );
                    }

                    break;
                }
            }
        }
    }

    public GameCharacter GetCurrentAttacker()
    {
        return this.currentAttacker;
    }

    public void SetIsAbleToUseSkill( bool isAbleToUseSkill )
    {
        this.isAbleToUseSkill = isAbleToUseSkill;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public bool GetIsAbleToUseSkill()
    {
        return this.isAbleToUseSkill;
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

    public void SetEnergyMarkerRemainingATLs( int energyMarkerRemainingATLs )
    {
        this.energyMarkerRemainingATLs = energyMarkerRemainingATLs;
    }

    public void MinusEnergyMarkerRemainingATLs()
    {
        if (this.energyMarkerRemainingATLs > 0)
        {
            this.energyMarkerRemainingATLs--;
        }
    }

    public int GetEnergyMarkerRemainingATLs()
    {
        return this.energyMarkerRemainingATLs;
    }

    public bool HasEnergyMarker()
    {
        return ( this.energyMarkerRemainingATLs > 0 );
    }

    public GameObject GetOwnContainer()
    {
        return this.ownContainer;
    }

    public GameObject GetOpponentContainer()
    {
        return this.opponentContainer;
    }

    public void Reset()
    {
        SetCurrentSkill( null );
        SetCurrentAttacker( null );
    }
}
