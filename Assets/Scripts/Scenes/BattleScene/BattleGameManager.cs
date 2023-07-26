using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGameManager : MonoBehaviour
{
    [Header( "Managers" )]
    [SerializeField] private BattleUiManager battleUiManager = null;
    [SerializeField] private BattleFlowManager battleFlowManager = null;

    [Header( "Data" )]
    [SerializeField] private SkillDatabase skillDatabase = null;
    [SerializeField] private CharacterDatabase characterDatabase = null;

    [Header( "References" )]
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
        this.battleFlowManager.Initialize( this, OnPreparationPhaseStarted, OnExecutionPhaseStarted, OnExecutionPhaseFinished );

        // -------------------- Set up the player's characters --------------------

        this.playerCharacterList = new List<PlayerCharacter>();
        this.playerCharacter.Initialize( this.characterDatabase.GetPlayerCharacterDataById( 1 ), this.skillDatabase );
        this.playerCharacterList.Add( this.playerCharacter );

        // ------------------------------------------------------------------------

        // -------------------- Set up the enemy's characters --------------------

        this.enemyCharacterList = new List<EnemyCharacter>();
        this.enemyCharacter.Initialize( this.characterDatabase.GetEnemyCharacterDataById( 1 ), this.skillDatabase );
        this.enemyCharacterList.Add( this.enemyCharacter );

        // -----------------------------------------------------------------------

        this.battleFlowManager.StartGame();
    }

    public void StartExecution()
    {
        this.battleFlowManager.GetCurrentRound().SetCurrentPhase( BattleFlowRound.PhaseType.Execution );
    }

    private void OnPreparationPhaseStarted()
    {
        this.battleUiManager.SetSelectedGameCharacter( this.playerCharacterList[ 0 ] );
        this.battleUiManager.ShowSkillSelectionPanel();
        this.battleUiManager.ShowSkillSlotListPanel();
    }

    private void OnExecutionPhaseStarted()
    {
        this.battleUiManager.HideSkillSelectionPanel();
        this.battleUiManager.ShowATLSlotListPanel( this.battleFlowManager.GetCurrentRound().GetFlowATLs() );
        this.battleUiManager.GetSkillSlotListPanel().SetIsSkillSlotListScrollable( true );
        this.battleFlowManager.GetCurrentRound().StartRunningATL();
    }

    private void OnExecutionPhaseFinished()
    {
        this.battleUiManager.HideSkillSlotListPanel();
        this.battleUiManager.HideATLSlotListPanel();
        this.battleUiManager.GetSkillSlotListPanel().SetIsSkillSlotListScrollable( false );
        this.battleFlowManager.StartNewRound();
    }

    public List<PlayerCharacter> GetPlayerCharacterList()
    {
        return this.playerCharacterList;
    }

    public List<EnemyCharacter> GetEnemyCharacterList()
    {
        return this.enemyCharacterList;
    }

    public SkillDatabase GetSkillDatabase()
    {
        return this.skillDatabase;
    }

    public CharacterDatabase GetCharacterDatabase()
    {
        return this.characterDatabase;
    }
}
