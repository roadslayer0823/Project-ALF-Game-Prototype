using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowRound
{
    private BattleFlowManager battleFlowManager = null;
    private int roundNumber = 0;
    private PhaseType currentPhase = PhaseType.None;
    private BattleFlowATL[] flowATLs = null;
    private int flowATLIndex = 0;

    private Action<PhaseType> onCurrentPhaseChangedCallback = null;

    public enum PhaseType
    {
        None,
        Preparation,
        Execution,
        ExecutionDone
    }

    public BattleFlowRound( BattleFlowManager battleFlowManager, int roundNumber, bool isPlayerFirst, Action<PhaseType> onCurrentPhaseChangedCallback )
    {
        this.battleFlowManager = battleFlowManager;
        this.roundNumber = roundNumber;
        this.onCurrentPhaseChangedCallback = onCurrentPhaseChangedCallback;

        List<BattleFlowATL> _atlList = new List<BattleFlowATL>();
        bool _isPlayer = isPlayerFirst;

        for (int i = 0; i < GameConfiguration.NUMBER_OF_ATL_SLOTS; i++)
        {
            _atlList.Add( new BattleFlowATL( ( _isPlayer ) ? this.battleFlowManager.GetNextPlayerCharacter() : this.battleFlowManager.GetNextEnemyCharacter() ) );
            _isPlayer = !( _isPlayer );
        }

        this.flowATLs = _atlList.ToArray();
    }

    public void SetCurrentPhase( PhaseType currentPhase )
    {
        this.currentPhase = currentPhase;

        if (this.onCurrentPhaseChangedCallback != null)
        {
            this.onCurrentPhaseChangedCallback( this.currentPhase );
        }
        else
        {
            Debug.Log( "The value for 'onCurrentPhaseChangedCallback' is not assigned." );
        }
    }

    public PhaseType GetCurrentPhase()
    {
        return this.currentPhase;
    }

    public void StartRunningATL()
    {
        this.flowATLIndex = 0;
        this.battleFlowManager.StartCoroutine( RunATLFlow() );
    }

    private IEnumerator RunATLFlow()
    {
        while (this.flowATLIndex < this.flowATLs.Length)
        {
            yield return new WaitForSeconds( 2.0f );
            this.flowATLIndex++;
        }

        SetCurrentPhase( PhaseType.ExecutionDone );
    }

    public int GetRoundNumber()
    {
        return this.roundNumber;
    }

    public BattleFlowATL[] GetFlowATLs()
    {
        return this.flowATLs;
    }

    public BattleFlowATL GetCurrentATL()
    {
        return this.flowATLs[ this.flowATLIndex ];
    }
}
