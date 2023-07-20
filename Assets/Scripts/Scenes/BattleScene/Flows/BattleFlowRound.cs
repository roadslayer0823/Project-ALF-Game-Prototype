using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFlowRound
{
    private BattleFlowManager battleFlowManager = null;
    private int roundNumber = 0;
    private PhaseType currentPhase = PhaseType.None;
    private BattleFlowATL[] flowATLs = null;

    public enum PhaseType
    {
        None,
        Preparation,
        Execution
    }

    public BattleFlowRound( BattleFlowManager battleFlowManager, int roundNumber, bool isPlayerFirst )
    {
        this.battleFlowManager = battleFlowManager;
        this.roundNumber = roundNumber;

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
    }

    public int GetRoundNumber()
    {
        return this.roundNumber;
    }
}
