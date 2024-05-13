using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowRound_V2
{
    private BattleFlowManager_V2 battleFlowManager = null;
    private int roundNumber = 0;
    private PhaseType currentPhase = PhaseType.None;
    private BattleFlowATL_V2[] flowATLs = null;
    private int flowATLIndex = 0;

    private Action<PhaseType> onCurrentPhaseChangedCallback = null;
    private BattleFlowATL_V2 extraATL = null;

    public enum PhaseType
    {
        None,
        Preparation,
        Execution,
        ExecutionDone
    }

    public BattleFlowRound_V2( BattleFlowManager_V2 battleFlowManager, int roundNumber, Action<PhaseType> onCurrentPhaseChangedCallback )
    {
        this.battleFlowManager = battleFlowManager;
        this.roundNumber = roundNumber;
        this.onCurrentPhaseChangedCallback = onCurrentPhaseChangedCallback;

        List<BattleFlowATL_V2> _atlList = new();
        for (int i = 0; i < GameConfiguration.Instance.GetBattleConfiguration().GetNumberOfATLSlots(); i++)
        {
            _atlList.Add( GetNewATL( i + 1 ) );
        }

        this.flowATLs = _atlList.ToArray();
    }

    public void StartRunningATL()
    {
        this.flowATLIndex = 0;
        this.battleFlowManager.StartCoroutine( RunATLFlow() );
    }

    private IEnumerator RunATLFlow()
    {
        BattleFlowATL_V2 _currentATL = null;

        do
        {
            _currentATL = GetCurrentATL();

            if (_currentATL == null)
            {
                break;
            }

            _currentATL.SetIsATLSlotExecuted( true );

            this.battleFlowManager.OnNewATLStarted();

            yield return battleFlowManager.StartCoroutine( this.battleFlowManager.RunBattleAnimation( this, _currentATL ) );

            if (this.battleFlowManager.GetBattleGameManager().HasBattleEnded())
            {
                yield break;
            }

            if (_currentATL == this.extraATL)
            {
                this.extraATL = null;
            }

            _currentATL = GetCurrentATL();

            if (_currentATL == null)
            {
                break;
            }

            this.flowATLIndex++;
        }
        while ( true );

        //this.battleFlowManager.GetBattleGameManager().GetBattleUiManager().GetATLSlotListPanelV2().GoToFinish( 0.2f );
        this.battleFlowManager.GetBattleGameManager().GetBattleUiManager().GetATLSlotListPanelV3().GoToFinish( 0.2f );
        yield return new WaitForSeconds( 0.3f );

        SetCurrentPhase( PhaseType.ExecutionDone );
    }

    public BattleFlowATL_V2 GoToNextATL()
    {
        this.flowATLIndex++;

        BattleFlowATL_V2 _currentATL = GetCurrentATL();
        if (_currentATL != null)
        {
            _currentATL.SetIsATLSlotExecuted( true );
            this.battleFlowManager.OnNewATLStarted();
        }
        else
        {
            SetCurrentPhase( PhaseType.ExecutionDone );
        }

        return _currentATL;
    }

    private void OnAttackOpportunityEnded()
    {
    }

    public int GetRoundNumber()
    {
        return this.roundNumber;
    }

    public BattleFlowATL_V2[] GetFlowATLs()
    {
        return this.flowATLs;
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

    public BattleFlowATL_V2 GetCurrentATL()
    {
        if (this.flowATLIndex < this.flowATLs.Length)
        {
            return this.flowATLs[ this.flowATLIndex ];
        }
        else if (this.extraATL != null)
        {
            return this.extraATL;
        }

        return null;
    }

    public void AddExtraATL()
    {
        this.extraATL = GetNewATL( GameConfiguration.Instance.GetBattleConfiguration().GetNumberOfATLSlots() + 1 );
    }

    public BattleFlowATL_V2 GetNewATL( int atlNumber )
    {
        return new BattleFlowATL_V2( this.battleFlowManager, atlNumber, GameConfiguration.Instance.GetBattleConfiguration().GetAttackOpportunityDurationInSeconds(), OnAttackOpportunityEnded );
    }
}
