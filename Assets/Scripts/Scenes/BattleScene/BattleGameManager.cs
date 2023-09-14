using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        AudioManager.Instance.SetUpAudioDatabase( this.audioDatabase );

        this.battleUiManager.HideSkillSelectionPanel();
        this.battleUiManager.HideSkillSlotListPanel();
        this.battleUiManager.HideATLSlotListPanel();
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

        this.battleUiManager.GetCharacterInfoPanel().SetSelectedCharacter( this.playerCharacter );
        this.battleFlowManager.StartGame();

        AudioManager.Instance.PlayBackgroundMusic( this.backgroundMusicAudioClip, 0.5f );
    }

    public void StartExecution()
    {
        this.battleFlowManager.GetCurrentRound().SetCurrentPhase( BattleFlowRound.PhaseType.Execution );
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
        this.battleUiManager.HideSkillSlotListPanel();
        this.battleUiManager.HideATLSlotListPanel();
        this.battleUiManager.GetSkillSlotListPanel().SetIsSkillSlotListScrollable( false );
        this.battleUiManager.GetSkillSlotListPanel().ResetLastRoundSelectedActiveSkill();
        this.battleUiManager.CheckWhetherToEnableExecuteButton();
        this.battleFlowManager.StartNewRound();
    }

    public void OnNewRoundStarted()
    {
        List<GameCharacter> _characters = new List<GameCharacter>();
        _characters.AddRange( this.playerCharacterList );
        _characters.AddRange( this.enemyCharacterList );

        for (int i = 0; i < _characters.Count; i++)
        {
            GameCharacter _character = _characters[ i ];

            float _currentStatePoint = _character.GetCurrentStatePoint();
            if (_currentStatePoint < 0)
            {
                _character.MinusMaximumStatePoint( Mathf.Abs( _currentStatePoint ) );
            }

            _character.SetCurrentStatePointToMaximum();
            _character.ResetCounterAttacks();
        }
    }

    public void OnNewATLStarted()
    {
        this.battleUiManager.DisablePlayerActionPanelButtons();
    }

    private void OnCharacterEventTriggered( BattleAnimationManager.AnimationEvent animationEvent, GameCharacter gameCharacter )
    {
        gameCharacter.OnEventTriggered( this, animationEvent );

        if (animationEvent == BattleAnimationManager.AnimationEvent.OnBeingInBreakStatus)
        {
            this.battleFlowManager.GetCurrentRound().UpdateATLSlotStatuses( gameCharacter, false );
        }
    }

    private void OnBattleEnded( bool isVictory )
    {
        this.hasBattleEnded = true;

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
    }

    public List<PlayerCharacter> GetPlayerCharacterList()
    {
        return this.playerCharacterList;
    }

    public List<EnemyCharacter> GetEnemyCharacterList()
    {
        return this.enemyCharacterList;
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
