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

        for (int i = 0; i < GameConfiguration.Instance.GetBattleConfiguration().GetNumberOfATLSlots(); i++)
        {
            BattleFlowATL _atl = new BattleFlowATL( i + 1, ( _isPlayer ) ? this.battleFlowManager.GetNextPlayerCharacter() : this.battleFlowManager.GetNextEnemyCharacter() );
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
        BattleGameManager _battleGameManager = this.battleFlowManager.GetBattleGameManager();

        List<GameCharacter> _gameCharacterList = new List<GameCharacter>();
        _gameCharacterList.AddRange( _battleGameManager.GetPlayerCharacterList() );
        _gameCharacterList.AddRange( _battleGameManager.GetEnemyCharacterList() );

        for (int i = 0; i < _gameCharacterList.Count; i++)
        {
            UpdateATLSlotStatuses( _gameCharacterList[ i ], true );
        }

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

            _currentATL.SetIsATLSlotExecuted( true );
            this.battleFlowManager.OnNewATLStarted();

            ATLSlot _currentATLSlot = _currentATL.GetATLSlot();
            _currentATLSlot.ShowSelectionHighlight();

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

            _currentATL.Finish();
            this.flowATLIndex++;
        }
        while ( true );

        this.battleFlowManager.GetBattleGameManager().GetBattleUiManager().GetATLSlotListPanelV2().GoToFinish( 0.2f );
        yield return new WaitForSeconds( 0.3f );

        SetCurrentPhase( PhaseType.ExecutionDone );
    }

    public void GoToTargetATL( BattleFlowATL targetATL, bool isEndable )
    {
        bool _hasTargetATL = false;
        BattleFlowATL _currentATL = GetCurrentATL();

        do
        {
            _currentATL?.Finish();

            this.flowATLIndex++;
            _currentATL = GetCurrentATL();

            if (_currentATL == null)
            {
                break;
            }

            _currentATL.SetIsATLSlotExecuted( true );
            this.battleFlowManager.OnNewATLStarted();

            if (_currentATL == targetATL)
            {
                _hasTargetATL = true;
                _currentATL.GetATLSlot().ShowSelectionHighlight();
                break;
            }
        }
        while ( true );

        if (!_hasTargetATL && isEndable)
        {
            SetCurrentPhase( PhaseType.ExecutionDone );
        }
    }

    public void UpdateATLSlotStatuses( GameCharacter gameCharacter, bool isNewRound )
    {
        if (gameCharacter.GetIsInBreakStatus())
        {
            int _breakStatusRemainingATLs = gameCharacter.GetBreakStatusRemainingATLs();
            BattleFlowATL _currentATL = GetCurrentATL();

            int _theATLIndex = this.flowATLIndex;
            int _nextATLIndex = 0;

            if (_currentATL != null)
            {
                if (_currentATL.GetSelectedCharacter() == gameCharacter)
                {
                    _currentATL.GetATLSlot().Deactivate();

                    if (isNewRound)
                    {
                        _breakStatusRemainingATLs--;
                    }
                }
            }

            while (_breakStatusRemainingATLs > 0)
            {
                BattleFlowATL _nextATL = GetNextATL( gameCharacter, _theATLIndex, out _nextATLIndex );
                if (_nextATL != null)
                {
                    _nextATL.GetATLSlot().Deactivate();
                    _breakStatusRemainingATLs--;
                    _theATLIndex = _nextATLIndex;
                }
                else
                {
                    break;
                }
            }
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
        return GetNextATL( gameCharacter, this.flowATLIndex );
    }

    public BattleFlowATL GetNextATL( GameCharacter gameCharacter, out int nextATLIndex )
    {
        return GetNextATL( gameCharacter, this.flowATLIndex, out nextATLIndex );
    }

    public BattleFlowATL GetNextATL( GameCharacter gameCharacter, int fromATLIndex )
    {
        return GetNextATL( gameCharacter, fromATLIndex, out _ );
    }

    public BattleFlowATL GetNextATL( GameCharacter gameCharacter, int fromATLIndex, out int nextATLIndex )
    {
        BattleFlowATL _nextATL = null;
        nextATLIndex = fromATLIndex;

        do
        {
            nextATLIndex++;

            if (nextATLIndex < this.flowATLs.Length)
            {
                _nextATL = this.flowATLs[ nextATLIndex ];
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
