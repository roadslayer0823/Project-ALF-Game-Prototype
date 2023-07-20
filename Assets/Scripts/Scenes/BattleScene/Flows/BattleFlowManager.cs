using System;
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

    private Action onPreparationPhaseStartedCallback = null;
    private Action onExecutionPhaseStartedCallback = null;

    public enum PhaseType
    {
        None,
        GameStarted,
        GameEnded
    }

    public void Initialize( BattleGameManager battleGameManager, Action onPreparationPhaseStartedCallback, Action onExecutionPhaseStartedCallback )
    {
        this.battleGameManager = battleGameManager;
        this.onPreparationPhaseStartedCallback = onPreparationPhaseStartedCallback;
        this.onExecutionPhaseStartedCallback = onExecutionPhaseStartedCallback;
    }

    public void StartGame()
    {
        this.isPlayerFirst = ( UnityEngine.Random.value < 0.5f );
        this.currentPhase = PhaseType.GameStarted;
        StartNewRound();
    }

    private void StartNewRound()
    {
        int _roundNumber = 0;
        if (this.currentRound != null)
        {
            _roundNumber = this.currentRound.GetRoundNumber();
        }

        this.currentRound = new BattleFlowRound( this, _roundNumber + 1, this.isPlayerFirst, OnCurrentRoundPhaseChanged );
        this.currentRound.SetCurrentPhase( BattleFlowRound.PhaseType.Preparation );

        // For next round.
        this.isPlayerFirst = !( this.isPlayerFirst );
    }

    private void OnCurrentRoundPhaseChanged( BattleFlowRound.PhaseType phaseType )
    {
        switch ( phaseType )
        {
            case BattleFlowRound.PhaseType.Preparation:

                if (this.onPreparationPhaseStartedCallback != null)
                {
                    this.onPreparationPhaseStartedCallback();
                }
                else
                {
                    Debug.Log( "The value for 'onPreparationPhaseStartedCallback' is not assigned." );
                }

                break;

            case BattleFlowRound.PhaseType.Execution:

                if (this.onExecutionPhaseStartedCallback != null)
                {
                    this.onExecutionPhaseStartedCallback();
                }
                else
                {
                    Debug.Log( "The value for 'onExecutionPhaseStartedCallback' is not assigned." );
                }

                break;
        }
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
}
