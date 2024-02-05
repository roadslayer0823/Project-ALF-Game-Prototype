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
    private bool hasBattleEnded = false;

    private const string AUDIO_ID_VICTORY = "victory";
    private const string AUDIO_ID_DEFEAT = "defeat";
    private const string AUDIO_ID_BREAK = "break";
    private const string AUDIO_ID_CROSSING_ACTION_MODE = "crossing_action_mode";
    private const string AUDIO_ID_COMMAND_PHASE = "command_phase";
    private const string AUDIO_ID_LINE_BREAK = "line_break";

    void Awake()
    {
        AudioManager.Instance.SetUpAudioDatabase( this.audioDatabase );

        this.battleUiManager.SetAllActive( false );
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

        this.battleAnimationManager.Initialize( OnBattleEnded );

        // -------------------- Set up the player's characters --------------------

        this.playerCharacterList = new List<PlayerCharacter>();
        this.playerCharacter.Initialize( DatabaseManager.Instance.GetCharacterDataById( "C1" ), this.playerContainer, this.opponentContainer, OnCharacterEventTriggered );
        this.playerCharacterList.Add( this.playerCharacter );

        // ------------------------------------------------------------------------

        // -------------------- Set up the enemy's characters --------------------

        this.enemyCharacterList = new List<EnemyCharacter>();
        this.enemyCharacter.Initialize( DatabaseManager.Instance.GetCharacterDataById( "E1" ), this.opponentContainer, this.playerContainer, OnCharacterEventTriggered );
        this.enemyCharacterList.Add( this.enemyCharacter );

        // -----------------------------------------------------------------------

        AudioManager.Instance.PlayBackgroundMusic( clip: this.backgroundMusicAudioClip, loopStartTime: 35.142f );

        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CROSSING_ACTION_MODE, () =>
        {
            this.battleUiManager.SetAllActive( true );
            this.battleUiManager.HideSkillSelectionPanel();
            this.battleUiManager.HideSkillSlotListPanel();
            this.battleUiManager.HideATLSlotListPanel();

            this.battleUiManager.GetCharacterInfoPanel().SetSelectedCharacter( this.playerCharacter );

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

        this.battleUiManager.GetCharacterInfoPanel().HideRoundInfoText();
    }

    public void OnPreparationPhaseStarted()
    {
        this.battleUiManager.SetSelectedGameCharacter( this.playerCharacter );
        this.battleUiManager.HideATLSlotListPanel();
        this.battleUiManager.ShowSkillSelectionPanel();
        this.battleUiManager.ShowPreparationSection();

        this.battleAnimationManager.ChangeToBackgroundPartB();
        this.playerContainer.SetActive( true );
        this.opponentContainer.SetActive( true );
        this.playerCharacter.PlayCharacterAnimation( "Prepare" );
        this.enemyCharacter.PlayCharacterAnimation( "Idle" );
        this.enemyCharacter.InitializeSelectedSkills();
    }

    public void OnExecutionPhaseStarted()
    {
        this.battleUiManager.ShowBattleSection( this.playerCharacter );
        this.battleUiManager.HideSkillSelectionPanel();
        this.battleUiManager.ShowSkillSlotListPanel();

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

        BattleLogicManager.OnExecutionPhaseFinished( GetCharacterList() );

        this.battleUiManager.HideSkillSlotListPanel();
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
        }
        else
        {
            _roundNumber = this.battleFlowManager_V2.GetCurrentRound().GetRoundNumber();
        }

        if (_roundNumber > 1)
        {
            BattleLogicManager.OnNewRoundStarted( GetCharacterList() );
        }
    }

    public void OnNewRoundStarted( bool isPlayerFirst )
    {
        this.battleUiManager.GetCharacterInfoPanel().ShowRoundInfoText( isPlayerFirst );
        OnNewRoundStarted();
    }

    public void OnNewATLStarted()
    {
        BattleLogicManager.OnNewATLStarted( GetCharacterList() );

        this.battleUiManager.DisablePlayerActionPanelButtons();
    }

    private void OnCharacterEventTriggered( BattleAnimationManager.AnimationEvent animationEvent, GameCharacter gameCharacter )
    {
        if (BattleLogicManager.IsGameCharacterDead( gameCharacter ))
        {
            return;
        }

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

                GameCharacter _currentCaster = battleAnimationManager.GetCurrentCaster();
                _currentObservingSkill.SetObservedSkill( _currentCaster, _currentCaster.GetCurrentSkill() );

                break;

            case BattleAnimationManager.AnimationEvent.OnSkillBeingObserved:

                CharacterSkill _gameCharacterCurrentSkill = gameCharacter.GetCurrentObservingSkill();
                Subskill _gameCharacterCurrentSubskillData = _gameCharacterCurrentSkill.GetCharacterSubskillData().GetSubskillData();

                GameCharacter _observedSkillCaster = _currentObservingSkill.GetObservedSkillCaster();
                CharacterSkill _observedSkill = _currentObservingSkill.GetObservedSkill();
                Subskill _observedSubskillData = _observedSkill.GetCharacterSubskillData().GetSubskillData();

                float _currentObservedRate = _gameCharacterCurrentSkill.AddObservedSkillData( _observedSubskillData.FeatureId, _observedSubskillData.DisplayName, _observedSkill.GetSkillData().skillType,
                                                                                              _gameCharacterCurrentSubskillData.ObservationRate );

                BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + gameCharacter.GetCharacterName() + "</color>使用" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _gameCharacterCurrentSubskillData.DisplayName + "</color>（看破技能）來看破"
                                                         + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _observedSkillCaster.GetCharacterName() + "</color>使用的" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _observedSubskillData.DisplayName + "</color>和對"
                                                         + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _observedSubskillData.DisplayName + "</color>的看破值現為" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _currentObservedRate.ConvertToIntegerInPercentage() + "%</color>。" );

                break;
        }
    }

    private void OnBattleEnded( bool isVictory )
    {
        this.hasBattleEnded = true;

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

    public bool GetHasBattleEnded()
    {
        return this.hasBattleEnded;
    }
}
