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

        for (int i = 0; i < GameConfiguration.Battle.Instance.GetNumberOfATLSlots(); i++)
        {
            BattleFlowATL _atl = new BattleFlowATL( ( _isPlayer ) ? this.battleFlowManager.GetNextPlayerCharacter() : this.battleFlowManager.GetNextEnemyCharacter() );
            _atl.SetAttackTarget( ( _isPlayer ) ? this.battleFlowManager.GetNextEnemyCharacter() : this.battleFlowManager.GetNextPlayerCharacter() );
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
        BattleFlowATL _currentATL = null;

        do
        {
            _currentATL = GetCurrentATL();

            if (_currentATL == null)
            {
                break;
            }

            this.battleFlowManager.OnNewATLStarted();

            ATLSlot _currentATLSlot = _currentATL.GetATLSlot();
            _currentATLSlot.ShowSelectionHighlight();
            _currentATL.SetIsATLSlotExecuted( true );

            yield return battleFlowManager.StartCoroutine( this.battleFlowManager.RunBattleAnimation( this, _currentATL ) );

            if (this.battleFlowManager.GetBattleGameManager().GetHasBattleEnded())
            {
                yield break;
            }

            _currentATL = GetCurrentATL();

            if (_currentATL == null)
            {
                break;
            }

            _currentATLSlot = _currentATL.GetATLSlot();
            _currentATLSlot.MarkATLSlotColorInactive();
            _currentATLSlot.HideSelectionHighlight();

            if (_currentATL.CheckIsPlayer())
            {
                // TODO: Check if the current slot skill is same with the current atl, if not same then dont swipe

                // Auto swipe left the Skill Slot
                _currentATLSlot.onATLSlotExecutedCallback();
                _currentATLSlot.onSkillSlotSwipedCallback();
            }

            this.flowATLIndex++;
        }
        while ( true );

        SetCurrentPhase( PhaseType.ExecutionDone );
    }

    public void GoToTargetATL( BattleFlowATL targetATL, bool isEndable )
    {
        bool _hasTargetATL = false;
        BattleFlowATL _currentATL = null;

        do
        {
            _currentATL = GetCurrentATL();

            if (_currentATL == null)
            {
                break;
            }

            if (_currentATL == targetATL)
            {
                _hasTargetATL = true;
                _currentATL.GetATLSlot().ShowSelectionHighlight();
                break;
            }
            else
            {
                ATLSlot _currentATLSlot = _currentATL.GetATLSlot();
                _currentATLSlot.MarkATLSlotColorInactive();
                _currentATLSlot.HideSelectionHighlight();

                this.flowATLIndex++;
            }
        }
        while ( true );

        if (!_hasTargetATL && isEndable)
        {
            SetCurrentPhase( PhaseType.ExecutionDone );
        }
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

    public BattleFlowATL GetNextATL( GameCharacter gameCharacter )
    {
        BattleFlowATL _nextATL = null;
        int _index = this.flowATLIndex;

        do
        {
            _index++;

            if (_index < this.flowATLs.Length)
            {
                _nextATL = this.flowATLs[ _index ];
            }
            else
            {
                break;
            }

            if (_nextATL.GetSelectedCharacter() == gameCharacter)
            {
                return _nextATL;
            }
        }
        while ( true );

        return null;
    }
}
