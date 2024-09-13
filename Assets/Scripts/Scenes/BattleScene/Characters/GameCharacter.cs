using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Skill = DatabaseManager.Skill;
using RangeType = DatabaseManager.Subskill.RangeType;
using Character = DatabaseManager.Character;
using AnimationEvent = BattleAnimationManager.AnimationEvent;
using BattleResultData_GameCharacter = BattleResultData.BattleResultData_GameCharacter;

public partial class GameCharacter : MonoBehaviour
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
    [SerializeField] private CharacterAnimationHandler characterAnimationHandler = null;

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

    private CharacterSkill currentSkill = null;
    private CharacterSkill currentObservingSkill = null;
    private GameCharacter currentAttacker = null;
    private GameCharacter currentAttackTarget = null;
    private float skillCountdownTime = 0.0f;

    // Energy Marker
    private int energyMarkerRemainingATLs = 0;

    // Audios
    private const string AUDIO_ID_BREAK = "break";

    // Animation Event
    private BattleAnimationEventManager battleAnimationEventManager = null;

    // Version 2
    private CharacterSkill assignedSkill = null;     // 已按下的技能，該技能等待被發動。
    private bool isInRepulseCommandTime = false;     // 是否在“迎戰指令時間”裡？
    private bool isMeleeAttacking = false;           // 是否正在進行近戰？
    private bool isCounterAttacking = false;         // 是否正在進行反擊？
    private int stateBreakStatusRemainingATLs = 0;   // 以太崩潰維持值 (ATL)
    private int stressBreakStatusRemainingATLs = 0;  // 負荷崩潰維持值 (ATL)
    private int numberOfEnteringIntoBreakStatus = 0; // 陷入崩潰狀態的次數
    private bool isDead = false;                     // 是否死亡？
    private bool hasJustDied = false;                // 是否剛剛才死亡？
    private BattleResultData_GameCharacter temporaryBattleResultData = null;
    private List<CharacterIdentityType> characterIdentityTypeList = null; // 暫時性身份列表
    private List<CharacterIdentityType> permanentCharacterIdentityTypeList = null; // 永久性身份列表
    private CommandTimeType currentCommandTimeType = CommandTimeType.None; // 目前的指令時間
    private float statePointBeforeBreakStatus = 0.0f; // 崩潰前一刻的以太值數值
    private List<CharacterSkill> allSkills = null;
    private CharacterSkill lastAtlSkill = null;

    public enum CharacterIdentityType
    {
        // 無身份
        None,

        // 玩家 1
        PlayerOne,

        // 玩家 2
        PlayerTwo,

        // 先手方
        Lead,

        // 後手方
        Improviser,

        // 平手方
        Deuce,

        // 抵抗成功方
        SuccessfulResister,

        // 直擊方
        Assaulter,

        // 輕直擊方
        LightAssaulter,

        // 重直擊方
        HeavyAssaulter,

        // 受擊方
        Recipient,

        // 輕受擊方
        LightRecipient,

        // 重受擊方
        HeavyRecipient,

        // 未能抵抗方
        NonResister,

        // 速度勝方
        SpeedWinner,

        // 強度勝方
        StrengthWinner,

        // 速度負方
        SpeedLoser,

        // 強度負方
        StrengthLoser,

        // 速度強度負方
        SpeedStrengthLoser,

        // 以太崩潰方
        StateBreakStatusHolder,

        // 負荷崩潰方
        StressBreakStatusHolder,

        // 勝利優惠機制方
        WinningBenefitHolder,

        // 近距離遠程方
        NearDistanceRangedDealer,

        // 中距離近戰方
        NormalDistanceMeleeDealer,

        // 已更新按下技能方
        UpdatedSelectedSkill,

        // 無視追風角力方
        IgnoreZhuiFengJiaoLi,

        // 無視追風角力激昂方
        IgnoreZhuiFengJiaoLiJiAng,

        // 無視遠程方
        IgnoreRangedSkill
    }

    public enum CommandTimeType
    {
        None,

        // 反擊指令時間
        CounterAttack,

        // 臨戰指令時間後
        CombatAfter,

        // 近戰反擊指令時間
        MeleeCounterAttack,

        // 近戰指令時間
        MeleeCombat
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
        this.characterIdentityTypeList = new List<CharacterIdentityType>();
        this.permanentCharacterIdentityTypeList = new List<CharacterIdentityType>();
        this.allSkills = new List<CharacterSkill>();

        this.characterAnimationHandler.Initialize(this);

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

            this.allSkills.Add( _characterSkill );
        }

        InitializePassiveSkillLists();

        this.onCharacterInfoUpdated?.Invoke();

        if (this.battleAnimationEventManager == null)
        {
            if (this.characterEventHandler != null)
            {
                this.characterEventHandler.GetCallback().AddListener( OnCharacterAnimationTriggered );
            }

            if (this.skillEffectEventHandler != null)
            {
                this.skillEffectEventHandler.GetCallback().AddListener( OnSkillEffectAnimationTriggered );
            }
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

        if (this.characterAnimator != null)
        {
            this.characterAnimator.Play( animationName, 0, 0.0f );
        }
    }

    public void PlaySkillEffectAnimation( string animationName, Action<string> onAnimationTriggeredCallback = null )
    {
        this.onSkillEffectAnimationTriggeredCallback = onAnimationTriggeredCallback;

        if (this.skillEffectAnimator != null)
        {
            this.skillEffectAnimator.Play( animationName, 0, 0.0f );
        }
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
        Debug.Log( "Show Character Object" );

        this.characterAnimator.gameObject.SetActive( true );
        //this.characterAnimator.Play( "Idle" );

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

    public void ShowPopUpDisplayInfoV2( float healthPointDamage = 0.0f, float maxStatePointUp = 0.0f, float statePointDamage = 0.0f,
                                        float stressValueDamage = 0.0f, float stressValueDown = 0.0f )
    {
        PopUpDisplayInfoV2.SpawnPopUpDisplayInfoV2( popUpDisplayInfoPrefabV2, pivot.position, !isPlayer, healthPointDamage, maxStatePointUp, statePointDamage, stressValueDamage, stressValueDown );
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

    public CharacterSkill GetSkillBySubskillId( string subskillId, out string errorMessage )
    {
        errorMessage = "";

        string[] _splittedStrings = subskillId.Split( '_' );
        if (_splittedStrings.Length == 2)
        {
            string _skillId = _splittedStrings[ 0 ];
            bool _hasSkill = false;

            for (int i = 0; i < this.skills.Length; i++)
            {
                CharacterSkill _skill = skills[ i ];
                if (_skill.GetSkillData().Id.Equals( _skillId, StringComparison.OrdinalIgnoreCase ))
                {
                    _hasSkill = true;

                    if (int.TryParse( _splittedStrings[ 1 ], out int _skillLevel ))
                    {
                        if (_skill.SetSelectedSkillLevel( _skillLevel ))
                        {
                            return _skill;
                        }
                        else
                        {
                            errorMessage = "no such skill level";
                        }
                    }
                    else
                    {
                        errorMessage = "invalid subskill id";
                    }

                    break;
                }
            }

            if (!_hasSkill)
            {
                errorMessage = "no such skill id";
            }
        }
        else
        {
            errorMessage = "invalid subskill id";
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

        if (this.currentSkill != null)
        {
            this.currentSkill.SetCurrentRangeType( this.currentSkill.GetCharacterSubskillData().GetSubskillData().Range );
        }

        //this.isAbleToUseSkill = true;

        //this.currentSkillStatIncrement = 0;
        //if (this.currentAttacker != null)
        //{
        //    CharacterSkill _currentAttackerSkill = this.currentAttacker.GetCurrentSkill();
        //    if (_currentAttackerSkill != null)
        //    {
        //        for (int i = 0; i < this.selectedBackendSkillList.Count; i++)
        //        {
        //            CharacterSkill _backendSkill = this.selectedBackendSkillList[ i ];
        //            if (_backendSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
        //            {
        //                ObservedSkillData _observedSkillData = _backendSkill.GetObservedSkillData( _currentAttackerSkill.GetCharacterSubskillData().GetSubskillData().FeatureId );
        //                if (_observedSkillData != null)
        //                {
        //                    this.currentSkillStatIncrement = Mathf.FloorToInt( _observedSkillData.GetCurrentObservedRate() );
        //                }

        //                break;
        //            }
        //        }
        //    }
        //}

        this.onCharacterInfoUpdated?.Invoke();
    }

    public CharacterSkill GetCurrentSkill()
    {
        return this.currentSkill;
    }

    public RangeType GetCurrentSkillRangeType()
    {
        if (this.currentSkill != null)
        {
            return this.currentSkill.GetCurrentRangeType();
        }

        return RangeType.none;
    }

    public void SetCurrentObservingSkill( CharacterSkill observingSkill, bool isSelectingSkill )
    {
        this.currentObservingSkill = observingSkill;

        if (this.currentObservingSkill != null && isSelectingSkill)
        {
            TriggerEvent( AnimationEvent.OnObservingSkillSelected );
        }
    }

    public void ResetCurrentObservingSkill()
    {
        this.currentObservingSkill = null;
    }

    public CharacterSkill GetCurrentObservingSkill()
    {
        return this.currentObservingSkill;
    }

    public void SetCurrentAttacker( GameCharacter currentAttacker )
    {
        if (this.currentAttacker != null)
        {
            this.currentAttacker.SetCurrentAttackTarget( null );
        }

        this.currentAttacker = currentAttacker;

        if (this.currentAttacker != null)
        {
            this.currentAttacker.SetCurrentAttackTarget( this );
        }
    }

    public GameCharacter GetCurrentAttacker()
    {
        return this.currentAttacker;
    }

    public void SetCurrentAttackTarget( GameCharacter currentAttackTarget )
    {
        this.currentAttackTarget = currentAttackTarget;
    }

    public GameCharacter GetCurrentAttackTarget()
    {
        return this.currentAttackTarget;
    }

    public void SetSkillCountdownTime( float skillCountdownTime )
    {
        this.skillCountdownTime = skillCountdownTime;
    }

    public float GetSkillCountdownTime()
    {
        return this.skillCountdownTime;
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
        this.currentSkill?.SetCurrentRangeType( RangeType.none );

        SetCurrentAttacker( null );
        SetCurrentSkill( null );
        ResetCurrentObservingSkill();
        SetCurrentObservedSkill( null ); // Obsolete
    }

#region Version 2

    public void RecordAllSkillSelectedLevelsAsPresets()
    {
        for (int i = 0; i < this.allSkills.Count; i++)
        {
            this.allSkills[ i ].RecordSelectedSkillLevelAsPreset();
        }
    }

    public void ResetAllSkillSelectedLevelsToPresets()
    {
        for (int i = 0; i < this.allSkills.Count; i++)
        {
            this.allSkills[ i ].ResetSelectedSkillLevelToPreset();
        }
    }

    public void ApplyBattleResultData( BattleResultData_GameCharacter battleResultData, BattleGameManager battleGameManager = null, bool useMaximumStatePointWithoutBonus = false )
    {
        if (battleResultData != null)
        {
            bool _isDead = this.isDead;

            this.maximumHealthPoint = battleResultData.maximumHealthPoint;
            this.currentHealthPoint = battleResultData.currentHealthPoint;
            this.virtualHealthPoint = battleResultData.virtualHealthPoint;
            this.originalStatePoint = battleResultData.originalStatePoint;
            this.maximumStatePoint = ( useMaximumStatePointWithoutBonus ) ? battleResultData.maximumStatePointWithoutBonus : battleResultData.maximumStatePoint;
            this.minimumStatePoint = battleResultData.minimumStatePoint;
            this.currentStatePoint = battleResultData.currentStatePoint;
            this.maximumStressValue = battleResultData.maximumStressValue;
            this.currentStressValue = battleResultData.currentStressValue;

            // 崩潰狀態
            this.stateBreakStatusRemainingATLs = battleResultData.stateBreakStatusRemainingATLs;
            this.stressBreakStatusRemainingATLs = battleResultData.stressBreakStatusRemainingATLs;
            this.numberOfEnteringIntoBreakStatus = battleResultData.numberOfEnteringIntoBreakStatus;

            // 能量殘響
            this.energyMarkerRemainingATLs = battleResultData.energyMarkerRemainingATLs;

            // 其他狀態
            this.isDead = battleResultData.isDead;
            this.hasJustDied = ( !_isDead && this.isDead );

            ApplyBattleResultData_CategorizedPassiveSkillManager( battleResultData );

            if (battleGameManager != null)
            {
                ShowPassiveSkillTags( battleResultData.triggeredPassiveSkillList, battleGameManager );
            }

            InvokeOnCharacterInfoUpdatedCallback();
        }

        this.temporaryBattleResultData = null;
    }

    private void UpdateDebugLog( string eventName )
    {
#if ALF_DEBUG

        if (this.temporaryBattleResultData != null)
        {
            this.temporaryBattleResultData.GetBattleResultData().UpdateDebugLog( this.temporaryBattleResultData, eventName );
        }

#endif
    }

    public void AddCharacterIdentityType( CharacterIdentityType characterIdentityType )
    {
        this.characterIdentityTypeList.Add( characterIdentityType );
        UpdateDebugLog( "得到新身份" );
    }

    public void AddCharacterIdentityTypes( CharacterIdentityType[] characterIdentityTypes )
    {
        this.characterIdentityTypeList.AddRange( characterIdentityTypes );
        UpdateDebugLog( "得到新身份" );
    }

    public void AddCharacterIdentityTypes( List<CharacterIdentityType> characterIdentityTypes )
    {
        this.characterIdentityTypeList.AddRange( characterIdentityTypes );
        UpdateDebugLog( "得到新身份" );
    }

    public void AddPermanentCharacterIdentityType( CharacterIdentityType characterIdentityType )
    {
        this.permanentCharacterIdentityTypeList.Add( characterIdentityType );
        UpdateDebugLog( "得到新身份" );
    }

    public void RemoveCharacterIdentityType( CharacterIdentityType characterIdentityType )
    {
        this.characterIdentityTypeList.Remove( characterIdentityType );
        UpdateDebugLog( "取消舊身份" );
    }

    public bool HasCharacterIdentityType( CharacterIdentityType characterIdentityType )
    {
        return this.GetAllCharacterIdentityTypes().Contains( characterIdentityType );
    }

    public bool HasOneOfCharacterIdentityTypes( CharacterIdentityType[] characterIdentityTypes )
    {
        for (int i = 0; i < characterIdentityTypes.Length; i++)
        {
            if (this.GetAllCharacterIdentityTypes().Contains( characterIdentityTypes[ i ] ))
            {
                return true;
            }
        }

        return false;
    }

    public void ClearCharacterIdentityTypeList()
    {
        this.characterIdentityTypeList.Clear();
    }

    public bool IsCharacterIdentityTypeListEmpty()
    {
        return ( this.characterIdentityTypeList.Count <= 0 );
    }

    public List<CharacterIdentityType> GetPermanentCharacterIdentityTypeList()
    {
        return this.permanentCharacterIdentityTypeList;
    }

    public List<CharacterIdentityType> GetAllCharacterIdentityTypes()
    {
        return this.permanentCharacterIdentityTypeList.Concat( this.characterIdentityTypeList ).ToList();
    }

    public void SetCurrentCommandTimeType( CommandTimeType currentCommandTimeType )
    {
        this.currentCommandTimeType = currentCommandTimeType;
    }

    public CommandTimeType GetCurrentCommandTimeType()
    {
        return this.currentCommandTimeType;
    }

    public void SetAssignedSkill( CharacterSkill assignedSkill )
    {
        if (this.isInRepulseCommandTime)
        {
            if(this.assignedSkill == null)
            {
                this.assignedSkill = assignedSkill;
            }
            this.assignedSkill?.SetHasSkillUpdateIndicator(true);
        }
        else
        {
            this.assignedSkill = assignedSkill;
        }

        //Debug.Log("Character: " + characterName);
        //Debug.Log("SetAssignedSkill: " + this.assignedSkill.GetSkillData().Id);
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
            SetCurrentSkill( this.assignedSkill.GetClone() );
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

    public void SetIsMeleeAttacking( bool isMeleeAttacking )
    {
        this.isMeleeAttacking = isMeleeAttacking;
    }

    public bool GetIsMeleeAttacking()
    {
        return this.isMeleeAttacking;
    }

    public void SetIsCounterAttacking( bool isCounterAttacking )
    {
        this.isCounterAttacking = isCounterAttacking;
    }

    public bool GetIsCounterAttacking()
    {
        return this.isCounterAttacking;
    }

    public bool IsInBreakStatus()
    {
        return ( IsInStateBreakStatus() || IsInStressBreakStatus() );
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

    public int GetNumberOfEnteringIntoBreakStatus()
    {
        return this.numberOfEnteringIntoBreakStatus;
    }

    public bool GetIsDead()
    {
        return this.isDead;
    }

    public bool GetHasJustDied()
    {
        return this.hasJustDied;
    }

    public void SetStatePointBeforeBreakStatus( float statePointBeforeBreakStatus )
    {
        this.statePointBeforeBreakStatus = statePointBeforeBreakStatus;
    }

    public float GetStatePointBeforeBreakStatus()
    {
        return this.statePointBeforeBreakStatus;
    }

    public void SetTemporaryBattleResultData( BattleResultData_GameCharacter temporaryBattleResultData )
    {
        this.temporaryBattleResultData = temporaryBattleResultData;
    }

    public BattleResultData_GameCharacter GetTemporaryBattleResultData()
    {
        return this.temporaryBattleResultData;
    }

    public void InvokeOnCharacterInfoUpdatedCallback()
    {
        this.onCharacterInfoUpdated?.Invoke();
    }

    public void AddToAllSkills( CharacterSkill characterSkill )
    {
        this.allSkills.Add( characterSkill );
    }

    public void SetLastAtlSkill( CharacterSkill lastAtlSkill )
    {
        this.lastAtlSkill = lastAtlSkill;
    }

    public CharacterSkill GetLastAtlSkill()
    {
        return this.lastAtlSkill;
    }

    public CharacterAnimationHandler GetCharacterAnimationHandler()
    {
        return this.characterAnimationHandler;
    }

    public void PlayIdleAnimation()
    {
        if (this.characterAnimationHandler == null)
        {
            PlayCharacterAnimation( "Idle" );
        }
        else
        {
            this.characterAnimationHandler.ResetAnimation();
            this.characterAnimationHandler.FlipContainer( !this.isPlayer );
            this.characterAnimationHandler.GetPlayerAnimator().Play( "Idle" );
        }
    }

    public void PlayPrepareAnimation()
    {
        if (this.characterAnimationHandler == null)
        {
            PlayCharacterAnimation( "Prepare" );
        }
        else
        {
            this.characterAnimationHandler.ResetAnimation();
            this.characterAnimationHandler.GetPlayerAnimator().Play( "Prepare" );
        }
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
