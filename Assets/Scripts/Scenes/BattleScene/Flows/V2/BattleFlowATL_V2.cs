using System;
using System.Collections;
using UnityEngine;

public class BattleFlowATL_V2
{
    private BattleFlowManager_V2 battleFlowManager = null;
    private int atlNumber = 0;
    private ATLSlotV2 atlSlot = null;
    private bool isATLSlotExecuted = false;

    private float attackOpportunityDuration = 0.0f;
    private bool isDuringAttackOpportunityPeriod = false;
    private Action onAttackOpportunityEndedCallback = null;

    public BattleFlowATL_V2( BattleFlowManager_V2 battleFlowManager, int atlNumber, float attackOpportunityDuration, Action onAttackOpportunityEndedCallback )
    {
        this.battleFlowManager = battleFlowManager;
        this.atlNumber = atlNumber;
        this.attackOpportunityDuration = attackOpportunityDuration;
        this.onAttackOpportunityEndedCallback = onAttackOpportunityEndedCallback;
    }

    public int GetATLNumber()
    {
        return this.atlNumber;
    }

    public void SetATLSlot( ATLSlotV2 atlSlot )
    {
        this.atlSlot = atlSlot;
    }

    public ATLSlotV2 GetATLSlot()
    {
        return this.atlSlot;
    }

    public bool GetIsATLSlotExecuted()
    {
        return this.isATLSlotExecuted;
    }

    public void SetIsATLSlotExecuted( bool isATLSlotExecuted )
    {
        this.isATLSlotExecuted = isATLSlotExecuted;
    }

    public void StartAttackOpportunityCountdownTimer( SkillPromptPanelV2 skillPromptPanel )
    {
        this.isDuringAttackOpportunityPeriod = true;
        this.battleFlowManager.StartCoroutine( RunAttackOpportunityCountdownTimer( skillPromptPanel ) );
    }

    private IEnumerator RunAttackOpportunityCountdownTimer( SkillPromptPanelV2 skillPromptPanel )
    {
        skillPromptPanel.SetCommandPhaseProgressBar( 1.0f, true, true );
        skillPromptPanel.SetCommandPhaseProgressBar( 1.0f, true, false );

        //float _attackOpportunityStartTime = Time.realtimeSinceStartup;
        float _attackOpportunityStartTime = Time.time;
        float _remainingTime = 0.0f;

        do
        {
            yield return null;

            //_remainingTime = this.attackOpportunityDuration - ( Time.realtimeSinceStartup - _attackOpportunityStartTime );
            _remainingTime = this.attackOpportunityDuration - ( Time.time - _attackOpportunityStartTime );

            float _remainingTimePercentage = _remainingTime / this.attackOpportunityDuration;
            skillPromptPanel.SetCommandPhaseProgressBar( _remainingTimePercentage, true, true );
            skillPromptPanel.SetCommandPhaseProgressBar( _remainingTimePercentage, true, false );
        }
        while (_remainingTime > 0);

        this.isDuringAttackOpportunityPeriod = false;
        this.onAttackOpportunityEndedCallback?.Invoke();
    }

    public float GetAttackOpportunityDuration()
    {
        return this.attackOpportunityDuration;
    }

    public bool GetIsDuringAttackOpportunityPeriod()
    {
        return this.isDuringAttackOpportunityPeriod;
    }
}
