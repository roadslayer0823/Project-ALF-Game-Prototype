using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
            BattleFlowATL _atl = new BattleFlowATL( ( _isPlayer ) ? this.battleFlowManager.GetNextPlayerCharacter() : this.battleFlowManager.GetNextEnemyCharacter() );
            _atl.SetAttackTarget( ( _isPlayer ) ? this.battleFlowManager.GetNextEnemyCharacter() : this.battleFlowManager.GetNextPlayerCharacter() );

            if (_atl.GetSelectedCharacter() is EnemyCharacter)
            {
                CharacterSkill[] _skills = _atl.GetSelectedCharacter().GetSkills();
                _atl.SetSelectedSkill( _skills[ Random.Range( 0, _skills.Length ) ] );
            }

            _atlList.Add( _atl );

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
        BattleFlowATL _currentATL = GetCurrentATL();

        while (_currentATL != null)
        {
            BattleFlowATL currentBattleFlowATL = this.flowATLs[this.flowATLIndex];
            ATLSlot currentATLSlot = currentBattleFlowATL.GetATLSlot();

            currentATLSlot.ShowSelectionHighlight();
            currentBattleFlowATL.SetIsATLSlotExecuted(true);

            yield return battleFlowManager.StartCoroutine( this.battleFlowManager.RunBattleAnimation( _currentATL ) );

            currentATLSlot.MarkATLSlotColorInactive();
            currentATLSlot.HideSelectionHighlight();

            if (this.flowATLs[this.flowATLIndex].CheckIsPlayer())
            {
                // Auto swipe left the Skill Slot
                currentATLSlot.onATLSlotExecutedCallback();
                currentATLSlot.onSkillSlotSwipedCallback();
            }

            this.flowATLIndex++;
            _currentATL = GetCurrentATL();
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
        if (this.flowATLIndex < this.flowATLs.Length)
        {
            return this.flowATLs[ this.flowATLIndex ];
        }

        return null;
    }
}
