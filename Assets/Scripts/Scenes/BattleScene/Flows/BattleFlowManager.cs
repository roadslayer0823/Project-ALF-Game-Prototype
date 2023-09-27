using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowManager : MonoBehaviour
{
    private BattleGameManager battleGameManager = null;
    private PhaseType currentPhase = PhaseType.None;
    private BattleFlowRound currentRound = null;

    private bool isPlayerFirst = false;
    private int playerCharacterIndex = 0;
    private int enemyCharacterIndex = 0;

    public enum PhaseType
    {
        None,
        GameStarted,
        GameEnded
    }

    public void Initialize( BattleGameManager battleGameManager )
    {
        this.battleGameManager = battleGameManager;
    }

    public void StartGame()
    {
        this.isPlayerFirst = ( UnityEngine.Random.value < 0.5f );
        this.currentPhase = PhaseType.GameStarted;
        StartNewRound();
    }

    public void StartNewRound()
    {
        int _roundNumber = 0;
        if (this.currentRound != null)
        {
            _roundNumber = this.currentRound.GetRoundNumber();
        }

        this.battleGameManager.OnNewRoundStarted( this.isPlayerFirst );
        this.currentRound = new BattleFlowRound( this, _roundNumber + 1, this.isPlayerFirst, OnCurrentRoundPhaseChanged );
        this.currentRound.SetCurrentPhase( BattleFlowRound.PhaseType.Preparation );

        // For next round.
        this.isPlayerFirst = !( this.isPlayerFirst );
    }

    public IEnumerator RunBattleAnimation( BattleFlowRound battleFlowRound, BattleFlowATL battleFlowATL )
    {
        yield return StartCoroutine( battleGameManager.GetBattleAnimationManager().RunBattleAnimation( this.battleGameManager, battleFlowRound, battleFlowATL ) );
    }

    private void OnCurrentRoundPhaseChanged( BattleFlowRound.PhaseType phaseType )
    {
        switch ( phaseType )
        {
            case BattleFlowRound.PhaseType.Preparation:

                this.battleGameManager.OnPreparationPhaseStarted();

                break;

            case BattleFlowRound.PhaseType.Execution:

                this.battleGameManager.OnExecutionPhaseStarted();

                break;

            case BattleFlowRound.PhaseType.ExecutionDone:

                this.battleGameManager.OnExecutionPhaseFinished();

                break;
        }
    }

    public void OnNewATLStarted()
    {
        this.battleGameManager.OnNewATLStarted();
    }

    public PlayerCharacter GetNextPlayerCharacter()
    {
        List<PlayerCharacter> _playerCharacterList = this.battleGameManager.GetPlayerCharacterList();
        PlayerCharacter _playerCharacter = _playerCharacterList[ this.playerCharacterIndex ];
        this.playerCharacterIndex = ( this.playerCharacterIndex + 1 ) % _playerCharacterList.Count;
        return _playerCharacter;
    }

    public EnemyCharacter GetNextEnemyCharacter()
    {
        List<EnemyCharacter> _enemyCharacterList = this.battleGameManager.GetEnemyCharacterList();
        EnemyCharacter _enemyCharacter = _enemyCharacterList[ this.enemyCharacterIndex ];
        this.enemyCharacterIndex = ( this.enemyCharacterIndex + 1 ) % _enemyCharacterList.Count;
        return _enemyCharacter;
    }

    public BattleFlowRound GetCurrentRound()
    {
        return this.currentRound;
    }

    public BattleGameManager GetBattleGameManager()
    {
        return this.battleGameManager;
    }
}
