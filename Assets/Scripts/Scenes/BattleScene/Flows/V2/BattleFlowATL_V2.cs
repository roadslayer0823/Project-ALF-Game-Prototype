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

    public void StartAttackOpportunityCountdownTimer()
    {
        this.isDuringAttackOpportunityPeriod = true;
        this.battleFlowManager.StartCoroutine( RunAttackOpportunityCountdownTimer() );
    }

    private IEnumerator RunAttackOpportunityCountdownTimer()
    {
        float _attackOpportunityStartTime = Time.realtimeSinceStartup;

        do
        {
            yield return null;
        }
        while (Time.realtimeSinceStartup - _attackOpportunityStartTime < this.attackOpportunityDuration);

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
