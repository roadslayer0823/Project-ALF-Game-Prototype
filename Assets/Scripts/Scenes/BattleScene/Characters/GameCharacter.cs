using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using Character = DatabaseManager.Character;
using AnimationEvent = BattleAnimationManager.AnimationEvent;
using BattleResultData_GameCharacter = BattleResultData.BattleResultData_GameCharacter;

public class GameCharacter : MonoBehaviour
{
    [SerializeField] private SortingGroup sortingGroup = null;
    [SerializeField] private Transform pivot = null;
    [SerializeField] private Animator characterAnimator = null;

    [Header( "Version 1" )]
    [SerializeField] private Animator skillEffectAnimator = null;
    [SerializeField] private PopUpDisplayInfo popUpDisplayInfoPrefab = null;
    [SerializeField] private UnityEventHandler characterEventHandler = null;
    [SerializeField] private UnityEventHandler skillEffectEventHandler = null;

    [Header( "Version 2" )]
    [SerializeField] private PopUpDisplayInfoV2 popUpDisplayInfoPrefabV2 = null;

    private string id = null;
    private string characterName = null;
    private bool isPlayer = false;
    private float maximumHealthPoint = 0.0f;
    private float currentHealthPoint = 0.0f;
    private float virtualHealthPoint = 0.0f;
    private float originalStatePoint = 0.0f;
    private float maximumStatePoint = 0.0f;
    private float minimumStatePoint = 0.0f;
    private float currentStatePoint = 0.0f;
    private float maximumStressValue = 0.0f;
    private float currentStressValue = 0.0f;
    protected CharacterSkill[] skills = null;
    protected List<CharacterSkill> selectedActiveSkillList = null;
    protected List<CharacterSkill> selectedBackendSkillList = null;

    private GameObject ownContainer = null;
    private GameObject opponentContainer = null;
    private GameCharacterInfoBox gameCharacterInfoBox = null;

    private Action onInitialized = null;
    private Action<AnimationEvent,GameCharacter> onEventTriggeredCallback = null;
    private Action<string> onCharacterAnimationTriggeredCallback = null;
    private Action<string> onSkillEffectAnimationTriggeredCallback = null;
    private Action onCharacterInfoUpdated = null;

    private CharacterIdentityType currentCharacterIdentityType = CharacterIdentityType.None;
    private CharacterSkill currentSkill = null;
    private RangeType currentSkillRangeType = RangeType.none;
    private CharacterSkill currentObservingSkill = null;
    private CharacterSkill currentObservedSkill = null;
    private int currentSkillStatIncrement = 0;
    private GameCharacter currentAttacker = null;
    private float skillCountdownTime = 0.0f;
    private int counterAttacks = 0;
    private bool isAbleToUseSkill = false;
    private List<PopUpDisplayInfo> popUpDisplayInfoList = new();

    // Break Status
    private int breakStatusRemainingATLs = 0;
    private bool isBreakStatusCausedByStatePoint = false;
    private bool isBreakStatusCausedByStressValue = false;

    // Energy Marker
    private int energyMarkerRemainingATLs = 0;
    private bool hasJustAddedEnergyMarker = false;

    // Audios
    private const string AUDIO_ID_BREAK = "break";

    // Animation Event
    private BattleAnimationEventManager battleAnimationEventManager = null;

    // Version 2
    private CharacterSkill assignedSkill = null;    // 已按下的技能，該技能等待被發動。
    private bool isInRepulseCommandTime = false;    // 是否在“迎戰指令時間”裡？
    private bool isCounterAttacking = false;        // 是否正在進行反擊？
    private int stateBreakStatusRemainingATLs = 0;  // 以太崩潰維持值 (ATL)
    private int stressBreakStatusRemainingATLs = 0; // 負荷崩潰維持值 (ATL)

    public enum CharacterIdentityType
    {
        None,
        Lead,
        Improviser,
        Assaulter,
        LightAssaulter,
        HeavyAssaulter,
        Recipient,
        LightRecipient,
        HeavyRecipient,
        SuccessfulResister,
        SuccessfulDefender,
        SuccessfulEvader,
        Deuce
    }

    public enum CharacterActionType
    {
        None,
        Repulse,
        Derive,
        Counter,
        Defend,
        Evade
    }

    public void Initialize( Character characterData, bool isPlayer, GameObject ownContainer, GameObject opponentContainer, Action<AnimationEvent,GameCharacter> onEventTriggeredCallback = null )
    {
        base.gameObject.name = "Character: " + characterData.DisplayName;
        this.ownContainer = ownContainer;
        this.opponentContainer = opponentContainer;
        this.onEventTriggeredCallback = onEventTriggeredCallback;

        this.id = characterData.Id;
        this.characterName = characterData.DisplayName;
        this.isPlayer = isPlayer;
        this.maximumHealthPoint = characterData.MaximumHealthPoint;
        this.currentHealthPoint = this.maximumHealthPoint;
        this.virtualHealthPoint = this.maximumHealthPoint;
        this.originalStatePoint = characterData.MaximumStatePoint;
        this.maximumStatePoint = this.originalStatePoint;
        this.minimumStatePoint = GameConfiguration.Instance.GetBattleConfiguration().GetMinimumCurrentStatePoint();
        this.currentStatePoint = this.maximumStatePoint;
        this.maximumStressValue = characterData.MaximumStressValue;
        this.currentStressValue = 0.0f;
        this.selectedActiveSkillList = new List<CharacterSkill>();
        this.selectedBackendSkillList = new List<CharacterSkill>();

        List<CharacterSkill> _skillList = new();
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

            if (_characterSkill.GetCharacterSubskillData().GetSubskillData().IsDefault)
            {
                AddSelectedSkill( _characterSkill );
            }
        }

        this.onCharacterInfoUpdated?.Invoke();

        if (this.battleAnimationEventManager == null)
        {
            this.characterEventHandler.GetCallback().AddListener( OnCharacterAnimationTriggered );
            this.skillEffectEventHandler.GetCallback().AddListener( OnSkillEffectAnimationTriggered );
        }

        this.onInitialized?.Invoke();
    }

    public void AddOnInitializedCallback( Action onInitialized )
    {
        this.onInitialized += onInitialized;
    }

    public void AddOnCharacterInfoUpdatedCallback( Action onCharacterInfoUpdated )
    {
        this.onCharacterInfoUpdated += onCharacterInfoUpdated;
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

    public void MinusCurrentStatePoint( float amount, bool onHit, bool isBreakStatusAvailable )
    {
        if (amount > 0)
        {
            SetCurrentStatePoint( this.currentStatePoint - amount );

            if (isBreakStatusAvailable)
            {
                if (BattleLogicManager.IsGameCharacterInBreakStatus( this, onHit ))
                {
                    this.isBreakStatusCausedByStatePoint = true;

                    if (!GetIsInBreakStatus())
                    {
                        EnterIntoBreakStatus( 1 );
                    }

                    AudioManager.Instance.PlaySoundEffect( AUDIO_ID_BREAK );
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

    public void AddCurrentStressValue( float amount, bool isBreakStatusAvailable )
    {
        Debug.Log("AddCurrentStressValue");
        if (amount > 0)
        {
            if (!this.isBreakStatusCausedByStressValue)
            {
                this.currentStressValue += amount;

                if (isBreakStatusAvailable)
                {
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
                else
                {
                    if (this.currentStressValue >= this.maximumStressValue)
                    {
                        this.currentStressValue = this.maximumStressValue - 1;
                    }
                }
            }

            this.onCharacterInfoUpdated?.Invoke();
        }
    }

    public float MinusCurrentStressValue( float amount )
    {
        Debug.Log("MinusCurrentStressValue");
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

    public void ClearSelectedBackendSkillList()
    {
        this.selectedBackendSkillList.Clear();
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

    public void ShowCharacterObject()
    {
        this.characterAnimator.gameObject.SetActive( true );

        if (this.gameCharacterInfoBox != null)
        {
            this.gameCharacterInfoBox.gameObject.SetActive( true );
        }
    }

    public void HideCharacterObject()
    {
        this.characterAnimator.gameObject.SetActive( false );

        if (this.gameCharacterInfoBox != null)
        {
            this.gameCharacterInfoBox.gameObject.SetActive( false );
        }
    }

    public bool IsCharacterObjectActive()
    {
        return ( this.characterAnimator.gameObject.activeInHierarchy );
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

        if (battleGameManager.GetBattleFlowManager_V2() == null)
        {
            BattleFlowATL _nextATL = battleGameManager.GetBattleFlowManager().GetCurrentRound().GetNextATL( this );
            if (_nextATL != null)
            {
                repulseSkill = _nextATL.GetSelectedSkill().GetCharacterSubskillData().GetSelectedRepulseSkill();
            }
            else
            {
                return false;
            }
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

            if (this.currentAttacker != null)
            {
                Subskill _attackerSubskillData = this.currentAttacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
                if (( int )_attackerSubskillData.EffectType > ( int )_backendSubskillData.EffectType)
                {
                    return false;
                }
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

    public void ShowPopUpDisplayInfoV2( float healthPointDamage = 0.0f, float maxStatePointUp = 0.0f, float statePointDamage = 0.0f,
                                        float stressValueDamage = 0.0f, float stressValueDown = 0.0f )
    {
        PopUpDisplayInfoV2.SpawnPopUpDisplayInfoV2( popUpDisplayInfoPrefabV2, pivot.position, !isPlayer, healthPointDamage, maxStatePointUp, statePointDamage, stressValueDamage, stressValueDown );
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

    public bool GetIsPlayer()
    {
        return this.isPlayer;
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

    public float GetOriginalStatePoint()
    {
        return this.originalStatePoint;
    }

    public float GetMaximumStatePoint()
    {
        return this.maximumStatePoint;
    }

    public float GetMinimumStatePoint()
    {
        return this.minimumStatePoint;
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

    public CharacterSkill GetSkillbySkillId(string idCheck)
    {
        foreach (CharacterSkill skill in skills)
        {
            if (idCheck == skill.GetSkillData().Id)
            {
                return skill;
            }
        }

        return null;
    }

    public SortingGroup GetSortingGroup()
    {
        return this.sortingGroup;
    }

    public Transform GetPivot()
    {
        return this.pivot;
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

        this.currentSkillStatIncrement = 0;
        if (this.currentAttacker != null)
        {
            CharacterSkill _currentAttackerSkill = this.currentAttacker.GetCurrentSkill();
            if (_currentAttackerSkill != null)
            {
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

        this.onCharacterInfoUpdated?.Invoke();
    }

    public CharacterSkill GetCurrentSkill()
    {
        return this.currentSkill;
    }

    public void SetCurrentSkillRangeType( RangeType currentSkillRangeType )
    {
        this.currentSkillRangeType = currentSkillRangeType;
    }

    public RangeType GetCurrentSkillRangeType()
    {
        return this.currentSkillRangeType;
    }

    public void SetCurrentObservingSkill( CharacterSkill observingSkill )
    {
        this.currentObservingSkill = observingSkill;

        if (this.currentObservingSkill != null)
        {
            TriggerEvent( AnimationEvent.OnObservingSkillSelected );
        }
    }

    public CharacterSkill GetCurrentObservingSkill()
    {
        return this.currentObservingSkill;
    }

    public void SetCurrentObservedSkill( CharacterSkill currentObservedSkill )
    {
        this.currentObservedSkill = currentObservedSkill;
    }

    public CharacterSkill GetCurrentObservedSkill()
    {
        return this.currentObservedSkill;
    }

    public void SetCurrentSkillStatIncrement( int currentSkillStatIncrement )
    {
        this.currentSkillStatIncrement = currentSkillStatIncrement;
    }

    public void ResetCurrentSkillStatIncrement()
    {
        this.currentSkillStatIncrement = 0;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public int GetCurrentSkillStatIncrement()
    {
        return this.currentSkillStatIncrement;
    }

    public void SetCurrentAttacker( GameCharacter currentAttacker )
    {
        this.currentAttacker = currentAttacker;
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
        this.hasJustAddedEnergyMarker = true;
        this.onCharacterInfoUpdated?.Invoke();
    }

    public void MinusEnergyMarkerRemainingATLs()
    {
        if (this.energyMarkerRemainingATLs > 0)
        {
            if (this.hasJustAddedEnergyMarker)
            {
                this.hasJustAddedEnergyMarker = false;
            }
            else
            {
                this.energyMarkerRemainingATLs--;
            }
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

    public void RemoveEnergyMarker()
    {
        this.energyMarkerRemainingATLs = 0;
        this.onCharacterInfoUpdated?.Invoke();
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
        SetCurrentCharacterIdentityType( CharacterIdentityType.None );
        SetCurrentSkill( null );
        SetCurrentSkillRangeType( RangeType.none );
        SetCurrentObservingSkill( null );
        SetCurrentObservedSkill( null );
        SetCurrentAttacker( null );
    }

#region Version 2

    public void ApplyBattleResultData( BattleResultData_GameCharacter battleResultData, bool needToUpdateDisplay = true )
    {
        if (battleResultData != null)
        {
            this.maximumHealthPoint = battleResultData.maximumHealthPoint;
            this.currentHealthPoint = battleResultData.currentHealthPoint;
            this.virtualHealthPoint = battleResultData.virtualHealthPoint;
            this.originalStatePoint = battleResultData.originalStatePoint;
            this.maximumStatePoint = battleResultData.maximumStatePoint;
            this.minimumStatePoint = battleResultData.minimumStatePoint;
            this.currentStatePoint = battleResultData.currentStatePoint;
            this.maximumStressValue = battleResultData.maximumStressValue;
            this.currentStressValue = battleResultData.currentStressValue;

            // 崩潰狀態
            this.stateBreakStatusRemainingATLs = battleResultData.stateBreakStatusRemainingATLs;
            this.stressBreakStatusRemainingATLs = battleResultData.stressBreakStatusRemainingATLs;

            // 能量殘響
            this.energyMarkerRemainingATLs = battleResultData.energyMarkerRemainingATLs;

            if (needToUpdateDisplay)
            {
                InvokeOnCharacterInfoUpdatedCallback();
            }
        }
    }

    public void SetCurrentCharacterIdentityType( CharacterIdentityType currentCharacterIdentityType )
    {
        this.currentCharacterIdentityType = currentCharacterIdentityType;
    }

    public CharacterIdentityType GetCurrentCharacterIdentityType()
    {
        return this.currentCharacterIdentityType;
    }

    public void SetAssignedSkill( CharacterSkill assignedSkill )
    {
        this.assignedSkill = assignedSkill;

        if (this.isInRepulseCommandTime)
        {
            this.assignedSkill?.SetHasSkillUpdateIndicator( true );
        }
    }

    public CharacterSkill ResetAssignedSkill()
    {
        this.assignedSkill = null;
        return this.assignedSkill;
    }

    public CharacterSkill GetAssignedSkill()
    {
        return this.assignedSkill;
    }

    public void ApplyAssignedSkillAsCurrentSkill()
    {
        if (GetIsInBreakStatus())
        {
            return;
        }

        if (this.assignedSkill != null)
        {
            SetCurrentSkill( this.assignedSkill );
            this.assignedSkill = null;
        }
    }

    public void SetIsInRepulseCommandTime( bool isInRepulseCommandTime )
    {
        this.isInRepulseCommandTime = isInRepulseCommandTime;
    }

    public bool GetIsInRepulseCommandTime()
    {
        return this.isInRepulseCommandTime;
    }

    public void SetIsCounterAttacking( bool isCounterAttacking )
    {
        this.isCounterAttacking = isCounterAttacking;
    }

    public bool GetIsCounterAttacking()
    {
        return this.isCounterAttacking;
    }

    public int GetStateBreakStatusRemainingATLs()
    {
        return this.stateBreakStatusRemainingATLs;
    }

    public bool IsInStateBreakStatus()
    {
        return ( this.stateBreakStatusRemainingATLs > 0 );
    }

    public int GetStressBreakStatusRemainingATLs()
    {
        return this.stressBreakStatusRemainingATLs;
    }

    public bool IsInStressBreakStatus()
    {
        return ( this.stressBreakStatusRemainingATLs > 0 );
    }

    public void InvokeOnCharacterInfoUpdatedCallback()
    {
        this.onCharacterInfoUpdated?.Invoke();
    }

#endregion

#region Battle Animation Event Manager

    public void SetUp(BattleAnimationEventManager battleAnimationEventManager)
    {
        this.battleAnimationEventManager = battleAnimationEventManager;
    }

    public void OnAnimationEventTriggered(BattleAnimationEventManager.CharacterAnimationEventType animationEventType, string parameter = "")
    {
        this.battleAnimationEventManager.OnAnimationEventTriggered(animationEventType, parameter);
    }

#endregion
}
