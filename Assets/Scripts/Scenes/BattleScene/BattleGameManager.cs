using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;

public class BattleGameManager : MonoBehaviour
{
    [Header( "Managers" )]
    [SerializeField] private BattleUiManager battleUiManager = null;
    [System.Obsolete][SerializeField] private BattleFlowManager battleFlowManager = null;
    [SerializeField] private BattleFlowManager_V2 battleFlowManager_V2 = null;
    [SerializeField] private BattleAnimationManager battleAnimationManager = null;
    [SerializeField] private BattleAnimationEventManager battleAnimationEventManager = null;
    [SerializeField] private BattleVisualEffectManager battleVisualEffectManager = null;

    [Header( "References" )]
    [SerializeField] private GameObject playerContainer = null;
    [SerializeField] private GameObject opponentContainer = null;
    [SerializeField] private PlayerCharacter playerCharacter = null;
    [SerializeField] private EnemyCharacter enemyCharacter = null;

    [Header( "Audio" )]
    [SerializeField] private AudioDatabase audioDatabase = null;
    [SerializeField] private AudioClip backgroundMusicAudioClip = null;

    private List<PlayerCharacter> playerCharacterList = null;
    private List<EnemyCharacter> enemyCharacterList = null;
    private GamePhase currentGamePhase = GamePhase.None;

    private const string AUDIO_ID_VICTORY = "victory";
    private const string AUDIO_ID_DEFEAT = "defeat";
    private const string AUDIO_ID_BREAK = "break";
    private const string AUDIO_ID_CROSSING_ACTION_MODE = "crossing_action_mode";
    private const string AUDIO_ID_COMMAND_PHASE = "command_phase";
    private const string AUDIO_ID_LINE_BREAK = "line_break";

    public enum GamePhase
    {
        None,
        Preparation,
        Execution,
        Transition,
        End
    }

    void Awake()
    {
        AudioManager.Instance.SetUpAudioDatabase( this.audioDatabase );

        this.battleUiManager.SetAllActive( false );
        //this.battleUiManager.HideEnemyCharacterInfoBox();
        this.battleAnimationManager.ChangeToBackgroundPartB();
        this.playerCharacter.PlayCharacterAnimation( "Prepare" );
        this.enemyCharacter.PlayCharacterAnimation( "Idle" );
    }

    void Start()
    {
        this.battleUiManager.Initialize( this );

        if (this.battleFlowManager_V2 == null)
        {
            this.battleFlowManager.Initialize( this );
        }
        else
        {
            this.battleFlowManager_V2.Initialize( this );
        }

        this.battleAnimationManager.Initialize( this, OnBattleEnded );
        this.battleVisualEffectManager.SetUp();

        // -------------------- Set up the player's characters --------------------

        this.playerCharacterList = new List<PlayerCharacter>();
        this.playerCharacter.SetUp( this.battleAnimationEventManager );
        this.playerCharacter.Initialize( DatabaseManager.Instance.GetCharacterDataById( "C1" ), true, this.playerContainer, this.opponentContainer, OnCharacterEventTriggered );
        this.playerCharacterList.Add( this.playerCharacter );

        // ------------------------------------------------------------------------

        // -------------------- Set up the enemy's characters --------------------

        this.enemyCharacterList = new List<EnemyCharacter>();
        this.enemyCharacter.SetUp( this.battleAnimationEventManager );
        this.enemyCharacter.Initialize( DatabaseManager.Instance.GetCharacterDataById( "E1" ), false, this.opponentContainer, this.playerContainer, OnCharacterEventTriggered );
        this.enemyCharacterList.Add( this.enemyCharacter );

        // -----------------------------------------------------------------------

        AudioManager.Instance.PlayBackgroundMusic( clip: this.backgroundMusicAudioClip, loopStartTime: 35.142f );

        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CROSSING_ACTION_MODE, () =>
        {
            this.battleUiManager.SetAllActive( true );
            this.battleUiManager.HideSkillSelectionPanel();
            this.battleUiManager.HideSkillSlotListPanel();
            this.battleUiManager.HideATLSlotListPanel();

            if (this.battleUiManager.GetPlayerDashboard() == null)
            {
                this.battleUiManager.GetCharacterInfoPanel().SetSelectedCharacter( this.playerCharacter );
            }
            else
            {
                //this.battleUiManager.GetCharacterInfoPanelV2().SetSelectedCharacter( this.playerCharacter );
            }

            if (this.battleFlowManager_V2 == null)
            {
                this.battleFlowManager.StartGame();
            }
            else
            {
                this.battleFlowManager_V2.StartGame();
            }

            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_COMMAND_PHASE );
        } );

        this.battleUiManager.PlayInstructionAnimation();
    }

    public void StartExecution()
    {
        if (this.battleFlowManager_V2 == null)
        {
            this.battleFlowManager.GetCurrentRound().SetCurrentPhase( BattleFlowRound.PhaseType.Execution );
        }
        else
        {
            this.battleFlowManager_V2.GetCurrentRound().SetCurrentPhase( BattleFlowRound_V2.PhaseType.Execution );
        }

        if (this.battleUiManager.GetPlayerDashboard() == null)
        {
            this.battleUiManager.GetCharacterInfoPanel().HideRoundInfoText();
        }
    }

    public void ShowPreparationView()
    {
        this.battleAnimationManager.ChangeToBackgroundPartB();
        this.playerContainer.SetActive( true );
        this.opponentContainer.SetActive( true );
        this.playerCharacter.PlayCharacterAnimation( "Prepare" );
        this.enemyCharacter.PlayCharacterAnimation( "Idle" );
    }

    public void OnPreparationPhaseStarted()
    {
        this.currentGamePhase = GamePhase.Preparation;

        this.battleUiManager.SetSelectedGameCharacter( this.playerCharacter );
        this.battleUiManager.HideATLSlotListPanel();
        this.battleUiManager.GetATLSlotListPanelV3().Reset();
        this.battleUiManager.CheckWhetherToEnableExecuteButton();

        if (this.battleUiManager.GetPreparationSection() == null)
        {
            this.battleUiManager.ShowSkillSelectionPanel();
            this.battleUiManager.ShowPreparationSection();
        }
        else
        {
            PreparationSection _preparationSection = this.battleUiManager.GetPreparationSection();

            if (BattleLogicManagerV2.ShouldPreparationSectionBeSkipped( this.playerCharacter ))
            {
                _preparationSection.DisableSkillSelectionButtons();
            }
            else
            {
                _preparationSection.EnableSkillSelectionButtons();
            }

            _preparationSection.Show();
        }

        ShowPreparationView();
        this.enemyCharacter.InitializeSelectedSkills();

        /*
        if (BattleLogicManagerV2.ShouldPreparationSectionBeSkipped( this.playerCharacter ))
        {
            this.battleUiManager.GetPlayerDashboard().ProcessExecuteButton();

            BattleLog.Instance.AddOnScreenBattleLog( $"因為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ this.playerCharacter.GetCharacterName() }</color>正在處於<color={ BattleLog.KEYWORD_COLOR_CODE }>崩潰狀態</color>，"
                                                     + $"所以跳過<color={ BattleLog.SPECIAL_COLOR_CODE }>【 準備階段 】</color>，直接進入<color={ BattleLog.SPECIAL_COLOR_CODE }>【 戰鬥階段 】</color>。" );
        }
        */

        //StartCoroutine( RunStartingPreparationPhase() );
    }

    //private IEnumerator RunStartingPreparationPhase()
    //{
    //    yield return null;
    //    yield return null;
    //    this.battleUiManager.ShowEnemyCharacterInfoBoxHUD();
    //}

    public void OnExecutionPhaseStarted()
    {
        this.currentGamePhase = GamePhase.Execution;

        this.playerCharacter.RecordAllSkillSelectedLevelsAsPresets();

        if (this.battleUiManager.GetPreparationSection() == null)
        {
            this.battleUiManager.ShowBattleSection( this.playerCharacter );
        }
        else
        {
            this.battleUiManager.ShowBattleSection();
        }

        this.battleUiManager.HideSkillSelectionPanel();
        this.battleUiManager.ShowSkillSlotListPanel();
        //this.battleUiManager.ShowEnemyCharacterInfoBoxUI();

        if (this.battleFlowManager_V2 == null)
        {
            this.playerCharacter.PlayCharacterAnimation( "Idle" );
            this.enemyCharacter.PlayCharacterAnimation( "Idle" );
            this.battleUiManager.ShowATLSlotListPanel( this.battleFlowManager.GetCurrentRound().GetFlowATLs() );
        }
        else
        {
            this.battleUiManager.ShowATLSlotListPanel( this.battleFlowManager_V2.GetCurrentRound().GetFlowATLs() );
        }

        if (!this.battleUiManager.IsUsingSkillSlotListPanelV2())
        {
            this.battleUiManager.GetSkillSlotListPanel().SetIsSkillSlotListScrollable( true );
        }

        if (this.battleFlowManager_V2 == null)
        {
            this.battleFlowManager.GetCurrentRound().StartRunningATL();
        }
        else
        {
            this.battleFlowManager_V2.GetCurrentRound().StartRunningATL();
        }
    }

    public void OnExecutionPhaseFinished()
    {
        this.currentGamePhase = GamePhase.Transition;

        int _roundNumber = 0;
        if (this.battleFlowManager_V2 == null)
        {
            _roundNumber = this.battleFlowManager.GetCurrentRound().GetRoundNumber();
        }
        else
        {
            _roundNumber = this.battleFlowManager_V2.GetCurrentRound().GetRoundNumber();
        }

        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>【 第 { _roundNumber } 回合結束 】</color>" );

        this.battleVisualEffectManager.TriggerCombatCommandCutOut( false );

        if (this.battleFlowManager_V2 == null)
        {
            BattleLogicManager.OnExecutionPhaseFinished( GetCharacterList() );
        }
        else
        {
            List<GameCharacter> _gameCharacters = GetCharacterList();
            BattleResultData _battleResultData = BattleLogicManagerV2.OnTheEndOfRound( _gameCharacters.ToArray(), out List<string> _resultLogList );

            for (int i = 0; i < _gameCharacters.Count; i++)
            {
                GameCharacter _gameCharacter = _gameCharacters[ i ];
                _gameCharacter.ResetAssignedSkill();
                _gameCharacter.Reset();
                _gameCharacter.SetIsCounterAttacking( false );

                BattleResultData.BattleResultData_GameCharacter _gameCharacterResultData = _battleResultData.GetGameCharacterResultData( _gameCharacter, out bool _isNewElement );
                if (!_isNewElement)
                {
                    _gameCharacter.ApplyBattleResultData( _gameCharacterResultData, true );
                }
            }

            for (int i = 0; i < _resultLogList.Count; i++)
            {
                BattleLog.Instance.AddOnScreenBattleLog( _resultLogList[ i ] );
            }
        }

        this.battleUiManager.PlayHideSkillSlotListPanelAnimation();
        this.battleUiManager.HideATLSlotListPanel();
        this.battleUiManager.GetSkillSlotListPanel().SetIsSkillSlotListScrollable( false );

        if (this.battleUiManager.IsUsingSkillSlotListPanelV2())
        {
            this.battleUiManager.GetActiveSkillSlotListPanelV2().ResetLastRoundSelectedActiveSkill();
        }
        else
        {
            this.battleUiManager.GetSkillSlotListPanel().ResetLastRoundSelectedActiveSkill();
        }

        this.battleUiManager.CheckWhetherToEnableExecuteButton();

        if (this.battleFlowManager_V2 == null)
        {
            this.battleFlowManager.StartNewRound();
        }
        else
        {
            this.battleFlowManager_V2.StartNewRound();
        }
    }

    public void OnNewRoundStarted()
    {
        int _roundNumber = 0;
        if (this.battleFlowManager_V2 == null)
        {
            _roundNumber = this.battleFlowManager.GetCurrentRound().GetRoundNumber();

            if (_roundNumber > 1)
            {
                BattleLogicManager.OnNewRoundStarted( GetCharacterList() );
            }
        }
        else
        {
            _roundNumber = this.battleFlowManager_V2.GetCurrentRound().GetRoundNumber();
        }
    }

    public void OnNewRoundStarted( bool isPlayerFirst )
    {
        if (this.battleUiManager.GetPlayerDashboard() == null)
        {
            this.battleUiManager.GetCharacterInfoPanel().ShowRoundInfoText( isPlayerFirst );
        }

        OnNewRoundStarted();
    }

    public void OnNewATLStarted()
    {
        BattleLogicManager.OnNewATLStarted( GetCharacterList() );

        this.battleUiManager.DisablePlayerActionPanelButtons();
    }

    private void OnCharacterEventTriggered( BattleAnimationManager.AnimationEvent animationEvent, GameCharacter gameCharacter )
    {
        //if (BattleLogicManager.IsGameCharacterDead( gameCharacter ))
        //{
        //    return;
        //}

        gameCharacter.OnEventTriggered( this, animationEvent );

        CharacterSkill _currentObservingSkill = gameCharacter.GetCurrentObservingSkill();

        switch ( animationEvent )
        {
            case BattleAnimationManager.AnimationEvent.OnBeingInBreakStatus:

                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_BREAK );

                if (this.battleFlowManager_V2 == null)
                {
                    this.battleFlowManager.GetCurrentRound().UpdateATLSlotStatuses( gameCharacter, false );
                }

                break;

            case BattleAnimationManager.AnimationEvent.OnObservingSkillSelected:

                if (this.battleFlowManager_V2 == null)
                {
                    GameCharacter _currentCaster = battleAnimationManager.GetCurrentCaster();
                    _currentObservingSkill.SetObservedSkill( _currentCaster, _currentCaster.GetCurrentSkill() );
                }
                else
                {
                    BattleLogicManagerV2.ExecuteObservingSkill( gameCharacter, gameCharacter.GetCurrentAttacker(), out string _resultLog );
                    BattleLog.Instance.AddOnScreenBattleLog( _resultLog );

                    this.battleUiManager.GetSkillPromptPanel().ShowPassiveSkillEffectTag( _currentObservingSkill.GetCharacterSubskillData().GetSubskillData().DisplayName, gameCharacter.GetIsPlayer() );

                    if (gameCharacter.GetIsPlayer())
                    {
                        this.battleUiManager.GetBackendSkillSlotListPanel().GetSkillSlot( _currentObservingSkill ).UpdateCurrentObservedStatus( true );
                    }
                }

                break;

            case BattleAnimationManager.AnimationEvent.OnSkillBeingObserved:

                CharacterSkill _gameCharacterCurrentSkill = gameCharacter.GetCurrentObservingSkill();
                Subskill _gameCharacterCurrentSubskillData = _gameCharacterCurrentSkill.GetCharacterSubskillData().GetSubskillData();

                GameCharacter _observedSkillCaster = _currentObservingSkill.GetObservedSkillCaster();
                CharacterSkill _observedSkill = _currentObservingSkill.GetObservedSkill();
                Subskill _observedSubskillData = _observedSkill.GetCharacterSubskillData().GetSubskillData();

                float _currentObservedRate = _gameCharacterCurrentSkill.AddObservedSkillData( _observedSkill, _gameCharacterCurrentSubskillData.ObservationRate );

                BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + gameCharacter.GetCharacterName() + "</color>使用" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _gameCharacterCurrentSubskillData.DisplayName + "</color>（看破技能）來看破"
                                                         + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _observedSkillCaster.GetCharacterName() + "</color>使用的" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _observedSubskillData.DisplayName + "</color>和對"
                                                         + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _observedSubskillData.DisplayName + "</color>的看破值現為" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _currentObservedRate.ConvertToIntegerInPercentage() + "%</color>。" );

                break;
        }
    }

    private void OnBattleEnded( bool isVictory )
    {
        this.currentGamePhase = GamePhase.End;

        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>【 戰鬥結束 】</color>" );

        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_LINE_BREAK, () =>
        {
            if (isVictory)
            {
                this.battleUiManager.ShowVictoryResult();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_VICTORY );
            }
            else
            {
                this.battleUiManager.ShowDefeatResult();
                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_DEFEAT );
            }
        } );
    }

    public List<PlayerCharacter> GetPlayerCharacterList()
    {
        return this.playerCharacterList;
    }

    public List<EnemyCharacter> GetEnemyCharacterList()
    {
        return this.enemyCharacterList;
    }

    public List<GameCharacter> GetCharacterList()
    {
        List<GameCharacter> _characters = new List<GameCharacter>();
        _characters.AddRange( this.playerCharacterList );
        _characters.AddRange( this.enemyCharacterList );

        return _characters;
    }

    public PlayerCharacter GetPlayerCharacter()
    {
        return this.playerCharacter;
    }

    public EnemyCharacter GetEnemyCharacter()
    {
        return this.enemyCharacter;
    }

    public BattleUiManager GetBattleUiManager()
    {
        return this.battleUiManager;
    }

    public BattleFlowManager GetBattleFlowManager()
    {
        return this.battleFlowManager;
    }

    public BattleFlowManager_V2 GetBattleFlowManager_V2()
    {
        return this.battleFlowManager_V2;
    }

    public BattleAnimationManager GetBattleAnimationManager()
    {
        return this.battleAnimationManager;
    }

    public BattleAnimationEventManager GetBattleAnimationEventManager()
    {
        return this.battleAnimationEventManager;
    }

    public BattleVisualEffectManager GetBattleVisualEffectManager()
    {
        return this.battleVisualEffectManager;
    }

    public GamePhase GetCurrentGamePhase()
    {
        return this.currentGamePhase;
    }

    public bool HasBattleEnded()
    {
        return ( currentGamePhase == GamePhase.End );
    }
}
