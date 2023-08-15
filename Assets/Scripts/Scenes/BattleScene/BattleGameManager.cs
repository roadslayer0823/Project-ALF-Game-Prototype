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

    private List<PlayerCharacter> playerCharacterList = null;
    private List<EnemyCharacter> enemyCharacterList = null;

    void Awake()
    {
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
        for (int i = 0; i < this.playerCharacterList.Count; i++)
        {
            this.playerCharacterList[ i ].SetRemainingStatePointToMaximum();
        }

        for (int i = 0; i < this.enemyCharacterList.Count; i++)
        {
            this.enemyCharacterList[ i ].SetRemainingStatePointToMaximum();
        }
    }

    public void OnNewATLStarted()
    {
        this.battleUiManager.DisablePlayerActionPanelButtons();
    }

    private void OnCharacterEventTriggered( BattleAnimationManager.AnimationEvent animationEvent, GameCharacter gameCharacter )
    {
        gameCharacter.OnEventTriggered( this, animationEvent );
    }

    private void OnBattleEnded( bool isVictory )
    {
        if (isVictory)
        {
            this.battleUiManager.ShowVictoryResult();
        }
        else
        {
            this.battleUiManager.ShowDefeatResult();
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
}
