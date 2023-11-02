using System.Collections.Generic;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;

public class BattleGameManager : MonoBehaviour
{
    [Header( "Managers" )]
    [SerializeField] private BattleUiManager battleUiManager = null;
    [SerializeField] private BattleFlowManager battleFlowManager = null;
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
        this.battleFlowManager.Initialize( this );
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

        AudioManager.Instance.PlayBackgroundMusic( this.backgroundMusicAudioClip );
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CROSSING_ACTION_MODE, () =>
        {
            this.battleUiManager.SetAllActive( true );
            this.battleUiManager.HideSkillSelectionPanel();
            this.battleUiManager.HideSkillSlotListPanel();
            this.battleUiManager.HideATLSlotListPanel();

            this.battleUiManager.GetCharacterInfoPanel().SetSelectedCharacter( this.playerCharacter );
            this.battleFlowManager.StartGame();

            AudioManager.Instance.PlaySoundEffect( AUDIO_ID_COMMAND_PHASE );
        } );

        this.battleUiManager.PlayInstructionAnimation();
    }

    public void StartExecution()
    {
        this.battleFlowManager.GetCurrentRound().SetCurrentPhase( BattleFlowRound.PhaseType.Execution );
        this.battleUiManager.GetCharacterInfoPanel().HideRoundInfoText();
    }

    public void OnPreparationPhaseStarted()
    {
        this.battleUiManager.SetSelectedGameCharacter( this.playerCharacter );
        this.battleUiManager.ShowSkillSelectionPanel();
        this.battleUiManager.ShowSkillSlotListPanel();
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
        this.playerCharacter.PlayCharacterAnimation( "Idle" );
        this.enemyCharacter.PlayCharacterAnimation( "Idle" );

        this.battleUiManager.HideSkillSelectionPanel();
        this.battleUiManager.ShowATLSlotListPanel( this.battleFlowManager.GetCurrentRound().GetFlowATLs() );
        this.battleUiManager.GetSkillSlotListPanel().SetIsSkillSlotListScrollable( true );
        this.battleFlowManager.GetCurrentRound().StartRunningATL();
    }

    public void OnExecutionPhaseFinished()
    {
        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>【 第 { this.battleFlowManager.GetCurrentRound().GetRoundNumber() } 回合結束 】</color>" );

        BattleLogicManager.OnExecutionPhaseFinished( GetCharacterList() );

        this.battleUiManager.HideSkillSlotListPanel();
        this.battleUiManager.HideATLSlotListPanel();
        this.battleUiManager.GetSkillSlotListPanel().SetIsSkillSlotListScrollable( false );
        this.battleUiManager.GetSkillSlotListPanel().ResetLastRoundSelectedActiveSkill();
        this.battleUiManager.CheckWhetherToEnableExecuteButton();
        this.battleFlowManager.StartNewRound();
    }

    public void OnNewRoundStarted( bool isPlayerFirst )
    {
        this.battleUiManager.GetCharacterInfoPanel().ShowRoundInfoText( isPlayerFirst );

        if (battleFlowManager.GetCurrentRound().GetRoundNumber() > 1)
        {
            BattleLogicManager.OnNewRoundStarted( GetCharacterList() );
        }
    }

    public void OnNewATLStarted()
    {
        BattleLogicManager.OnNewATLStarted( GetCharacterList() );

        this.battleUiManager.DisablePlayerActionPanelButtons();
    }

    private void OnCharacterEventTriggered( BattleAnimationManager.AnimationEvent animationEvent, GameCharacter gameCharacter )
    {
        gameCharacter.OnEventTriggered( this, animationEvent );

        switch ( animationEvent )
        {
            case BattleAnimationManager.AnimationEvent.OnBeingInBreakStatus:

                AudioManager.Instance.PlaySoundEffect( AUDIO_ID_BREAK );
                this.battleFlowManager.GetCurrentRound().UpdateATLSlotStatuses( gameCharacter, false );

                break;

            case BattleAnimationManager.AnimationEvent.OnSkillBeingObserved:

                CharacterSkill _gameCharacterCurrentSkill = gameCharacter.GetCurrentSkill();
                Subskill _gameCharacterCurrentSubskillData = _gameCharacterCurrentSkill.GetCharacterSubskillData().GetSubskillData();
                GameCharacter _currentCaster = battleAnimationManager.GetCurrentCaster();
                CharacterSkill _currentCasterSkill = _currentCaster.GetCurrentSkill();
                Subskill _currentCasterSubskillData = _currentCasterSkill.GetCharacterSubskillData().GetSubskillData();

                float _currentObservedRate = _gameCharacterCurrentSkill.AddObservedSkillData( _currentCasterSubskillData.FeatureId, _currentCasterSubskillData.DisplayName, _currentCasterSkill.GetSkillData().skillType,
                                                                                              _gameCharacterCurrentSubskillData.ObservationRate );

                BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + gameCharacter.GetCharacterName() + "</color>使用" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _gameCharacterCurrentSubskillData.DisplayName + "</color>（看破技能）來看破"
                                                         + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _currentCaster.GetCharacterName() + "</color>使用的" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _currentCasterSubskillData.DisplayName + "</color>和對"
                                                         + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _currentCasterSubskillData.DisplayName + "</color>的看破值現為" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _currentObservedRate.ConvertToIntegerInPercentage() + "%</color>。" );

                break;
        }
    }

    private void OnBattleEnded( bool isVictory )
    {
        this.hasBattleEnded = true;

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

    public BattleUiManager GetBattleUiManager()
    {
        return this.battleUiManager;
    }

    public BattleFlowManager GetBattleFlowManager()
    {
        return this.battleFlowManager;
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
